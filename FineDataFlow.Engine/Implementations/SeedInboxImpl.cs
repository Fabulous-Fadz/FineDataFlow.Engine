using FineDataFlow.Engine.Abstractions;
using System;
using System.Linq;
using System.Reflection;
using System.Threading.Tasks;
using System.Threading.Tasks.Dataflow;

namespace FineDataFlow.Engine.Implementations
{
	internal class SeedInboxImpl : ISeedInbox
	{
		private static readonly Type VoidType = typeof(void);
		private static readonly Type TaskType = typeof(Task);

		// fields

		private Func<Task> _processRowAsync;

		// properties

		public IStep Step { get; set; }
		public string Name { get; set; }
		public IOutbox FromOutbox { get; set; }
		public Attribute Attribute { get; set; }
		public Type StepPluginType { get; set; }
		public object StepPluginObject { get; set; }
		public ActionBlock<Row> ActionBlock { get; set; }
		public MemberInfo StepPluginObjectMember { get; set; }

		// methods

		private string AttributeName<T>() where T : Attribute
		{
			return $"{typeof(T).Name}$".Replace("Attribute$", null, StringComparison.OrdinalIgnoreCase);
		}

		public void Initialize()
		{
			if (StepPluginType == null)
			{
				throw new InvalidOperationException($"{nameof(StepPluginType)} is required");
			}

			if (StepPluginObjectMember == null)
			{
				throw new InvalidOperationException($"{nameof(StepPluginObjectMember)} is required");
			}

			if (StepPluginObjectMember is not MethodInfo)
			{
				throw new InvalidOperationException($"{nameof(StepPluginObjectMember)} must be a method");
			}

			if (Attribute == null)
			{
				throw new InvalidOperationException($"{nameof(Attribute)} is required");
			}

			if (Attribute is not SeedInboxAttribute)
			{
				throw new InvalidOperationException($"{nameof(Attribute)} must be of type {nameof(SeedInboxAttribute)}");
			}

			if (!StepPluginType.GetMethods(BindingFlags.Instance | BindingFlags.Static | BindingFlags.Public | BindingFlags.NonPublic).Any(x => x == StepPluginObjectMember))
			{
				throw new InvalidOperationException($"{nameof(StepPluginObjectMember)} must be a member of {nameof(StepPluginObject)}'s type");
			}

			if (!StepPluginObjectMember.IsDefined(Attribute.GetType()))
			{
				throw new InvalidOperationException($"{nameof(StepPluginObjectMember)} must have attribute of type {nameof(SeedInboxAttribute)} defined");
			}

			var method = (MethodInfo)StepPluginObjectMember;
			var attribute = (SeedInboxAttribute)Attribute;

			Name = string.IsNullOrWhiteSpace(attribute.Name) ? StepPluginObjectMember.Name : attribute.Name;

			if (method.ReturnType == VoidType)
			{
				var processRowAsync = (Action)method.CreateDelegate(typeof(Action), StepPluginObject);
				
				_processRowAsync = async () =>
				{
					processRowAsync();
					await Task.CompletedTask;
				};
			}
			else if (method.ReturnType == TaskType)
			{
				_processRowAsync = (Func<Task>)method.CreateDelegate(typeof(Func<Task>), StepPluginObject);
			}
			else
			{
				throw new InvalidOperationException($"{AttributeName<StepPluginAttribute>()} method {StepPluginType.FullName}.{method.Name} has an invalid signature for a {AttributeName<SeedInboxAttribute>()}");
			}
		}

		public async Task ProcessRowAsync(Row row)
		{
			await _processRowAsync();
		}

		public async Task DoneAsync()
		{
			await Task.CompletedTask;
		}

		public void Dispose()
		{
			// ...
		}
	}
}
