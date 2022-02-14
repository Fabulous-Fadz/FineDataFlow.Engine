using FineDataFlow.Engine.Abstractions;
using System;
using System.Reflection;
using System.Threading.Tasks.Dataflow;

namespace FineDataFlow.Engine.Implementations
{
	internal class RowErrorOutboxImpl : IRowErrorOutbox
	{
		// properties

		public string Name { get; set; }
		public IInbox ToInbox { get; set; }
		public object StepObject { get; set; }
		public MemberInfo Member { get; set; }
		public Attribute Attribute { get; set; }
		public ActionBlock<Row> ActionBlock { get; set; }

		// methods
		
		public void Initialize()
		{
			if (StepObject == null)
			{
				throw new InvalidOperationException($"{nameof(StepObject)} is required");
			}

			Name = IRowErrorOutbox.OutboxName;
		}

		public void AddRow(Row row)
		{
			ActionBlock?.Post(row);
		}
	}
}
