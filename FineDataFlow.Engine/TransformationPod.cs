using Microsoft.Extensions.DependencyInjection;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;

namespace FineDataFlow.Engine
{
	internal class TransformationPod
	{
		public Step Step { get; set; }
		public List<StepPod> StepPods { get; } = new();
		public Transformation Transformation { get; set; }
		public CancellationToken CancellationToken { get; set; }
		
		private readonly IServiceProvider _serviceProvider;

		public TransformationPod
		(
			IServiceProvider serviceProvider
		)
		{
			_serviceProvider = serviceProvider ?? throw new ArgumentNullException(nameof(serviceProvider));
		}

		public void Initialize()
		{
			// create step pods

			Transformation
				.Steps
				.AsParallel()
				.ForAll(step =>
				{
					var stepPod = _serviceProvider.GetRequiredService<StepPod>();

					stepPod.Step = step;
					stepPod.CancellationToken = CancellationToken;
					stepPod.Initialize();

					StepPods.Add(stepPod);
				});

			// connect step pods (inboxes -> outboxes)

			Transformation
				.Hops
				.AsParallel()
				.Where(x => x.Enabled)
				.Where(x => !string.IsNullOrWhiteSpace(x.FromStepName))
				.Where(x => !string.IsNullOrWhiteSpace(x.FromOutboxName))
				.Where(x => !string.IsNullOrWhiteSpace(x.ToStepName))
				.Where(x => !string.IsNullOrWhiteSpace(x.ToInboxName))
				.ForAll(hop =>
				{
					var fromOutbox = StepPods
						.AsParallel()
						.Where(x => x.Step.Name.Equals(hop.FromStepName, StringComparison.OrdinalIgnoreCase))
						.SelectMany(x => x.Outboxes)
						.Where(x => x.Name.Equals(hop.FromOutboxName, StringComparison.OrdinalIgnoreCase))
						.SingleOrDefault();

					if (fromOutbox == null)
					{
						return;
					}

					var toInbox = StepPods
						.AsParallel()
						.Where(x => x.Step.Name.Equals(hop.ToStepName, StringComparison.OrdinalIgnoreCase))
						.SelectMany(x => x.Inboxes)
						.Where(x => x.Name.Equals(hop.ToInboxName, StringComparison.OrdinalIgnoreCase))
						.SingleOrDefault();

					if (toInbox == null)
					{
						return;
					}

					fromOutbox.ToInbox = toInbox;
					toInbox.FromOutbox = fromOutbox;
				});
		}

		public Task RunAsync()
		{
			return Task.WhenAll(
				StepPods
					.AsParallel()
					.Select(sp => sp.RunAsync())
			);
		}
	}
}
