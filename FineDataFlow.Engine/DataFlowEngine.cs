using FineDataFlow.Engine.Inboxes;
using FineDataFlow.Engine.Outboxes;
using Microsoft.Extensions.DependencyInjection;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;

namespace FineDataFlow.Engine
{
	public class DataFlowEngine : IDisposable
	{
		// public

		public string PluginsFolder { get; set; } = "./Plugins";
		public Repository Repository { get; set; }

		// other

		internal List<Type> StepTypes { get; } = new();

		// fields

		private bool _disposed;
		private bool _disposing;
		private bool _initialized;
		private bool _initializing;
		private readonly ServiceProvider _serviceProvider;
		private readonly List<TransformationPod> _transformationPods = new();
		private readonly CancellationTokenSource _runCancellationTokenSource;

		// constructors

		public DataFlowEngine()
		{
			var services = new ServiceCollection();
			
			services.AddSingleton<IServiceCollection>(services);

			services.AddTransient<StepPod>();
			services.AddTransient<SeedRowInbox>();
			services.AddTransient<AllRowsInbox>();
			services.AddTransient<RowStreamInbox>();
			services.AddTransient<ErrorRowOutbox>();
			services.AddTransient<SuccessRowOutbox>();
			services.AddTransient<TransformationPod>();
			services.AddTransient<StepCompleteOutbox>();
			services.AddTransient<CancellationTokenSource>();
			services.AddTransient<PluginAssemblyLoadContext>();
			
			_serviceProvider = services.BuildServiceProvider();

			_runCancellationTokenSource = _serviceProvider.GetService<CancellationTokenSource>();
		}

		// methods

		public void Initialize()
		{
			if (!Directory.Exists(PluginsFolder))
			{
				throw new InvalidOperationException($"{nameof(PluginsFolder)} '{PluginsFolder}' not found");
			}

			if (Repository == null)
			{
				throw new InvalidOperationException($"{nameof(Repository)} is required");
			}

			if (_disposing || _disposed)
			{
				throw new ObjectDisposedException(GetType().FullName);
			}

			if (_initializing)
			{
				throw new InvalidOperationException("Initialization in progress");
			}

			if (_initialized)
			{
				return;
			}

			_initializing = true;

			// load plugin types

			Directory
				.GetDirectories(PluginsFolder, "FineDataFlow.Engine.Plugin.Step.*", SearchOption.TopDirectoryOnly)
				.AsParallel()
				.ForAll(pluginFolder =>
				{
					var loadContext = _serviceProvider.GetRequiredService<PluginAssemblyLoadContext>();
					
					loadContext.PluginFolder = pluginFolder;
					
					var type = loadContext.GetPluginType<Step>();
					
					if (type == null)
					{
						return;
					}

					StepTypes.Add(type);
				});

			// create transformation pods

			Repository.Transformations
				.AsParallel()
				.ForAll(tr =>
				{
					var transformationPod = _serviceProvider.GetRequiredService<TransformationPod>();
					
					transformationPod.Transformation = tr;
					transformationPod.CancellationToken = _runCancellationTokenSource.Token;

					_transformationPods.Add(transformationPod);
				});

			// initialize transformation pods

			_transformationPods
				.AsParallel()
				.ForAll(tp => tp.Initialize());
			
			// TODO:validate engine

			// ...

			_initialized = true;
		}

		public Task RunAsync()
		{
			if (_disposing || _disposed)
			{
				throw new ObjectDisposedException(GetType().FullName);
			}

			if (!_initialized)
			{
				Initialize();
			}

			try
			{
				return Task.WhenAll(
					_transformationPods
						.AsParallel()
						.Select(tp => tp.RunAsync())
				);
			}
			catch (Exception exception)
			{
				throw new DataFlowException("Error while starting engine", exception);
			}
		}

		public void Run()
		{
			RunAsync()
				.GetAwaiter()
				.GetResult();
		}
		
		public void Stop()
		{
			_runCancellationTokenSource?.Cancel();
		}

		public void Dispose()
		{
			if (_disposing || _disposed)
			{
				return;
			}

			_disposing = true;
			
			GC.SuppressFinalize(this);
			_serviceProvider.Dispose();

			_disposed = true;
		}
	}
}
