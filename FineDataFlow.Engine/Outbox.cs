using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks.Dataflow;
using MoreLinq;

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
		internal void AddRow(Row row) => ActionBlock?.Post(row);
		internal CancellationToken CancellationToken { get; set; }
		internal void AddRows(IEnumerable<Row> rows) => rows?.ForEach(AddRow);
		internal void AddRows(params Row[] rows) => AddRows(rows?.AsEnumerable());
	}
}
