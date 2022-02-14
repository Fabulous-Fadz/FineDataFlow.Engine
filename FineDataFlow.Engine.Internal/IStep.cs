using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace FineDataFlow.Engine.Internal
{
	internal interface IStep
	{
		// properties

		public string Name { get; }
		public bool Enabled { get; set; }
		public Type PluginType { get; set; }
		public string PluginId { get; set; }
		public List<IInbox> Inboxes { get; }
		public List<IOutbox> Outboxes { get; }
		
		// methods

		public void Initialize();
		public Task RunAsync();
	}
}
