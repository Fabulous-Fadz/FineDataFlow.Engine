namespace FineDataFlow.Engine
{
	public class Hop
	{
		public bool Enabled { get; set; } = true;
		public string FromStepName { get; set; }
		public string FromOutboxName { get; set; }
		public string ToStepName { get; set; }
		public string ToInboxName { get; set; }
	}
}
