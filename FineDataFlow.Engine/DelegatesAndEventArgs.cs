using System;
using System.Collections.Generic;

namespace FineDataFlow.Engine
{
	public delegate void OnRowEventHandler(object sender, OnRowEventArgs e);
	public delegate void OnRowsEventHandler(object sender, OnRowsEventArgs e);

	public class OnRowEventArgs : EventArgs
	{
		public Row Row { get; set; }
	}

	public class OnRowsEventArgs : EventArgs
	{
		public IEnumerable<Row> Rows { get; set; }
	}
}
