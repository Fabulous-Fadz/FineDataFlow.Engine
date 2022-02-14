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
		///	The location string of the plugin source
		///	<para>Examples:</para>
		///	<list type="bullet">
		///		<item>nuget-server|https://api.nuget.org/v3/index.json?username=uuu&amp;password=ppp</item>
		///		<item>ftp-folder|ftp://user_name:password@hostname/plugins</item>
		///		<item>local-folder|c:/plugins</item>
		///	</list>
		/// </summary>
		public string LocationString { get; set; }

		/// <summary>
		///		Checks if the <paramref name="locationString"/> is supported
		/// </summary>
		/// <param name="locationString">
		///		The location string specifying where to check.
		///		<para>Examples:</para>
		///		<list type="bullet">
		///			<item>nuget-server|https://api.nuget.org/v3/index.json?username=uuu&amp;password=ppp</item>
		///			<item>ftp-folder|ftp://user_name:password@hostname/plugins</item>
		///			<item>local-folder|c:/plugins</item>
		///			<item>e.t.c</item>
		///		</list>
		/// </param>
		/// <returns>
		///		True if the location string is supported
		/// </returns>
		public bool SupportsLocationString(string locationString);

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
