using System.Collections.Generic;

namespace FineDataFlow.Engine.Inboxes
{
	/// <summary>
	/// An inbox that holds a rows and outputs all of them at once
	/// </summary>
	public class AllRowsInbox : Inbox
	{
		private readonly List<Row> _rows = new();

		public IEnumerable<Row> Rows => _rows;

		public event OnRowsEventHandler OnRows;

		internal override void AddRow(Row row)
		{
			_rows.Add(row);

			if (row == null)
			{
				OnRows?.Invoke(this, new OnRowsEventArgs
				{
					Rows = _rows
				});
			}
		}
	}
}
