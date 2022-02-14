using FineDataFlow.Engine.Abstractions;
using FineDataFlow.Engine.Abstractions.Models;
using FineDataFlow.Engine.Internal;
using FineDataFlow.Engine.Internal.Impl;
using Microsoft.Extensions.DependencyInjection;
using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;

namespace FineDataFlow.Engine
{
	/// <summary>
	/// Represents an engine that can execute an <see cref="App"/>
	/// </summary>
	public class DataFlowEngine : IDisposable
	{
		// fields

		private IApp _app;
		private IRun _run;
		private bool _disposed;
		private bool _disposing;
		private bool _initialized;
		private bool _initializing;
		private readonly ServiceProvider _serviceProvider;

		// properties

		/// <summary>
		/// The source from which the app will be fetched
		/// </summary>
		public IAppSource AppSource { get; set; }

		/// <summary>
		/// The sources from which the plugins will be fetched
		/// </summary>
		public List<IPluginSource> PluginSources { get; set; } = new List<IPluginSource>();

		/// <summary>
		/// The folder into which plugins will be stored locally
		/// </summary>
		public string PluginsFolder { get; set; } = "./Plugins";

		// constructors

		/// <summary>
		/// Creates an instance of <see cref="DataFlowEngine"/>
		/// </summary>
		public DataFlowEngine()
		{
			var services = new ServiceCollection();

			// singletons

			services.AddSingleton<IRun, RunImpl>();
			services.AddSingleton<IServiceCollection>(services);

			// transients
			
			services.AddTransient<IApp, AppImpl>();
			services.AddTransient<IFlow, FlowImpl>();
			services.AddTransient<IStep, StepImpl>();
			services.AddTransient<CancellationTokenSource>();
			services.AddTransient<ISeedInbox, SeedInboxImpl>();
			services.AddTransient<IParameter, ParameterImpl>();
			services.AddTransient<IPluginLoader, PluginLoaderImpl>();
			services.AddTransient<IAllRowsInbox, AllRowsInboxImpl>();
			services.AddTransient<ISuccessOutbox, SuccessOutboxImpl>();
			services.AddTransient<IRowStreamInbox, RowStreamInboxImpl>();
			services.AddTransient<IRowErrorOutbox, RowErrorOutboxImpl>();
			services.AddTransient<IStepCompleteOutbox, StepCompleteOutboxImpl>();
			
			// service provider

			_serviceProvider = services.BuildServiceProvider();
		}

		// methods

		/// <summary>
		/// Initialize the engine
		/// </summary>
		/// <returns></returns>
		/// <exception cref="InvalidOperationException"></exception>
		/// <exception cref="ObjectDisposedException"></exception>
		/// <exception cref="ArgumentNullException"></exception>
		public async Task InitializeAsync()
		{
			// validate

			if (_disposing)
			{
				throw new InvalidOperationException("Disposing in progress");
			}

			if (_disposing)
			{
				throw new ObjectDisposedException(GetType().Name);
			}

			if (_initializing)
			{
				throw new InvalidOperationException("Initialization in progress");
			}

			if (_initialized)
			{
				throw new InvalidOperationException("Already initialized");
			}

			_initializing = true;
			
			if (AppSource == null)
			{
				throw new InvalidOperationException($"{nameof(AppSource)} is required");
			}

			if (PluginSources == null)
			{
				throw new InvalidOperationException($"{nameof(PluginSources)} is required");
			}

			if (string.IsNullOrWhiteSpace(PluginsFolder))
			{
				throw new InvalidOperationException($"{nameof(PluginsFolder)} is required");
			}

			// create and initialize app

			_app = _serviceProvider.GetRequiredService<IApp>();

			_app.AppSource = AppSource;
			_app.PluginSources = PluginSources;
			_app.PluginsFolder = PluginsFolder;
			
			await _app.InitializeAsync();

			// initialization state

			_initializing = false;
			_initialized = true;
		}

		/// <summary>
		/// Runs the engine
		/// </summary>
		/// <returns></returns>
		/// <exception cref="InvalidOperationException"></exception>
		/// <exception cref="ObjectDisposedException"></exception>
		public async Task RunAsync()
		{
			// validate

			if (_disposing)
			{
				throw new InvalidOperationException("Disposing in progress");
			}

			if (_disposing)
			{
				throw new ObjectDisposedException(GetType().Name);
			}

			if (_initializing)
			{
				throw new InvalidOperationException("Initialization in progress");
			}

			if (!_initialized)
			{
				await InitializeAsync();
			}

			// run

			_run = _serviceProvider.GetRequiredService<IRun>();
			_run.Task = _app.RunAsync();
			_run.CancellationTokenSource = _serviceProvider.GetRequiredService<CancellationTokenSource>();
			
			await _run.Task.ContinueWith(x => _run = null);
		}

		/// <summary>
		/// Stops the engine from running
		/// </summary>
		/// <returns></returns>
		/// <exception cref="InvalidOperationException"></exception>
		/// <exception cref="ObjectDisposedException"></exception>
		public async Task StopAsync()
		{
			// validate

			if (_run == null)
			{
				throw new InvalidOperationException("Is not running");
			}

			if (_disposing)
			{
				throw new InvalidOperationException("Disposing in progress");
			}

			if (_disposing)
			{
				throw new ObjectDisposedException(GetType().Name);
			}

			if (_initializing)
			{
				throw new InvalidOperationException("Initialization in progress");
			}

			if (!_initialized)
			{
				throw new InvalidOperationException("Not initialized");
			}

			// cancel

			_run?.CancellationTokenSource?.Cancel();

			// return

			await (_run?.Task ?? Task.CompletedTask);
		}

		/// <summary>
		/// Performs application-defined tasks associated with freeing, releasing, or resetting unmanaged resources.
		/// </summary>
		public void Dispose()
		{
			// validate

			if (_disposing || _disposed)
			{
				return;
			}

			// dispose

			_disposing = true;
			
			GC.SuppressFinalize(this);
			_serviceProvider.Dispose();

			_disposing = false;
			_disposed = true;
		}
	}
}
