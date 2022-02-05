using System;
using System.Collections.Generic;

namespace FineDataFlow.Engine
{
	public class Row
	{
		public Exception Error { get; set; }
		public Step ErrorStep { get; set; }
		public Inbox ErrorInbox { get; set; }
		public Dictionary<string, object> Data { get; } = new();
	}
}
