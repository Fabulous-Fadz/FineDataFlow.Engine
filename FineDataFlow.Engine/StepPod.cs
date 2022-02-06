using FineDataFlow.Engine.Inboxes;
using FineDataFlow.Engine.Outboxes;
using Microsoft.Extensions.DependencyInjection;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Threading;
using System.Threading.Tasks;
using System.Threading.Tasks.Dataflow;

namespace FineDataFlow.Engine
{
	internal class StepPod
	{
		public Step Step { get; set; }
		public List<Inbox> Inboxes { get; } = new();
		public List<Outbox> Outboxes { get; } = new();
		public CancellationToken CancellationToken { get; set; }
		
		private bool _stepInitialized;
		private readonly IServiceProvider _serviceProvider;

		public StepPod
		(
			IServiceProvider serviceProvider
		)
		{
			_serviceProvider = serviceProvider ?? throw new ArgumentNullException(nameof(serviceProvider));
		}

		public void Initialize()
		{
			var stepProperties = Step.GetType().GetProperties(BindingFlags.Instance | BindingFlags.Public | BindingFlags.NonPublic);

			// create inboxes

			foreach (var inboxProperty in stepProperties.Where(x => typeof(Inbox).IsAssignableFrom(x.PropertyType)))
			{
				var inbox = (Inbox)_serviceProvider.GetRequiredService(inboxProperty.PropertyType);

				inbox.Step = Step;
				inbox.Name = inboxProperty.Name;
				inbox.CancellationToken = CancellationToken;

				inboxProperty.SetValue(Step, inbox);
				Inboxes.Add(inbox);
			}

			// create outboxes

			foreach (var outboxProperty in stepProperties.Where(x => typeof(Outbox).IsAssignableFrom(x.PropertyType)))
			{
				var outbox = (Outbox)_serviceProvider.GetRequiredService(outboxProperty.PropertyType);

				outbox.Step = Step;
				outbox.Name = outboxProperty.Name;
				outbox.CancellationToken = CancellationToken;

				outboxProperty.SetValue(Step, outbox);
				Outboxes.Add(outbox);
			}
		}

		public Task RunAsync()
		{
			// setup inboxes and outboxes

			var allRowsInboxesCompletionTask = default(Task);
			
			var stepCompleteOutboxes = Outboxes
				.AsParallel()
				.Where(x => x is StepCompleteOutbox)
				.ToList();

			var rowErrorOutboxes = Outboxes
				.AsParallel()
				.Where(x => x is ErrorRowOutbox)
				.ToList();

			Inboxes
				.AsParallel()
				.ForAll(inbox =>
				{
					inbox.ActionBlock = new ActionBlock<Row>(async row =>
					{
						try
						{
							if (inbox is not AllRowsInbox)
							{
								await allRowsInboxesCompletionTask;
							}
							
							inbox.AddRow(row);
							
							if (row == null)
							{
								inbox.ActionBlock.Complete();
							}
						}
						catch (Exception exception)
						{
							if (!rowErrorOutboxes.Any())
							{
								throw; // no handlers -> blow up
							}

							if (row != null)
							{
								row.Error = exception;
								row.ErrorStep = Step;
								row.ErrorInbox = inbox;
							}

							rowErrorOutboxes.AsParallel().ForAll(scb => scb.AddRow(row));
						}
					},
					new ExecutionDataflowBlockOptions
					{
						EnsureOrdered = true, // completing blocks with null rows depends on this
						CancellationToken = CancellationToken // force stopping the engine depends on this
					});
					
					inbox
						.ActionBlock
						.Completion
						.ContinueWith(x =>
						{
							rowErrorOutboxes
								.AsParallel()
								.ForAll(scb => scb.AddRow(null));

						}, CancellationToken);
					
					if (inbox.FromOutbox != null)
					{
						inbox.FromOutbox.ActionBlock = inbox.ActionBlock;
					}
				});

			allRowsInboxesCompletionTask = Task.WhenAll(
				Inboxes
					.AsParallel()
					.Where(x => x is AllRowsInbox)
					.Select(inbox => inbox.ActionBlock.Completion)
			);

			// initialize step

			if (!_stepInitialized)
			{
				Step.Initialize();
				_stepInitialized = true;
			}

			// collect runnable tasks

			var runnableTasks = new List<Task>();

			runnableTasks.AddRange(
				Inboxes
					.Select(x => x.ActionBlock?.Completion)
					.Where(x => x != null)
			);

			runnableTasks.AddRange(
				Outboxes
					.Where(x => !stepCompleteOutboxes.Contains(x))
					.Select(x => x.ActionBlock?.Completion)
					.Where(x => x != null)
			);

			runnableTasks.Add
			(
				Task.Run(() =>
				{
					Inboxes
						.AsParallel()
						.Where(x => x is SeedRowInbox)
						.ForAll(x => x.ActionBlock.Post(null));
				})
			);

			// return

			return Task
				.WhenAll(runnableTasks)
				.ContinueWith(x =>
				{
					stepCompleteOutboxes
						.AsParallel()
						.ForAll(scb => scb.AddRow(null));

				}, CancellationToken);
		}
	}
}