using Microsoft.Extensions.DependencyInjection;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Threading.Tasks;
using System.Threading.Tasks.Dataflow;
using Fasterflect;
using FineDataFlow.Engine.Abstractions;

namespace FineDataFlow.Engine.Implementations
{
	internal class StepImpl : IStep
	{
		// fields

		public readonly IRun _run;
		private IRowErrorOutbox _rowErrorOutbox;
		private IStepCompleteOutbox _stepCompleteOutbox;
		private readonly IServiceProvider _serviceProvider;

		// properties

		public string Name { get; set; }
		public bool Enabled { get; set; }
		public string PluginId { get; set; }
		public Type PluginType { get; set; }
		public object PluginObject { get; set; }
		public List<IInbox> Inboxes { get; } = new();
		public List<IOutbox> Outboxes { get; } = new();
		
		// constructors

		public StepImpl
		(
			IRun run,
			IServiceProvider serviceProvider
		)
		{
			_run = run ?? throw new ArgumentNullException(nameof(run));
			_serviceProvider = serviceProvider ?? throw new ArgumentNullException(nameof(serviceProvider));
		}

		// methods

		public void Initialize()
		{
			if (PluginType == null)
			{
				throw new InvalidOperationException($"{nameof(PluginType)} is required");
			}

			var stepAttribute = PluginType.GetCustomAttribute<StepPluginAttribute>();

			if (stepAttribute == null)
			{
				throw new InvalidOperationException($"{nameof(PluginType)} must have {nameof(StepPluginAttribute)} defined");
			}

			if (!(PluginObject == null && PluginType.IsAbstract && PluginType.IsSealed)) // static check
			{
				PluginObject = Activator.CreateInstance(PluginType);
			}

			// create inboxes

			foreach (var attributeInterfacePair in new[]
			{
				new { AttributeType = typeof(SeedInboxAttribute), InterfaceType = typeof(ISeedInbox) },
				new { AttributeType = typeof(AllRowsInboxAttribute), InterfaceType = typeof(IAllRowsInbox) },
				new { AttributeType = typeof(RowStreamInboxAttribute), InterfaceType = typeof(IRowStreamInbox) },
			})
			{
				foreach (var member in PluginType.GetMethods(BindingFlags.Instance | BindingFlags.Static | BindingFlags.Public | BindingFlags.NonPublic).Cast<MemberInfo>())
				{
					var attribute = member.GetCustomAttribute(attributeInterfacePair.AttributeType, true);
					
					if (attribute == null)
					{
						continue;
					}

					var inbox = (IInbox)_serviceProvider.GetRequiredService(attributeInterfacePair.InterfaceType);
					inbox.Step = this;
					inbox.Attribute = attribute;
					inbox.StepPluginType = PluginType;
					inbox.StepPluginObject = PluginObject;
					inbox.StepPluginObjectMember = member;
					inbox.Initialize();
					Inboxes.Add(inbox);
				}
			}

			// create outboxes

			foreach (var attributeInterfacePair in new[]
			{
				new { AttributeType = typeof(SuccessOutboxAttribute), InterfaceType = typeof(ISuccessOutbox) },
			})
			{
				foreach (var member in PluginType.GetProperties(BindingFlags.Instance | BindingFlags.Static | BindingFlags.Public | BindingFlags.NonPublic).Cast<MemberInfo>().Concat(PluginType.GetFields(BindingFlags.Instance | BindingFlags.Static | BindingFlags.Public | BindingFlags.NonPublic)))
				{
					var attribute = member.GetCustomAttribute(attributeInterfacePair.AttributeType, true);
					
					if (attribute == null)
					{
						continue;
					}

					var outbox = (IOutbox)_serviceProvider.GetRequiredService(attributeInterfacePair.InterfaceType);
					outbox.Step = this;
					outbox.Attribute = attribute;
					outbox.StepPluginType = PluginType;
					outbox.StepPluginObject = PluginObject;
					outbox.StepPluginObjectMember = member;
					outbox.Initialize();
					Outboxes.Add(outbox);
				}
			}

			_rowErrorOutbox = _serviceProvider.GetRequiredService<IRowErrorOutbox>();
			_rowErrorOutbox.Step = this;
			_rowErrorOutbox.StepPluginType = PluginType;
			_rowErrorOutbox.StepPluginObject = PluginObject;
			_rowErrorOutbox.Initialize();
			Outboxes.Add(_rowErrorOutbox);

			_stepCompleteOutbox = _serviceProvider.GetRequiredService<IStepCompleteOutbox>();
			_stepCompleteOutbox.StepPluginType = PluginType;
			_stepCompleteOutbox.StepPluginObject = PluginObject;
			_stepCompleteOutbox.Initialize();
			Outboxes.Add(_stepCompleteOutbox);
		}

		public async Task RunAsync()
		{
			// setup inboxes and outboxes

			var allRowsInboxesCompletionTask = default(Task);

			void evaluateInboxCompletion(IInbox inbox)
			{
				Task.Factory.StartNew(() =>
				{
					lock (this)
					{
						if (inbox.ActionBlock.InputCount == 0 && (inbox.FromOutbox == null || inbox.FromOutbox.Step.Inboxes.Count == 0 || inbox.FromOutbox.Step.Inboxes.All(x => x.ActionBlock.Completion.IsCompleted)))
						{
							Inboxes
								.AsParallel()
								.Where(x => !x.ActionBlock.Completion.IsCompleted)
								.ForAll(x =>
								{
									x.DoneAsync();
									x.ActionBlock.Complete();
								});
						}
					}
				});
			}

			Inboxes
				.AsParallel()
				.ForAll(inbox =>
				{
					inbox.ActionBlock = new ActionBlock<Row>(async row =>
					{
						try
						{
							if (inbox is not IAllRowsInbox)
							{
								await allRowsInboxesCompletionTask;
							}

							await inbox.ProcessRowAsync(row);
						}
						catch (Exception exception)
						{
							if (_rowErrorOutbox.ActionBlock == null)
							{
								throw; // no handlers -> blow up
							}

							if (row == null)
							{
								row = new Row();
							}

							row["$Error:"] = true;
							row["$Error:Step"] = Name;
							row["$Error:Inbox"] = inbox.Name;
							row["$Error:Message"] = exception.Message;
							row["$Error:StackTrace"] = exception.StackTrace;
							row["$Error:Name"] = exception.GetType().FullName;

							_rowErrorOutbox.AddRow(row);
						}
						finally
						{
							// completion detection and handling

							evaluateInboxCompletion(inbox);
						}
					},
					new ExecutionDataflowBlockOptions
					{
						CancellationToken = _run.CancellationToken
					});
					
					if (inbox.FromOutbox != null)
					{
						inbox.FromOutbox.ActionBlock = inbox.ActionBlock;
					}
				});

			allRowsInboxesCompletionTask = Task.WhenAll
			(
				Inboxes
					.AsParallel()
					.Where(inbox => inbox is IAllRowsInbox)
					.Select(inbox => inbox.ActionBlock.Completion)
			);

			// initialize step

			PluginType
				.GetMethods(BindingFlags.Instance | BindingFlags.Static | BindingFlags.Public | BindingFlags.NonPublic)
				.AsParallel()
				.ForAll(async method =>
				{
					var attribute = method.GetCustomAttribute<InitializeAttribute>();

					if (attribute == null)
					{
						return;
					}

					object result;

					if (method.IsStatic)
					{
						result = method.Call();
					}
					else
					{
						result = method.Call(PluginObject);
					}

					var resultTasks = new List<Task>();

					if (result is Task resultTask)
					{
						resultTasks.Add(resultTask);
					}

					await Task.WhenAll(resultTasks);
				});

			// collect runnable tasks

			var runnableTasks = new List<Task>();

			runnableTasks.AddRange(
				Inboxes
					.Select(inbox => inbox.ActionBlock?.Completion)
					.Where(inbox => inbox is not null)
			);

			runnableTasks.AddRange(
				Outboxes
					.Where(outbox => outbox != _stepCompleteOutbox)
					.Select(outbox => outbox.ActionBlock?.Completion)
					.Where(outbox => outbox is not null)
			);

			runnableTasks.Add
			(
				Task.Run(() =>
				{
					Inboxes
						.AsParallel()
						.Where(inbox => inbox is ISeedInbox)
						.ForAll(seedInbox =>
						{
							var seedRow = new Row();
							seedRow["$Seed:"] = true;
							seedInbox.ActionBlock.Post(seedRow);
						});

					Inboxes
						.AsParallel()
						.Where(inbox => inbox is not ISeedInbox)
						.ForAll(notSeedInbox => evaluateInboxCompletion(notSeedInbox));
				})
			);

			// return

			await Task
				.WhenAll(runnableTasks)
				.ContinueWith(t =>
				{
					var stepCompleteRow = new Row();
					stepCompleteRow["$StepComplete:"] = true;
					stepCompleteRow["$StepComplete:Step"] = Name;
					//TODO:Add other relvant info, e.g. TotalInput,TotalOutput,Rows/Sec,e.t.c
					_stepCompleteOutbox.AddRow(stepCompleteRow);

				}, _run.CancellationToken);
		}

		public void Dispose()
		{
			PluginType
				?.GetMethods(BindingFlags.Instance | BindingFlags.Static | BindingFlags.Public | BindingFlags.NonPublic)
				?.AsParallel()
				?.ForAll(async method =>
				{
					var attribute = method.GetCustomAttribute<DestroyAttribute>();

					if (attribute == null)
					{
						return;
					}

					object result;

					if (method.IsStatic)
					{
						result = method.Call();
					}
					else
					{
						result = method.Call(PluginObject);
					}

					var resultTasks = new List<Task>();

					if (result is Task resultTask)
					{
						resultTasks.Add(resultTask);
					}

					await Task.WhenAll(resultTasks);
				});
		}
	}
}