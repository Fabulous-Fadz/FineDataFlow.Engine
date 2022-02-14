using FineDataFlow.Engine.Abstractions.Models;

namespace FineDataFlow.Engine.Internal
{
	internal interface IParameter
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
