using FineDataFlow.Engine.Abstractions;
using System;
using System.Linq;
using System.Reflection;
using System.Threading.Tasks.Dataflow;

namespace FineDataFlow.Engine.Implementations
{
	internal class SuccessOutboxImpl : ISuccessOutbox
	{
		private static readonly Type PropertyOrFieldType = typeof(Action<Row>);

		// properties

		public IStep Step { get; set; }
		public string Name { get; set; }
		public IInbox ToInbox { get; set; }
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

			if (StepPluginObjectMember is not PropertyInfo && StepPluginObjectMember is not FieldInfo)
			{
				throw new InvalidOperationException($"{nameof(StepPluginObjectMember)} must be a property or field");
			}

			if (Attribute == null)
			{
				throw new InvalidOperationException($"{nameof(Attribute)} is required");
			}

			if (Attribute is not SuccessOutboxAttribute)
			{
				throw new InvalidOperationException($"{nameof(Attribute)} must be of type {nameof(SuccessOutboxAttribute)}");
			}

			if (!StepPluginType.GetMembers(BindingFlags.Instance | BindingFlags.Static | BindingFlags.Public | BindingFlags.NonPublic).Any(x => x == StepPluginObjectMember))
			{
				throw new InvalidOperationException($"{nameof(StepPluginObjectMember)} must be a member of {nameof(StepPluginObject)}'s type");
			}

			if (!StepPluginObjectMember.IsDefined(Attribute.GetType()))
			{
				throw new InvalidOperationException($"{nameof(StepPluginObjectMember)} must have attribute of type {nameof(SuccessOutboxAttribute)} defined");
			}

			var action = new Action<Row>(AddRow);
			var attribute = (SuccessOutboxAttribute)Attribute;
			
			Name = string.IsNullOrWhiteSpace(attribute.Name) ? StepPluginObjectMember.Name : attribute.Name;

			if (StepPluginObjectMember is PropertyInfo property)
			{
				if (property.PropertyType != PropertyOrFieldType || !property.CanWrite)
				{
					throw new InvalidOperationException($"{AttributeName<StepPluginAttribute>()} property {StepPluginType.FullName}.{property.Name} has an invalid signature for a {AttributeName<SuccessOutboxAttribute>()}");
				}

				property.SetValue(StepPluginObject, action);
			}
			else if (StepPluginObjectMember is FieldInfo field)
			{
				if (field.FieldType != PropertyOrFieldType)
				{
					throw new InvalidOperationException($"{AttributeName<StepPluginAttribute>()} field {StepPluginType.FullName}.{field.Name} has an invalid signature for a {AttributeName<SuccessOutboxAttribute>()}");
				}

				field.SetValue(StepPluginObject, action);
			}
		}

		public void AddRow(Row row)
		{
			ActionBlock?.Post(row);
		}

		public void Dispose()
		{
			// ...
		}
	}
}
