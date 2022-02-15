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
	internal class StepImpl : IStep, IDisposable
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
					inbox.Member = member;
					inbox.Attribute = attribute;
					inbox.StepPluginType = PluginType;
					inbox.StepPluginObject = PluginObject;
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
					outbox.Member = member;
					outbox.Attribute = attribute;
					outbox.StepPluginType = PluginType;
					outbox.StepPluginObject = PluginObject;
					outbox.Initialize();
					Outboxes.Add(outbox);
				}
			}

			_rowErrorOutbox = _serviceProvider.GetRequiredService<IRowErrorOutbox>();
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
							
							if (row == null)
							{
								inbox.ActionBlock.Complete();
							}
						}
						catch (Exception exception)
						{
							if (_rowErrorOutbox == null)
							{
								throw; // no handlers -> blow up
							}

							if (row != null)
							{
								row["Error"] = exception;
								row["ErrorStep"] = PluginObject;
								row["ErrorInbox"] = inbox;
							}

							_rowErrorOutbox.ActionBlock.Post(row);
						}
					},
					new ExecutionDataflowBlockOptions
					{
						EnsureOrdered = true, // completing blocks with null rows depends on this
						CancellationToken = _run.CancellationToken // force stopping the engine depends on this
					});
					
					inbox
						.ActionBlock
						.Completion
						.ContinueWith(x => _rowErrorOutbox?.AddRow(null), _run.CancellationToken);
					
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
						.ForAll(inbox => inbox.ActionBlock.Post(null));
				})
			);

			// return

			await Task
				.WhenAll(runnableTasks)
				.ContinueWith(t => _stepCompleteOutbox?.AddRow(null), _run.CancellationToken);
		}

		public void Dispose()
		{
			GC.SuppressFinalize(this);

			PluginType
				.GetMethods(BindingFlags.Instance | BindingFlags.Static | BindingFlags.Public | BindingFlags.NonPublic)
				.AsParallel()
				.ForAll(async method =>
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