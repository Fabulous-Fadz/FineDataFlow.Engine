using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace FineDataFlow.Engine.Abstractions
{
	internal interface IStep : IDisposable
	{
		// properties

		public string Name { get; }
		public bool Enabled { get; set; }
		public string PluginId { get; set; }
		public Type PluginType { get; set; }
		public List<IInbox> Inboxes { get; }
		public List<IOutbox> Outboxes { get; }
		public object PluginObject { get; set; }
		
		// methods

		public void Initialize();
		public Task RunAsync();
	}
}
