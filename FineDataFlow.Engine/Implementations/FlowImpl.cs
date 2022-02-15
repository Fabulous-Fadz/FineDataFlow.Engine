using FineDataFlow.Engine.Abstractions;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace FineDataFlow.Engine.Implementations
{
	internal class FlowImpl : IFlow
	{
		// properties

		public string Name { get; set; }
		public bool Enabled { get; set; }
		public List<IHop> Hops { get; set; } = new();
		public List<IStep> Steps { get; set; } = new();

		// methods

		public void Initialize()
		{
			// connect steps (inboxes -> outboxes)

			Hops
				.AsParallel()
				.Where(hop => hop.Enabled)
				.Where(hop => !string.IsNullOrWhiteSpace(hop.FromStepName))
				.Where(hop => !string.IsNullOrWhiteSpace(hop.FromOutboxName))
				.Where(hop => !string.IsNullOrWhiteSpace(hop.ToStepName))
				.Where(hop => !string.IsNullOrWhiteSpace(hop.ToInboxName))
				.ForAll(hop =>
				{
					var fromOutbox = Steps
						.AsParallel()
						.Where(pod => pod.Name.Equals(hop.FromStepName, StringComparison.OrdinalIgnoreCase))
						.SelectMany(pod => pod.Outboxes)
						.Where(oubox => oubox.Name.Equals(hop.FromOutboxName, StringComparison.OrdinalIgnoreCase))
						.SingleOrDefault();

					if (fromOutbox == null)
					{
						return;
					}

					var toInbox = Steps
						.AsParallel()
						.Where(pod => pod.Name.Equals(hop.ToStepName, StringComparison.OrdinalIgnoreCase))
						.SelectMany(pod => pod.Inboxes)
						.Where(inbox => inbox.Name.Equals(hop.ToInboxName, StringComparison.OrdinalIgnoreCase))
						.SingleOrDefault();

					if (toInbox == null)
					{
						return;
					}

					fromOutbox.ToInbox = toInbox;
					toInbox.FromOutbox = fromOutbox;
				});

			// initialize steps

			Steps
				.AsParallel()
				.ForAll(step => step.Initialize());
		}

		public Task RunAsync()
		{
			return Task.WhenAll
			(
				Steps
					.AsParallel()
					.Select(s => s.RunAsync())
			);
		}
	}
}
