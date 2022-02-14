namespace FineDataFlow.Engine.Abstractions
{
	internal interface IRowErrorOutbox : IOutbox
	{
		public const string OutboxName = "RowError";
	}
}
