using System;

namespace FineDataFlow.Engine
{
	public class DataFlowException : Exception
	{
		public DataFlowException(string message) : base(message) { }
		public DataFlowException(string message, Exception innerException) : base(message, innerException) {}
	}
}
