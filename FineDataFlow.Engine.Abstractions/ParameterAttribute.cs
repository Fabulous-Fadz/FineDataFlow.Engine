using System;

namespace FineDataFlow.Engine.Abstractions
{
	/// <summary>
	/// Place on a property/field to indicate that it is a parameter.
	/// </summary>
	[AttributeUsage(AttributeTargets.Property | AttributeTargets.Field)]
	public class ParameterAttribute : Attribute
	{
		/// <summary>
		/// The name of the parameter.
		/// </summary>
		public string Name { get; }

		/// <summary>
		/// Initializes a new instance of the <see cref="ParameterAttribute" /> class.
		/// </summary>
		public ParameterAttribute()
		{
		}

		/// <summary>
		/// Initializes a new instance of the <see cref="ParameterAttribute" /> class with
		/// the name of the parameter.
		/// </summary>
		/// <param name="name">The name of the parameter</param>
		public ParameterAttribute(string name)
		{
			Name = name;
		}
	}
}
