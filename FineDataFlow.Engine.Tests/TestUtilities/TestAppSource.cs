using FineDataFlow.Engine.Abstractions;
using FineDataFlow.Engine.Abstractions.Models;
using System.Threading.Tasks;

namespace FineDataFlow.Engine.Tests.TestUtilities
{
	internal class TestAppSource : IAppSource
	{
		public App App { get; set; } = new();

		public TestAppSource()
		{
			App.Name = "TestName";
			App.Version = "TestVersion";
			App.Description = "TestDescription";
		}

		public Task<App> GetAppAsync()
		{
			return Task.FromResult(App);
		}
	}
}
