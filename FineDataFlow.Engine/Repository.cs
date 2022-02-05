using System;
using System.Collections.Generic;
using System.IO;

namespace FineDataFlow.Engine
{
	public class Repository
	{
		public string Name { get; set; }
		public List<Transformation> Transformations { get; } = new();

		public Repository()
		{

		}

		public Repository(string filePath) : this(File.OpenRead(filePath))
		{
		}

		public Repository(Stream stream)
		{
		}

		public void Save(string filePath)
		{
			if (string.IsNullOrWhiteSpace(filePath))
			{
				throw new ArgumentNullException(filePath);
			}

			using var stream = File.OpenWrite(nameof(filePath));
			Save(stream);
		}

		public void Save(Stream stream)
		{
			if (stream == null)
			{
				throw new ArgumentNullException(nameof(stream));
			}
		}

		//private class DB : DbContext
		//{
		//	public DbSet<Transformation> Transformations { get; set; }
		//	public DbSet<Step> Steps { get; set; }
		//	public DbSet<Hop> Hops { get; set; }
		//}
	}
}
