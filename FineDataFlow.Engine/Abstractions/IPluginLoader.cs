using System;

namespace FineDataFlow.Engine.Abstractions
{
	internal interface IPluginLoader : IDisposable
	{
		// properties

		public string PluginFolder { get; set; }
		public string PluginId { get; set; }
		public Type PluginAttributeType { get; set; }
		public Type PluginType { get; set; }

		// methods

		public void Initialize();
	}
}
