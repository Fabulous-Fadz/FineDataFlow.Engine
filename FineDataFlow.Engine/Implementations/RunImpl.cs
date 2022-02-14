using FineDataFlow.Engine.Abstractions;
using System.Threading;
using System.Threading.Tasks;

namespace FineDataFlow.Engine.Implementations
{
	internal class RunImpl : IRun
	{
		public Task Task { get; set; }
		public CancellationTokenSource CancellationTokenSource { get; set; }
		public CancellationToken CancellationToken => CancellationTokenSource?.Token ?? CancellationToken.None;
	}
}
