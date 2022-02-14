using System.Collections.Generic;

namespace FineDataFlow.Engine.Abstractions
{
	/// <summary>
	/// Represents a <see cref="Row"/> of data that flows through the engine.
	/// </summary>
	public class Row : Dictionary<string, object>
	{
		/// <summary>
		/// Initializes a new instance of the <see cref="Row" /> class.
		/// </summary>
		public Row()
		{
		}
	}
}
