using FineDataFlow.Engine.Abstractions;
using FineDataFlow.Engine.Abstractions.Models;
using System;

namespace FineDataFlow.Engine.Implementations
{
	internal class ParameterImpl : IParameter
	{
		// properties

		public string Name { get; set; }
		public ParameterType Type { get; set; }
		public string RawValue { get; set; }
		public object Value { get; set; }

		// methods

		public void Initialize()
		{
			// validate

			if (string.IsNullOrWhiteSpace(Name))
			{
				throw new ArgumentNullException(nameof(Name));
			}

			if (string.IsNullOrWhiteSpace(RawValue))
			{
				throw new ArgumentNullException(nameof(RawValue));
			}

			// initialize

			//TODO:Evaluate if RawValue is expression

			Value = RawValue;
		}

		public void Dispose()
		{
			// ...
		}
	}
}
