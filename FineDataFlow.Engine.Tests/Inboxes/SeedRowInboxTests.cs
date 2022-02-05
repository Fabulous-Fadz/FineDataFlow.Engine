using FineDataFlow.Engine.Inboxes;
using NUnit.Framework;
using Shouldly;

namespace FineDataFlow.Engine.Tests.Inboxes
{
	internal class SeedRowInboxTests
	{
		SeedRowInbox inbox;

		[SetUp]
		public void SetUp()
		{
			inbox = new SeedRowInbox();
		}

		[Test]
		public void Must_Succeed()
		{
			// arrange

			var inputRow = new Row();
			object eventSender = null;
			OnRowEventArgs eventArgs = null;

			inbox.OnRow += (object sender, OnRowEventArgs e) =>
			{
				eventSender = sender;
				eventArgs = e;
			};

			// act

			inbox.AddRow(inputRow);

			// assert

			eventSender.ShouldNotBeNull();
			eventSender.ShouldBe(inbox);

			eventArgs.ShouldNotBeNull();
			eventArgs.Row.ShouldNotBeNull();
			eventArgs.Row.Error.ShouldBeNull();
			eventArgs.Row.Data.ShouldNotBeNull();
		}
	}
}
