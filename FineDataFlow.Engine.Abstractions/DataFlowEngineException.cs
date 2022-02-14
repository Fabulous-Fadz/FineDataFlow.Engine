using System;

namespace FineDataFlow.Engine.Abstractions
{
	/// <summary>
	/// Represents <see cref="Exception"/>s that occur during engine usage
	/// </summary>
	public class DataFlowEngineException : Exception
	{
		/// <summary>
		/// Initializes a new instance of the <see cref="DataFlowEngineException"/> class with 
		/// a message that describes the exception
		/// </summary>
		/// <param name="message">A message that describes the exception</param>
		public DataFlowEngineException(string message) : base(message) {}

		/// <summary>
		/// Initializes a new instance of the <see cref="DataFlowEngineException"/> class with
		/// a message that describes the exception
		/// and a reference to the inner exception that is the cause of this exception
		/// </summary>
		/// <param name="message">A message that describes the exception</param>
		/// <param name="innerException">A reference to the inner exception that is the cause of this exception</param>
		public DataFlowEngineException(string message, Exception innerException) : base(message, innerException) {}
	}
}
