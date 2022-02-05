using FineDataFlow.Engine.Inboxes;
using FineDataFlow.Engine.Outboxes;
using NUnit.Framework;

namespace FineDataFlow.Engine.Tests
{
	public class UnitTests3
	{
		DataFlowEngine _engine;

		[SetUp]
		public void SetUp()
		{
			_engine = new DataFlowEngine();
		}

		[TearDown]
		public void TearDown()
		{
			_engine.Dispose();
		}

		[Test]
		public void Test1()
		{
			// arrange

			var repository1 = new Repository();
			repository1.Name = "Repository1";
			
			var transformation1 = new Transformation();
			transformation1.Name = "Transformation1";
			repository1.Transformations.Add(transformation1);

			transformation1.Steps.Add(new StartStep() { Name = "Start" });
			
			_engine.Repository = repository1;

			// act

			_engine.Run();
			
			// assert
		}

		class StartStep : Step
		{
			public SeedRowInbox SeedRowInbox { get; set; }
			public RowSuccessOutbox RowSuccessOutbox { get; set; }

			public override void Initialize()
			{
				SeedRowInbox.OnRow += SeedRowInbox_OnRow;
			}

			private void SeedRowInbox_OnRow(object sender, OnRowEventArgs e)
			{
				RowSuccessOutbox.AddRows(new(), new(), e.Row);
			}
		}
	}
}
