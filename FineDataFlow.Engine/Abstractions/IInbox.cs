using System;
using System.Reflection;
using System.Threading.Tasks;
using System.Threading.Tasks.Dataflow;

namespace FineDataFlow.Engine.Abstractions
{
	internal interface IInbox
	{
		// properties

		public string Name { get; }
		public MemberInfo Member { get; set; }
		public IOutbox FromOutbox { get; set; }
		public Attribute Attribute { get; set; }
		public Type StepPluginType { get; set; }
		public object StepPluginObject { get; set; }
		public ActionBlock<Row> ActionBlock { get; set; }
		
		// methods

		public void Initialize();
		public Task ProcessRowAsync(Row row);
	}
}
