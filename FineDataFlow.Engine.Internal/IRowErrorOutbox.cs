namespace FineDataFlow.Engine.Internal
{
	internal interface IRowErrorOutbox : IOutbox
	{
		public const string OutboxName = "RowError";
	}
}
