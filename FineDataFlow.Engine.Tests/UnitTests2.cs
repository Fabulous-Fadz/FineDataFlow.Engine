//using FineDataFlow.Engine;
//using FineDataFlow.Engine.Inboxes;
//using FineDataFlow.Engine.Outboxes;
//using NUnit.Framework;

//namespace FineDataFlow.Engine.Tests
//{
//	public class UnitTests2
//	{
//		DataFlowEngine _engine;

//		[SetUp]
//		public void SetUp()
//		{
//			_engine = new DataFlowEngine();
//		}

//		[TearDown]
//		public void TearDown()
//		{
//			_engine.Dispose();
//		}

//		[Test]
//		public void Test1()
//		{
//			// arrange

//			var app1 = new AppImpl();
//			app1.Name = "App1";
			
//			var flow1 = new FlowImpl();
//			flow1.Name = "Flow1";
//			app1.Flows.Add(flow1);

//			flow1.Steps.Add(new StartStep() { Name = "Start" });
//			flow1.Steps.Add(new SuccessStep() { Name = "Success" });
//			flow1.Steps.Add(new ErrorStep() { Name = "Error" });
//			flow1.Steps.Add(new StepCompleteStep() { Name = "StepComplete" });

//			flow1.Hops.Add(new HopImpl()
//			{
//				FromStepName = "Start", FromOutboxName = nameof(StartStep.SuccessRowOutbox) ,
//				ToStepName = "Success", ToInboxName = nameof(SuccessStep.AllRowsInbox),
//			});

//			flow1.Hops.Add(new HopImpl()
//			{
//				FromStepName = "Start", FromOutboxName = nameof(StartStep.RowErrorOutbox),
//				ToStepName = "Error", ToInboxName = nameof(ErrorStep.RowStreamInbox),
//			});

//			flow1.Hops.Add(new HopImpl()
//			{
//				FromStepName = "Start", FromOutboxName = nameof(StartStep.StepCompleteOutbox),
//				ToStepName = "StepComplete", ToInboxName = nameof(StepCompleteStep.RowStreamInbox),
//			});

//			flow1.Hops.Add(new HopImpl()
//			{
//				FromStepName = "Error", FromOutboxName = nameof(ErrorStep.RowSuccessOutbox),
//				ToStepName = "Success", ToInboxName = nameof(SuccessStep.RowStreamInbox),
//			});

//			_engine.App = app1;

//			// act

//			_engine.Run();
			
//			// assert
//		}

//		class StartStep : StepImpl
//		{
//			public SeedRowInboxImpl SeedRowInbox { get; set; }
//			public SuccessRowsOutboxImpl SuccessRowOutbox { get; set; }

//			public override void Initialize()
//			{
//				SeedRowInbox.OnRow += SeedRowInbox_OnRow;
//			}

//			private void SeedRowInbox_OnRow(object sender, IRowReceivedEventArgs e)
//			{
//				SuccessRowOutbox.AddRow(new());
//				SuccessRowOutbox.AddRow(new());
//				SuccessRowOutbox.AddRow(null);
//			}
//		}

//		class SuccessStep : StepImpl
//		{
//			public AllRowsInboxImpl AllRowsInbox { get; set; }
//			public RowStreamInboxImpl RowStreamInbox { get; set; }

//			public SuccessRowsOutboxImpl RowSuccessOutbox { get; set; }

//			public override void Initialize()
//			{
//				RowStreamInbox.OnRow += RowStreamInbox_OnRow;
//			}

//			private void RowStreamInbox_OnRow(object sender, IRowReceivedEventArgs e)
//			{
//				AllRowsInbox.
//				RowSuccessOutbox.AddRow(e.Row);
//			}
//		}

//		class ErrorStep : StepImpl
//		{
//			public RowStreamInboxImpl RowStreamInbox { get; set; }
//			public SuccessRowsOutboxImpl RowSuccessOutbox { get; set; }

//			public override void Initialize()
//			{
//				RowStreamInbox.OnRow += RowStreamInbox_OnRow;
//			}

//			private void RowStreamInbox_OnRow(object sender, IRowReceivedEventArgs e)
//			{
//				RowSuccessOutbox.AddRow(e.Row);
//			}
//		}

//		class StepCompleteStep : StepImpl
//		{
//			public RowStreamInboxImpl RowStreamInbox { get; set; }
//			public SuccessRowsOutboxImpl RowSuccessOutbox { get; set; }

//			public override void Initialize()
//			{
//				RowStreamInbox.OnRow += RowStreamInbox_OnRow;
//			}

//			private void RowStreamInbox_OnRow(object sender, IRowReceivedEventArgs e)
//			{
//				RowSuccessOutbox.AddRow(e.Row);
//			}
//		}
//	}
//}
