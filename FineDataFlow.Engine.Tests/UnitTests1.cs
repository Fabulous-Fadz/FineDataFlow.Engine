//using FineDataFlow.Engine;
//using FineDataFlow.Engine.EventArgz;
//using FineDataFlow.Engine.Inboxes;
//using FineDataFlow.Engine.Outboxes;
//using NUnit.Framework;

//namespace FineDataFlow.Engine.Tests
//{
//	public class UnitTests1
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
//				FromStepName = "Start", FromOutboxName = nameof(StartStep.SuccessRowsOutbox),
//				ToStepName = "Success", ToInboxName = nameof(SuccessStep.RowStreamInbox),
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

//			_engine.App = app1;

//			// act

//			_engine.Run();
			
//			// assert
//		}

//		class StartStep : IStep
//		{
//			public AbstractSeedRowInbox SeedRowInbox { get; set; }
//			public AbstractSuccessRowsOutbox SuccessRowsOutbox { get; set; }

//			public void Initialize()
//			{
//				SeedRowInbox.RowReceived += SeedRowInbox_RowReceived;
//			}

//			private void SeedRowInbox_RowReceived(object sender, IRowReceivedEventArgs e)
//			{
//				SuccessRowsOutbox.AddRow(e.Row);
//			}
//		}

//		class SuccessStep : IStep
//		{
//			public IRowStreamInbox RowStreamInbox { get; set; }
//			public AbstractSuccessRowsOutbox SuccessRowsOutbox { get; set; }

//			public void Initialize()
//			{
//				RowStreamInbox.RowReceived += RowStreamInbox_RowReceived;
//			}

//			private void RowStreamInbox_RowReceived(object sender, IRowReceivedEventArgs e)
//			{
//				SuccessRowsOutbox.AddRow(e.Row);
//			}
//		}

//		class ErrorStep : IStep
//		{
//			public IRowStreamInbox RowStreamInbox { get; set; }
//			public AbstractSuccessRowsOutbox SuccessRowsOutbox { get; set; }

//			public void Initialize()
//			{
//				RowStreamInbox.RowReceived += RowStreamInbox_RowReceived;
//			}

//			private void RowStreamInbox_RowReceived(object sender, IRowReceivedEventArgs e)
//			{
//				SuccessRowsOutbox.AddRow(e.Row);
//			}
//		}

//		class StepCompleteStep : IStep
//		{
//			public IRowStreamInbox RowStreamInbox { get; set; }
//			public AbstractSuccessRowsOutbox SuccessRowsOutbox { get; set; }

//			public void Initialize()
//			{
//				RowStreamInbox.RowReceived += RowStreamInbox_RowReceived;
//			}

//			private void RowStreamInbox_RowReceived(object sender, IRowReceivedEventArgs e)
//			{
//				SuccessRowsOutbox.AddRow(e.Row);
//			}
//		}
//	}
//}
