using FineDataFlow.Engine.Abstractions;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;

namespace FineDataFlow.Engine.Tests.TestUtilities
{
	public class TestPluginSource : IPluginSource
	{
		private static readonly string ProjectFolder = null;
		private static readonly Assembly AbstractionsAssembly = typeof(StepPluginAttribute).Assembly;
		private static readonly Assembly MainAssembly = typeof(IStep).Assembly;

		static TestPluginSource()
		{
			ProjectFolder = Path.GetDirectoryName(Assembly.GetExecutingAssembly().Location);

			while (!Directory.GetFiles(ProjectFolder, "*.*", SearchOption.TopDirectoryOnly).Any(x =>
			{
				var extension = Path.GetExtension(x).Trim('.', ' ');

				if (extension.StartsWith("proj", StringComparison.OrdinalIgnoreCase))
				{
					return false;
				}

				return extension.EndsWith("proj", StringComparison.OrdinalIgnoreCase);
			}))
			{
				ProjectFolder = Path.GetDirectoryName(ProjectFolder);
			}
		}

		public string Name { get; set; } = "TestPluginSource1";

		private string GetPluginName(string pluginId)
		{
			return pluginId.Split('@', 2).First();
		}
		
		private async Task<string> GetSourceFileAsync(string pluginId)
		{
			await Task.CompletedTask;

			var pluginCsFile = $"{GetPluginName(pluginId)}.cs";

			return Directory
				.GetFiles(ProjectFolder, "*.*", SearchOption.AllDirectories)
				.Where(x => x.EndsWith(pluginCsFile, StringComparison.OrdinalIgnoreCase))
				.FirstOrDefault();
		}

		public async Task<bool> HasPluginAsync(string pluginId)
		{
			var sourceFile = await GetSourceFileAsync(pluginId);

			if (string.IsNullOrWhiteSpace(sourceFile))
			{
				return false;
			}

			return true;
		}

		public async Task GetPluginAsync(string pluginId, string pluginFolder)
		{
			var sourceFile = await GetSourceFileAsync(pluginId);

			if (string.IsNullOrWhiteSpace(sourceFile))
			{
				throw new Exception("Plugin not found");
			}

			var metadataReferences = new List<MetadataReference>();
			var sourceText = await File.ReadAllTextAsync(sourceFile);
			var assemblyFile = Path.Combine(pluginFolder, $"{GetPluginName(pluginId)}.dll");
			var syntaxTrees = new[] { CSharpSyntaxTree.ParseText(sourceText, new CSharpParseOptions(LanguageVersion.Latest), sourceFile, Encoding.Default) };

			foreach (var assembly in AppDomain.CurrentDomain.GetAssemblies())
			{
				try
				{
					metadataReferences.Add(MetadataReference.CreateFromFile(assembly.Location));
				}
				catch
				{
					// ignore
				}
			}

			var compilation = CSharpCompilation.Create(Path.GetFileName(assemblyFile), syntaxTrees, metadataReferences, new CSharpCompilationOptions(OutputKind.DynamicallyLinkedLibrary));
			var result = compilation.Emit(assemblyFile, $"{assemblyFile.Replace(".dll", "")}.pdb", $"{assemblyFile.Replace(".dll", "")}.xml");

			if (!result.Success)
			{
				var list = result.Diagnostics.Select(x => x.ToString()).ToList();
				throw new Exception($"Compilation failed\n{string.Join("\n", list)}");
			}

			// add these as a challenge to make sure the engine does not load them

			File.Copy(MainAssembly.Location, Path.Combine(pluginFolder, Path.GetFileName(MainAssembly.Location)));
			File.Copy(AbstractionsAssembly.Location, Path.Combine(pluginFolder, Path.GetFileName(AbstractionsAssembly.Location)));
		}
	}
}
