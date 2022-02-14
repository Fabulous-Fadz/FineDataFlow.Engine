using System.Collections.Generic;
using System.Threading.Tasks;

namespace FineDataFlow.Engine.Abstractions
{
	internal interface IApp
	{
		// properties

		public string Name { get; set; }
		public string Version { get; set; }
		public List<IFlow> Flows { get; set; }
		public string Description { get; set; }
		public IAppSource AppSource { get; set; }
		public string PluginsFolder { get; set; }
		public List<IParameter> Parameters { get; set; }
		public List<IPluginSource> PluginSources { get; set; }

		// methods

		public Task InitializeAsync();
		public Task RunAsync();
	}
}
