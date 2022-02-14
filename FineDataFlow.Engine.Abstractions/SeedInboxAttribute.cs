using System;

namespace FineDataFlow.Engine.Abstractions
{
	/// <summary>
	/// Place on a method to indicate that it is an inbox that receives exactly one row when engine starts.
	/// </summary>
	[AttributeUsage(AttributeTargets.Method)]
	public class SeedInboxAttribute : Attribute
	{
		/// <summary>
		/// The name of the inbox.
		/// </summary>
		public string Name { get; }

		/// <summary>
		/// Initializes a new instance of the <see cref="SeedInboxAttribute" /> class.
		/// </summary>
		public SeedInboxAttribute()
		{
		}

		/// <summary>
		/// Initializes a new instance of the <see cref="SeedInboxAttribute" /> class with
		/// the name of the inbox.
		/// </summary>
		/// <param name="name">The name of the inbox</param>
		public SeedInboxAttribute(string name)
		{
			Name = name;
		}
	}
}
