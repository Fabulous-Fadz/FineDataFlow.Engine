using FineDataFlow.Engine.Abstractions.Models;
using System;

namespace FineDataFlow.Engine.Abstractions
{
	internal interface IParameter : IDisposable
	{
		// properties

		public string Name { get; set; }
		public ParameterType Type { get; set; }
		public string RawValue { get; set; }
		public object Value { get; set; }

		// methods

		public void Initialize();
	}
}
