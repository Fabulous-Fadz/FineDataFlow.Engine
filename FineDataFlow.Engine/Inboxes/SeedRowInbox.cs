namespace FineDataFlow.Engine.Inboxes
{
	/// <summary>
	/// An inbox that kicks of a flow by emitting one (null) row only
	/// </summary>
	public class SeedRowInbox : Inbox
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
