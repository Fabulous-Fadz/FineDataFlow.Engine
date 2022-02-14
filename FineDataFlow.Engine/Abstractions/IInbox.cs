using System;
using System.Reflection;
using System.Threading.Tasks.Dataflow;

namespace FineDataFlow.Engine.Abstractions
{
	internal interface IInbox
	{
		// properties

		public string Name { get; }
		public object StepObject { get; set; }
		public MemberInfo Member { get; set; }
		public IOutbox FromOutbox { get; set; }
		public Attribute Attribute { get; set; }
		public ActionBlock<Row> ActionBlock { get; set; }
		
		// methods

		public void Initialize();
		public void ProcessRow(Row row);
	}
}
