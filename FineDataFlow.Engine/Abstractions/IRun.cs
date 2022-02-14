﻿using System.Threading;
using System.Threading.Tasks;

namespace FineDataFlow.Engine.Abstractions
{
	internal interface IRun
	{
		public Task Task { get; set; }
		public CancellationTokenSource CancellationTokenSource { get; set; }
		public CancellationToken CancellationToken => CancellationTokenSource?.Token ?? CancellationToken.None;
	}
}