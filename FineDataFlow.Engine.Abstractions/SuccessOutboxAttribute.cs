using System;

namespace FineDataFlow.Engine.Abstractions
{
	/// <summary>
	/// Place on a property/field to indicate that it is an outbox that sends successfully processed rows.
	/// </summary>
	[AttributeUsage(AttributeTargets.Property | AttributeTargets.Field)]
	public class SuccessOutboxAttribute : Attribute
	{
		/// <summary>
		/// The name of the outbox.
		/// </summary>
		public string Name { get; }

		/// <summary>
		/// Initializes a new instance of the <see cref="SuccessOutboxAttribute" /> class.
		/// </summary>
		public SuccessOutboxAttribute()
		{
		}

		/// <summary>
		/// Initializes a new instance of the <see cref="SuccessOutboxAttribute" /> class with
		/// the name of the outbox.
		/// </summary>
		/// <param name="name">The name of the outbox</param>
		public SuccessOutboxAttribute(string name)
		{
			Name = name;
		}
	}
}
