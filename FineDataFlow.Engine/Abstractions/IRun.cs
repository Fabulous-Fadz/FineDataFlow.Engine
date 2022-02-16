using System;
using System.Threading;
using System.Threading.Tasks;

namespace FineDataFlow.Engine.Abstractions
{
	internal interface IRun : IDisposable
	{
		public Task Task { get; set; }
		public CancellationTokenSource CancellationTokenSource { get; set; }
		public CancellationToken CancellationToken => CancellationTokenSource?.Token ?? CancellationToken.None;
	}
}
