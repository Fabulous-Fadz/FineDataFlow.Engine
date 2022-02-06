using FineDataFlow.Engine.Outboxes;
using System.Threading;

namespace FineDataFlow.Engine
{
	public abstract class Step
	{
		public string Name { get; set; }
		public abstract void Initialize();
		//TODO:Max degree of paralellization
		internal ErrorRowOutbox RowErrorOutbox { get; set; }
		internal CancellationToken CancellationToken { get; set; }
		internal StepCompleteOutbox StepCompleteOutbox { get; set; }
	}
}
