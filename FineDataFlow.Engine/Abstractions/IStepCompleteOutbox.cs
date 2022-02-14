namespace FineDataFlow.Engine.Abstractions
{
	internal interface IStepCompleteOutbox : IOutbox
	{
		public const string OutboxName = "StepComplete";
	}
}
