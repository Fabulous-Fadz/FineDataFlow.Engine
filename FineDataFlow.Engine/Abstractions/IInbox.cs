using System;
using System.Reflection;
using System.Threading.Tasks;
using System.Threading.Tasks.Dataflow;

namespace FineDataFlow.Engine.Abstractions
{
	internal interface IInbox : IDisposable
	{
		// properties

		public string Name { get; }
		public IStep Step { get; set; }
		public IOutbox FromOutbox { get; set; }
		public Attribute Attribute { get; set; }
		public Type StepPluginType { get; set; }
		public object StepPluginObject { get; set; }
		public ActionBlock<Row> ActionBlock { get; set; }
		public MemberInfo StepPluginObjectMember { get; set; }
		
		// methods

		public void Initialize();
		public Task ProcessRowAsync(Row row);
		public Task DoneAsync();
	}
}
