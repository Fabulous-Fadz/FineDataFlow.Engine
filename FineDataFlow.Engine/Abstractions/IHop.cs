namespace FineDataFlow.Engine.Abstractions
{
	internal interface IHop 
	{
		public string FromStepName { get; set; }
		public string FromOutboxName { get; set; }
		public string ToStepName { get; set; }
		public string ToInboxName { get; set; }
		public bool Enabled { get; set; }
	}
}
