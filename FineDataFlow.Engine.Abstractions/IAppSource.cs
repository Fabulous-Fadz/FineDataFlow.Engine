using FineDataFlow.Engine.Abstractions.Models;
using System.Threading.Tasks;

namespace FineDataFlow.Engine.Abstractions
{
	/// <summary>
	/// The base interface for <see cref="App">App</see> sources
	/// </summary>
	public interface IAppSource
	{
		/// <summary>
		///		Gets an <see cref="App">App</see> from source
		/// </summary>
		/// <returns>
		///		<see cref="Task{App}"/> of <see cref="App"/>
		///	</returns>
		public Task<App> GetAppAsync();
	}
}
