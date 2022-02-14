using System.Collections.Generic;

namespace FineDataFlow.Engine.Abstractions.Models
{
	/// <summary>
	/// Represents a <see cref="Flow"/> (a collection of <see cref="Step"/>s and <see cref="Hop"/>s)
	/// </summary>
	public class Flow
	{
		/// <summary>
		/// The name of the <see cref="Flow"/>
		/// </summary>
		public virtual string Name { get; set; }

		/// <summary>
		/// Wether or not a <see cref="Flow"/> is enabled/active
		/// </summary>
		public bool Enabled { get; set; } = true;

		/// <summary>
		/// The list of <see cref="Hop"/>s in the <see cref="Flow"/> (links between <see cref="Step"/>s)
		/// </summary>
		public virtual List<Hop> Hops { get; set; } = new();

		/// <summary>
		/// The list of <see cref="Step"/>s in the <see cref="Flow"/>
		/// </summary>
		public virtual List<Step> Steps { get; set; } = new();
	}
}
