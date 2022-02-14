using FineDataFlow.Engine.Abstractions;
using System;
using System.Reflection;
using System.Threading.Tasks.Dataflow;

namespace FineDataFlow.Engine.Internal
{
	internal interface IOutbox
	{
		// properties

		public string Name { get; set; }
		public object StepObject { get; set; }
		public IInbox ToInbox { get; set; }
		public MemberInfo Member { get; set; }
		public Attribute Attribute { get; set; }
		public ActionBlock<Row> ActionBlock { get; set; }

		// methods

		public void Initialize();
		public void AddRow(Row row);
	}
}
