using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Runtime.Loader;

//https://docs.microsoft.com/en-us/dotnet/core/tutorials/creating-app-with-plugin-support
//https://cjansson.se/blog/post/creating-isolated-plugins-dotnetcore

namespace FineDataFlow.Engine
{
	internal class PluginAssemblyLoadContext : AssemblyLoadContext, IDisposable
    {
        public string PluginFolder { get; set; }

        private Type _pluginType;
        private List<Assembly> _loadedAssemblies;
        private AssemblyDependencyResolver _resolver;

        public PluginAssemblyLoadContext() : base(true)
        {
        }

        protected override Assembly Load(AssemblyName assemblyName)
        {
            var assemblyPath = _resolver.ResolveAssemblyToPath(assemblyName);

            if (string.IsNullOrWhiteSpace(assemblyPath))
            {
                return null;
            }

            return LoadFromAssemblyPath(assemblyPath);
        }

        protected override IntPtr LoadUnmanagedDll(string unmanagedDllName)
        {
            var unmanagedDllPath = _resolver.ResolveUnmanagedDllToPath(unmanagedDllName);

            if (string.IsNullOrWhiteSpace(unmanagedDllPath))
            {
                return IntPtr.Zero;
            }

            return LoadUnmanagedDllFromPath(unmanagedDllPath);
        }

        public Type GetPluginType<T>()
        {
            if (string.IsNullOrWhiteSpace(PluginFolder))
            {
                throw new InvalidOperationException($"{nameof(PluginFolder)} is required");
            }

            if (!Directory.Exists(PluginFolder))
            {
                throw new InvalidOperationException($"Folder '{PluginFolder}' not found");
            }

            if (_resolver == null)
            {
                _resolver = new AssemblyDependencyResolver(PluginFolder);
            }

            if (_loadedAssemblies == null)
            {
                _loadedAssemblies = new List<Assembly>();

                var assemblyDllFile = Directory
                    .EnumerateFiles(PluginFolder, $"{Path.GetFileNameWithoutExtension(PluginFolder)}.dll", SearchOption.AllDirectories)
                    .SingleOrDefault();

                if (!string.IsNullOrWhiteSpace(assemblyDllFile))
                {
                    _loadedAssemblies.Add(LoadFromAssemblyPath(assemblyDllFile));
                }
            }

            if (_pluginType == null)
            {
                _pluginType = _loadedAssemblies
                    .SelectMany(a => a.GetTypes())
                    .Where(t => t.FullName == Path.GetFileNameWithoutExtension(PluginFolder))
                    .Where(t => !t.IsAbstract)
                    .Where(t => typeof(T).IsAssignableFrom(t))
                    .FirstOrDefault();
            }

            return _pluginType;
        }

		public void Dispose()
		{
            GC.SuppressFinalize(this);

            Unload();
		}
	}
}
