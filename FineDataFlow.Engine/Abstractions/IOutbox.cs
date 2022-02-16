using System;
using System.Reflection;
using System.Threading.Tasks.Dataflow;

namespace FineDataFlow.Engine.Abstractions
{
	internal interface IOutbox : IDisposable
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

		public void Initialize();
		public void AddRow(Row row);
	}
}
