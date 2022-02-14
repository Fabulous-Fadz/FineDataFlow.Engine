//using FineDataFlow.Engine.Inboxes;
//using FineDataFlow.Engine.Outboxes;
//using NUnit.Framework;

//namespace FineDataFlow.Engine.Tests
//{
//	public class UnitTests3
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

//			private void SeedRowInbox_OnRow(object sender, OnRowEventArgs e)
//			{
//				SuccessRowOutbox.AddRow(new());
//				SuccessRowOutbox.AddRow(new());
//				SuccessRowOutbox.AddRow(null);
//			}
//		}
//	}
//}
