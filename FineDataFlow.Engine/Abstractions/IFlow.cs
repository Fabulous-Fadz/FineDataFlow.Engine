using System.Collections.Generic;
using System.Threading.Tasks;

namespace FineDataFlow.Engine.Abstractions
{
	internal interface IFlow
	{
		// properties

		public string Name { get; set; }
		public bool Enabled { get; set; }
		public List<IHop> Hops { get; set; }
		public List<IStep> Steps { get; set; }
		

		// methods

		public void Initialize();
		public Task RunAsync();
	}
}
