using FineDataFlow.Engine.Abstractions;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Runtime.Loader;

//https://docs.microsoft.com/en-us/dotnet/core/tutorials/creating-app-with-plugin-support
//https://cjansson.se/blog/post/creating-isolated-plugins-dotnetcore

namespace FineDataFlow.Engine.Implementations
{
	internal class PluginLoaderImpl : AssemblyLoadContext, IPluginLoader
	{
		// fields

		private readonly List<Assembly> _loadedAssemblies = new();
		private static readonly Dictionary<string, Assembly> _sharedAssemblies = new();
		
		// properties

		public string PluginId { get; set; }
		public Type PluginType { get; set; }
		public string PluginFolder { get; set; }
		public Type PluginAttributeType { get; set; }

		// constructors

		static PluginLoaderImpl()
		{
			_sharedAssemblies = new Dictionary<string, Assembly>();
			
			foreach (var assembly in new[]
			{
				typeof(IStep).Assembly,
				typeof(StepPluginAttribute).Assembly,
			})
			{
				_sharedAssemblies[Path.GetFileName(assembly.Location)] = assembly;
			}
		}

		public PluginLoaderImpl() : base(true)
		{
			_loadedAssemblies = new List<Assembly>();
		}

		// methods
		
		protected override Assembly Load(AssemblyName assemblyName)
		{
			var filename = $"{assemblyName.Name}.dll";

			if (_sharedAssemblies.ContainsKey(filename))
			{
				return _sharedAssemblies[filename];
			}

			return Assembly.Load(assemblyName);
		}

		public void Initialize()
		{
			foreach (var dllFile in Directory.EnumerateFiles(PluginFolder, "*.*").Where(x => x.EndsWith(".dll", StringComparison.OrdinalIgnoreCase)))
			{
				if (_sharedAssemblies.ContainsKey(Path.GetFileName(dllFile)))
				{
					continue;
				}

				_loadedAssemblies.Add(LoadFromAssemblyPath(dllFile));
			}

			PluginType = _loadedAssemblies
				.SelectMany(a => a.GetTypes())
				.Where(t => t.IsClass)
				.Where(t => !t.IsAbstract || (t.IsAbstract && t.IsSealed))
				.Where(t => t.IsDefined(PluginAttributeType))
				.FirstOrDefault();
		}

		public void Dispose()
		{
			GC.SuppressFinalize(this);

			PluginType = null;
			PluginAttributeType = null;
			_loadedAssemblies?.Clear();

			//TODO:This MUST work!!!!!!!!!!!!!!!
			//Unload();
		}
	}
}
