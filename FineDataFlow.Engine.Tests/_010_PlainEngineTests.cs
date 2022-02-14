using FineDataFlow.Engine.Abstractions.Models;
using FineDataFlow.Engine.Tests.TestUtilities;
using NUnit.Framework;
using Shouldly;
using System;
using System.Threading.Tasks;

namespace FineDataFlow.Engine.Tests
{
	public class _010_PlainEngineTests
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
		public void _010_NewEngine_SingleDispose()
		{
			_engine.Dispose();
		}

		[Test]
		public void _011_NewEngine_DoubleDispose()
		{
			_engine.Dispose();
			_engine.Dispose();
		}

		[Test]
		public void _020_NewEngine_Initialize_SingleDispose()
		{
			var exception = Should.Throw<InvalidOperationException>(() => _engine.InitializeAsync());

			exception.ShouldNotBeNull();
			exception.Message.ShouldBe($"{nameof(_engine.AppSource)} is required");

			_engine.Dispose();
		}

		[Test]
		public void _021_NewEngine_Initialize_DoubleDispose()
		{
			var exception = Should.Throw<InvalidOperationException>(() => _engine.InitializeAsync());

			exception.ShouldNotBeNull();
			exception.Message.ShouldBe($"{nameof(_engine.AppSource)} is required");

			_engine.Dispose();
			_engine.Dispose();
		}

		[Test]
		public async Task _030_NewEngine_AppSource_SingleInitialize()
		{
			var appSource = new TestAppSource();
			_engine.AppSource = appSource;
			await _engine.InitializeAsync();
		}

		[Test]
		public async Task _031_NewEngine_AppSource_DoubleInitialize()
		{
			var appSource = new TestAppSource();
			_engine.AppSource = appSource;
			await _engine.InitializeAsync();
			var exception = Should.Throw<InvalidOperationException>(() => _engine.InitializeAsync());

			exception.ShouldNotBeNull();
			exception.Message.ShouldBe($"Already initialized");
		}

		[Test]
		public async Task _040_NewEngine_AppSource_Initialize_Run()
		{
			var appSource = new TestAppSource();
			_engine.AppSource = appSource;
			await _engine.InitializeAsync();
			await _engine.RunAsync();
		}

		[Test]
		public async Task _050_NewEngine_AppSource_Run()
		{
			var appSource = new TestAppSource();
			_engine.AppSource = appSource;
			await _engine.RunAsync();
		}

		[Test]
		public async Task _060_NewEngine_AppSource_Run_Empty_Flow()
		{
			var appSource = new TestAppSource();

			appSource.App.Flows.Add(new Flow
			{
				Name = "Flow1",
				Enabled = true
			});

			_engine.AppSource = appSource;

			await _engine.RunAsync();
		}
	}
}
