using System;

namespace FineDataFlow.Engine.Abstractions
{
	internal interface IPluginLoader : IDisposable
	{
		string PluginFolder { get; set; }
		string PluginId { get; set; }
		Type PluginAttributeType { get; set; }
		Type PluginType { get; set; }

		void Initialize();
	}
}
