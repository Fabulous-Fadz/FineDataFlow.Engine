using System;

namespace FineDataFlow.Engine.Abstractions
{
	/// <summary>
	/// Place on a method to indicate that it should be called to initialize an object.
	/// </summary>
	[AttributeUsage(AttributeTargets.Method)]
	public class InitializeAttribute : Attribute
	{
	}
}
