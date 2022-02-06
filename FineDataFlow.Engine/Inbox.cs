using System.Threading;
using System.Threading.Tasks.Dataflow;

namespace FineDataFlow.Engine
{
	/// <summary>
	/// A base class for all inbox types
	/// </summary>
	public abstract class Inbox
	{
		internal Step Step { get; set; }
		internal string Name { get; set; }
		internal abstract void AddRow(Row row);
		internal Outbox FromOutbox { get; set; }
		internal ActionBlock<Row> ActionBlock { get; set; }
		internal CancellationToken CancellationToken { get; set; }
	}
}
