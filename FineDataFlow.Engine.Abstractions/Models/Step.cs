using System.Collections.Generic;

namespace FineDataFlow.Engine.Abstractions.Models
{
	/// <summary>
	/// Represents a <see cref="Step"/> (a task that processes <see cref="Row"/>s)
	/// </summary>
	public class Step
	{
		/// <summary>
		/// The name of the <see cref="Step"/>
		/// </summary>
		public virtual string Name { get; set; }

		/// <summary>
		/// The identifier if the plugin for the <see cref="Step"/>
		/// </summary>
		public string PluginId { get; set; }

		/// <summary>
		/// The list of <see cref="Parameter"/>s of the <see cref="Step"/>
		/// </summary>
		public virtual List<Parameter> Parameters { get; set; } = new();

		/// <summary>
		/// Indicates wether or not the <see cref="Step"/> is enabled/active
		/// </summary>
		public bool Enabled { get; set; } = true;
	}
}