namespace FineDataFlow.Engine.Abstractions.Models
{
	/// <summary>
	/// Represents a <see cref="Parameter"/>
	/// </summary>
	public class Parameter
	{
		/// <summary>
		/// The name of the <see cref="Parameter"/>
		/// </summary>
		public virtual string Name { get; set; }

		/// <summary>
		/// The type of the <see cref="Parameter"/>
		/// </summary>
		public virtual ParameterType Type { get; set; }

		/// <summary>
		/// The value of the <see cref="Parameter"/>
		/// </summary>
		public virtual string Value { get; set; }
	}
}
