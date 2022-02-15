using System;
using System.Reflection;
using System.Threading.Tasks.Dataflow;

namespace FineDataFlow.Engine.Abstractions
{
	internal interface IOutbox
	{
		// properties

		public string Name { get; set; }
		public IInbox ToInbox { get; set; }
		public MemberInfo Member { get; set; }
		public Attribute Attribute { get; set; }
		public Type StepPluginType { get; set; }
		public object StepPluginObject { get; set; }
		public ActionBlock<Row> ActionBlock { get; set; }

		// methods

		public void Initialize();
		public void AddRow(Row row);
	}
}
