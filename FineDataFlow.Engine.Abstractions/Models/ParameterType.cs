namespace FineDataFlow.Engine.Abstractions.Models
{
	/// <summary>
	/// Type of a <see cref="Parameter"/>
	/// </summary>
	public enum ParameterType
	{
		/// <summary>
		/// String
		/// </summary>
		String,

		/// <summary>
		/// Integer (number without decimals)
		/// </summary>
		Integer,

		/// <summary>
		/// Boolean (true/false)
		/// </summary>
		Boolean,

		/// <summary>
		/// Decimal (number with decimals)
		/// </summary>
		Decimal,

		/// <summary>
		/// Date (without time)
		/// </summary>
		DateOnly,

		/// <summary>
		/// Time (without date)
		/// </summary>
		Time,

		/// <summary>
		/// Date (with time)
		/// </summary>
		DateAndTime,
	}
}