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
		private static readonly Dictionary<string, Assembly> SharedAssemblies = new();

		// fields

		private List<Assembly> _loadedAssemblies;
		private AssemblyDependencyResolver _resolver;

		// properties

		public string PluginId { get; set; }
		public Type PluginType { get; set; }
		public string PluginFolder { get; set; }
		public Type PluginAttributeType { get; set; }

		// constructors

		static PluginLoaderImpl()
		{
			foreach (var assembly in new[]
			{
				typeof(StepPluginAttribute).Assembly,
				typeof(IStep).Assembly
			})
			{
				SharedAssemblies[Path.GetFileName(assembly.Location)] = assembly;
			}
		}

		public PluginLoaderImpl() : base(true)
		{
		}

		// methods

		protected override Assembly Load(AssemblyName assemblyName)
		{
			var assemblyPath = _resolver.ResolveAssemblyToPath(assemblyName);

			if (string.IsNullOrWhiteSpace(assemblyPath))
			{
				return null;
			}

			var sharedAssembly = SharedAssemblies
				.Where(x => x.Key.Equals(Path.GetFileName(assemblyPath), StringComparison.OrdinalIgnoreCase))
				.Select(x => x.Value)
				.Where(x => x != null)
				.FirstOrDefault();

			if (sharedAssembly != null)
			{
				return sharedAssembly;
			}

			return LoadFromAssemblyPath(assemblyPath);
		}

		protected override IntPtr LoadUnmanagedDll(string dllName)
		{
			var assemblyPath = _resolver.ResolveUnmanagedDllToPath(dllName);

			if (string.IsNullOrWhiteSpace(assemblyPath))
			{
				return IntPtr.Zero;
			}

			var sharedAssembly = SharedAssemblies
				.Where(x => x.Key.Equals(Path.GetFileName(assemblyPath), StringComparison.OrdinalIgnoreCase))
				.Select(x => x.Value)
				.Where(x => x != null)
				.FirstOrDefault();

			if (sharedAssembly != null)
			{
				return IntPtr.Zero;
			}

			return LoadUnmanagedDllFromPath(assemblyPath);
		}

		public void Initialize()
		{
			if (string.IsNullOrWhiteSpace(PluginFolder))
			{
				throw new InvalidOperationException($"{nameof(PluginFolder)} is required");
			}

			if (!Directory.Exists(PluginFolder))
			{
				throw new InvalidOperationException($"Folder '{PluginFolder}' not found");
			}

			_resolver = new AssemblyDependencyResolver(PluginFolder);
			_loadedAssemblies = new List<Assembly>();

			Directory
				.EnumerateFiles(PluginFolder, "*.*", SearchOption.AllDirectories)
				.Where(x => x.EndsWith("dll", StringComparison.OrdinalIgnoreCase))
				.ToList()
				.ForEach(dllFile =>
				{
					try
					{
						_loadedAssemblies.Add(LoadFromAssemblyPath(dllFile));
					}
					catch
					{
						// ignore
					}
				});

			PluginType = _loadedAssemblies
				.SelectMany(a => a.GetTypes())
				.Where(t => t.FullName == Path.GetFileNameWithoutExtension(PluginFolder))
				.Where(t => !t.IsAbstract)
				.Where(t => t.IsDefined(PluginAttributeType))
				.FirstOrDefault();
		}

		public void Dispose()
		{
			GC.SuppressFinalize(this);

			_resolver = null;
			PluginType = null;
			SharedAssemblies?.Clear();
			_loadedAssemblies?.Clear();
			PluginAttributeType = null;

			Unload();
		}
	}
}
