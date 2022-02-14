namespace FineDataFlow.Engine.Internal
{
	internal interface IStepCompleteOutbox : IOutbox
	{
		public const string OutboxName = "StepComplete";
	}
}
