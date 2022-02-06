using System.Threading;
using System.Threading.Tasks.Dataflow;

namespace FineDataFlow.Engine
{
	/// <summary>
	/// A base class for all outbox types
	/// </summary>
	public abstract class Outbox
	{
		internal Step Step { get; set; }
		internal string Name { get; set; }
		internal Inbox ToInbox { get; set; }
		internal ActionBlock<Row> ActionBlock { get; set; }
		internal CancellationToken CancellationToken { get; set; }
		
		public void AddRow(Row row) => ActionBlock?.Post(row);
	}
}
