using System;

namespace FineDataFlow.Engine.Abstractions
{
	/// <summary>
	/// Place on a method to indicate that it is an inbox that receives incoming rows one at a time.
	/// </summary>
	[AttributeUsage(AttributeTargets.Method)]
	public class RowStreamInboxAttribute : Attribute
	{
		/// <summary>
		/// The name of the inbox.
		/// </summary>
		public string Name { get; }

		/// <summary>
		/// Initializes a new instance of the <see cref="RowStreamInboxAttribute" /> class.
		/// </summary>
		public RowStreamInboxAttribute()
		{
		}

		/// <summary>
		/// Initializes a new instance of the <see cref="RowStreamInboxAttribute" /> class with
		/// the name of the inbox.
		/// </summary>
		/// <param name="name">The name of the inbox</param>
		public RowStreamInboxAttribute(string name)
		{
			Name = name;
		}
	}
}
