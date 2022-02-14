using System.Collections.Generic;

namespace FineDataFlow.Engine.Abstractions.Models
{
	/// <summary>
	/// Represents an <see cref="App"/> (a collection of <see cref="Flow"/>s and <see cref="Parameter"/>s)
	/// </summary>
	public class App
	{
		/// <summary>
		/// The name of the <see cref="App"/>
		/// </summary>
		public virtual string Name { get; set; }

		/// <summary>
		/// The description of the <see cref="App"/>
		/// </summary>
		public virtual string Description { get; set; }

		/// <summary>
		/// The version of the <see cref="App"/>
		/// </summary>
		public virtual string Version { get; set; }

		/// <summary>
		/// The list of <see cref="Flow"/>s in the <see cref="App"/>
		/// </summary>
		public virtual List<Flow> Flows { get; set; } = new();

		/// <summary>
		/// The list of <see cref="Parameter"/>s of the <see cref="App"/>
		/// </summary>
		public virtual List<Parameter> Parameters { get; set; } = new();
	}
}
