using System;

namespace FineDataFlow.Engine.Abstractions
{
	/// <summary>
	/// Place on a method to indicate that it is an inbox that receives all incoming rows at once.
	/// </summary>
	[AttributeUsage(AttributeTargets.Method)]
	public class AllRowsInboxAttribute : Attribute
	{
		/// <summary>
		/// The name of the inbox.
		/// </summary>
		public string Name { get; }

		/// <summary>
		/// Initializes a new instance of the <see cref="AllRowsInboxAttribute" /> class.
		/// </summary>
		public AllRowsInboxAttribute()
		{
		}

		/// <summary>
		/// Initializes a new instance of the <see cref="AllRowsInboxAttribute" /> class with
		/// the name of the inbox.
		/// </summary>
		/// <param name="name">The name of the inbox</param>
		public AllRowsInboxAttribute(string name)
		{
			Name = name;
		}
	}
}
