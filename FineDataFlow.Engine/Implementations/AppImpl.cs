using FineDataFlow.Engine.Abstractions;
using FineDataFlow.Engine.Abstractions.Models;
using Microsoft.Extensions.DependencyInjection;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;

namespace FineDataFlow.Engine.Implementations
{
	internal class AppImpl : IApp, IDisposable
	{
		private static readonly Type StepPluginAttributeType = typeof(StepPluginAttribute);

		// fields

		private List<IPluginLoader> _pluginLoaders = new();
		private readonly IServiceProvider _serviceProvider;

		// properties

		public string Name { get; set; }
		public string Version { get; set; }
		public string Description { get; set; }
		public IAppSource AppSource { get; set; }
		public string PluginsFolder { get; set; }
		public List<IFlow> Flows { get; set; } = new();
		public List<IParameter> Parameters { get; set; } = new();
		public List<IPluginSource> PluginSources { get; set; } = new();
		
		// constructors

		public AppImpl
		(
			IServiceProvider serviceProvider
		)
		{
			_serviceProvider = serviceProvider ?? throw new ArgumentNullException(nameof(serviceProvider));
		}
		
		// methods

		public async Task InitializeAsync()
		{
			// validate

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

			// get app

			var app = await AppSource.GetAppAsync();

			Name = app.Name;
			Version = app.Version;
			Description = app.Description;

			if (app?.Parameters != null)
			{
				Parameters = app
					.Parameters
					.AsParallel()
					.Select(UnpackParameter)
					.ToList();
			}

			if (app?.Flows != null)
			{
				Flows = app
					.Flows
					.AsParallel()
					.Select(UnpackFlow)
					.ToList();
			}

			// get plugins

			var pluginIdsWithoutValidSources = new List<string>();

			var pluginIdAndSourcePairs = Flows
				.AsParallel()
				.SelectMany(x => x.Steps)
				.Select(x => x.PluginId)
				.Distinct()
				.Select(pluginId =>
				{
					// get source

					var pluginSource = PluginSources
						.Where(x => x.HasPluginAsync(pluginId).GetAwaiter().GetResult())
						.FirstOrDefault();

					// skip if soure not found

					if (pluginSource == null)
					{
						pluginIdsWithoutValidSources.Add(pluginId);
						return null;
					}

					// return

					return new
					{
						PluginId = pluginId,
						PluginSource = pluginSource
					};
				})
				.Where(x => x != null)
				.ToList();

			if (pluginIdsWithoutValidSources.Any())
			{
				throw new InvalidOperationException($"The following plugins could not be found in the registered sources : {string.Join(", ", pluginIdsWithoutValidSources)}");
			}

			var pluginsWithEmptyFolders = new List<string>();

			var pluginIdAndFolderPairs = pluginIdAndSourcePairs
				.AsParallel()
				.Select(pair =>
				{
					// compose full folder path

					var pluginFolder = Path.Combine(PluginsFolder, pair.PluginId);

					// check the folder path

					if (Directory.Exists(pluginFolder))
					{
						if (Directory.EnumerateFileSystemEntries(pluginFolder).Any())
						{
							return null; // skip -> plugin already exists
						}
					}
					else
					{
						Directory.CreateDirectory(pluginFolder);
					}

					// get plugin and put it into specific folder

					pair.PluginSource.GetPluginAsync(pair.PluginId, pluginFolder).GetAwaiter().GetResult();

					// remove directory if there's nothing in it

					if (!Directory.EnumerateFileSystemEntries(pluginFolder).Any())
					{
						pluginsWithEmptyFolders.Add(pair.PluginId);
						Directory.Delete(pluginFolder);
					}

					// return

					return new
					{
						PluginId = pair.PluginId,
						PluginFolder = pluginFolder
					};
				})
				.Where(x => x != null)
				.ToList();

			if (pluginsWithEmptyFolders.Any())
			{
				throw new InvalidOperationException($"The following plugins were not fetched successfully : {string.Join(", ", pluginsWithEmptyFolders)}");
			}

			_pluginLoaders = pluginIdAndFolderPairs
				.AsParallel()
				.Select(pair =>
				{
					var iloader = _serviceProvider.GetRequiredService<IPluginLoader>();
					iloader.PluginId = pair.PluginId;
					iloader.PluginFolder = pair.PluginFolder;
					iloader.PluginAttributeType = StepPluginAttributeType;
					iloader.Initialize();
					return iloader;
				})
				.ToList();

			_pluginLoaders
				.AsParallel()
				.ForAll(loader =>
				{
					Flows
						.AsParallel()
						.SelectMany(flow => flow.Steps)
						.Where(step => step.PluginId == loader.PluginId)
						.ForAll(step => step.PluginType = loader.PluginType);
				});

			// initialize parameters

			Parameters
				.AsParallel()
				.ForAll(parameter => parameter.Initialize());

			// initialize flows

			Flows
				.AsParallel()
				.ForAll(flow => flow.Initialize());
		}

		public IParameter UnpackParameter(Parameter p)
		{
			var iparameter = _serviceProvider.GetRequiredService<IParameter>();

			iparameter.Name = p.Name;
			iparameter.Type = p.Type;
			iparameter.RawValue = p.Value;

			return iparameter;
		}

		public IFlow UnpackFlow(Flow f)
		{
			var iflow = _serviceProvider.GetRequiredService<IFlow>();

			iflow.Name = f.Name;
			iflow.Enabled = f.Enabled;

			if (f.Steps != null)
			{
				iflow.Steps = f.Steps
					.AsParallel()
					.Select(UnpackStep)
					.ToList();
			}

			if (f.Hops != null)
			{
				iflow.Hops = f.Hops
					.AsParallel()
					.Select(UnpackHop)
					.ToList();
			}

			return iflow;
		}

		public IStep UnpackStep(Step s)
		{
			var istep = _serviceProvider.GetRequiredService<IStep>();

			istep.Enabled = s.Enabled;
			istep.PluginId = s.PluginId;
			//istep.LogicType = s.FullyQualifiedClassName;//TODO:Resolve
			
			return istep;
		}
		
		public IHop UnpackHop(Hop h)
		{
			var ihop = _serviceProvider.GetRequiredService<IHop>();

			ihop.FromStepName = h.FromStepName;
			ihop.FromOutboxName = h.FromOutboxName;
			ihop.ToStepName = h.ToStepName;
			ihop.ToInboxName = h.ToInboxName;
			ihop.Enabled = h.Enabled;
			
			return ihop;
		}

		public Task RunAsync()
		{
			return Task.WhenAll
			(
				Flows
					.AsParallel()
					.Select(flow => flow.RunAsync())
			);
		}

		public void Dispose()
		{
			_pluginLoaders
				.AsParallel()
				.ForAll(pluginLoader => pluginLoader.Dispose());

			_pluginLoaders.Clear();
		}
	}
}
