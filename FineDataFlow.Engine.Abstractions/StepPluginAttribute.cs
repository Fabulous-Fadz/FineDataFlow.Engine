using System;

namespace FineDataFlow.Engine.Abstractions
{
	/// <summary>
	/// Place on a class to indicate that it is a step plugin.
	/// </summary>
	[AttributeUsage(AttributeTargets.Class)]
	public class StepPluginAttribute : Attribute
	{
		/// <summary>
		/// The name of the step.
		/// </summary>
		public string Name { get; }

		/// <summary>
		/// Initializes a new instance of the <see cref="StepPluginAttribute" /> class.
		/// </summary>
		public StepPluginAttribute()
		{
		}

		/// <summary>
		/// Initializes a new instance of the <see cref="StepPluginAttribute" /> class with
		/// the name of the step.
		/// </summary>
		/// <param name="name">The name of the step</param>
		public StepPluginAttribute(string name)
		{
			Name = name;
		}
	}
}
