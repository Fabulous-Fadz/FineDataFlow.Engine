using System.Threading.Tasks;

namespace FineDataFlow.Engine.Abstractions
{
	/// <summary>
	/// The base interface for plugin sources
	/// </summary>
	public interface IPluginSource
	{
		/// <summary>
		/// The name of the plugin source
		/// </summary>
		public string Name { get; set; }

		/// <summary>
		///		Checks if the source has a particular plugin
		/// </summary>
		/// <param name="pluginId">
		///		The ID of the plugin (which includes the version) e.g. FineDataFlow.Engine.EmailSender@1.0.0
		/// </param>
		/// <returns>
		///		<see cref="Task{Boolean}"/>
		/// </returns>
		public Task<bool> HasPluginAsync(string pluginId);

		/// <summary>
		///		Gets a plugin from the source<br/>
		///		and saves its files to the local folder
		/// </summary>
		/// <param name="pluginId">
		///		The ID of the plugin (which includes the version) e.g. FineDataFlow.Engine.EmailSender@1.0.0
		/// </param>
		/// <param name="pluginFolder">
		///		The local folder into which the plugin's files should be saved e.g. /path/to/plugins/FineDataFlow.Engine.EmailSender@1.0.0
		///	</param>
		/// <returns>
		///		<see cref="Task"/>
		/// </returns>
		public Task GetPluginAsync(string pluginId, string pluginFolder);
	}
}
