using FineDataFlow.Engine.Abstractions;
using System;
using System.Reflection;
using System.Threading.Tasks.Dataflow;

namespace FineDataFlow.Engine.Implementations
{
	internal class RowErrorOutboxImpl : IRowErrorOutbox
	{
		// properties

		public IStep Step { get; set; }
		public string Name { get; set; }
		public IInbox ToInbox { get; set; }
		public Attribute Attribute { get; set; }
		public Type StepPluginType { get; set; }
		public object StepPluginObject { get; set; }
		public ActionBlock<Row> ActionBlock { get; set; }
		public MemberInfo StepPluginObjectMember { get; set; }

		// methods
		
		public void Initialize()
		{
			Name = IRowErrorOutbox.OutboxName;
		}

		public void AddRow(Row row)
		{
			ActionBlock?.Post(row);
		}

		public void Dispose()
		{
			// ...
		}
	}
}
