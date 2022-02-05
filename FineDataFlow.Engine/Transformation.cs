using System.Collections.Generic;

namespace FineDataFlow.Engine
{
	public class Transformation
	{
		public string Name { get; set; }
		public bool Enabled { get; set; }
		public List<Hop> Hops { get; } = new();
		public List<Step> Steps { get; } = new();
	}
}
