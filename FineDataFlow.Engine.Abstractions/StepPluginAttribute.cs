using System;

namespace FineDataFlow.Engine.Abstractions
{
	/// <summary>
	/// Place on a class to indicate that it is a step plugin.
	/// </summary>
	[AttributeUsage(AttributeTargets.Class)]
	public class StepPluginAttribute : Attribute
	{
	}
}
