using System;

namespace FineDataFlow.Engine.Abstractions
{
	/// <summary>
	/// Place on a method to indicate that it should be called to free managed/unmanaged resources.
	/// </summary>
	[AttributeUsage(AttributeTargets.Method)]
	public class DestroyAttribute : Attribute
	{
	}
}
