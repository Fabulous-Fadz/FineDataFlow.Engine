using FineDataFlow.Engine.Inboxes;
using FineDataFlow.Engine.Outboxes;
using NUnit.Framework;

namespace FineDataFlow.Engine.Tests
{
	public class UnitTests1
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
			transformation1.Steps.Add(new SuccessStep() { Name = "Success" });
			transformation1.Steps.Add(new ErrorStep() { Name = "Error" });
			transformation1.Steps.Add(new StepCompleteStep() { Name = "StepComplete" });

			transformation1.Hops.Add(new Hop()
			{
				FromStepName = "Start", FromOutboxName = nameof(StartStep.SuccessRowOutbox),
				ToStepName = "Success", ToInboxName = nameof(SuccessStep.RowStreamInbox),
			});

			transformation1.Hops.Add(new Hop()
			{
				FromStepName = "Start", FromOutboxName = nameof(StartStep.RowErrorOutbox),
				ToStepName = "Error", ToInboxName = nameof(ErrorStep.RowStreamInbox),
			});

			transformation1.Hops.Add(new Hop()
			{
				FromStepName = "Start", FromOutboxName = nameof(StartStep.StepCompleteOutbox),
				ToStepName = "StepComplete", ToInboxName = nameof(StepCompleteStep.RowStreamInbox),
			});

			_engine.Repository = repository1;

			// act

			_engine.Run();
			
			// assert
		}

		class StartStep : Step
		{
			public SeedRowInbox SeedRowInbox { get; set; }
			public SuccessRowOutbox SuccessRowOutbox { get; set; }

			public override void Initialize()
			{
				SeedRowInbox.OnRow += SeedRowInbox_OnRow;
			}

			private void SeedRowInbox_OnRow(object sender, OnRowEventArgs e)
			{
				SuccessRowOutbox.AddRow(new());
				SuccessRowOutbox.AddRow(new());
				SuccessRowOutbox.AddRow(null);
			}
		}

		class SuccessStep : Step
		{
			public RowStreamInbox RowStreamInbox { get; set; }
			public SuccessRowOutbox RowSuccessOutbox { get; set; }

			public override void Initialize()
			{
				RowStreamInbox.OnRow += RowStreamInbox_OnRow;
			}

			private void RowStreamInbox_OnRow(object sender, OnRowEventArgs e)
			{
				RowSuccessOutbox.AddRow(e.Row);
			}
		}

		class ErrorStep : Step
		{
			public RowStreamInbox RowStreamInbox { get; set; }
			public SuccessRowOutbox RowSuccessOutbox { get; set; }

			public override void Initialize()
			{
				RowStreamInbox.OnRow += RowStreamInbox_OnRow;
			}

			private void RowStreamInbox_OnRow(object sender, OnRowEventArgs e)
			{
				RowSuccessOutbox.AddRow(e.Row);
			}
		}

		class StepCompleteStep : Step
		{
			public RowStreamInbox RowStreamInbox { get; set; }
			public SuccessRowOutbox RowSuccessOutbox { get; set; }

			public override void Initialize()
			{
				RowStreamInbox.OnRow += RowStreamInbox_OnRow;
			}

			private void RowStreamInbox_OnRow(object sender, OnRowEventArgs e)
			{
				RowSuccessOutbox.AddRow(e.Row);
			}
		}
	}
}
