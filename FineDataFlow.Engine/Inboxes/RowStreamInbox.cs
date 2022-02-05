namespace FineDataFlow.Engine.Inboxes
{
	/// <summary>
	/// An inbox that processes each row as soon as it's available
	/// </summary>
	public class RowStreamInbox : Inbox
	{
		public event OnRowEventHandler OnRow;

		internal override void AddRow(Row row)
		{
			OnRow?.Invoke(this, new OnRowEventArgs
			{
				Row = row
			});
		}
	}
}
