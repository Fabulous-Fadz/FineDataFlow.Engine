using FineDataFlow.Engine.Abstractions;
using System;
using System.Linq;
using System.Reflection;
using System.Threading.Tasks.Dataflow;

namespace FineDataFlow.Engine.Implementations
{
	internal class SuccessOutboxImpl : ISuccessOutbox
	{
		// properties

		public string Name { get; set; }
		public IInbox ToInbox { get; set; }
		public object StepObject { get; set; }
		public MemberInfo Member { get; set; }
		public Attribute Attribute { get; set; }
		public ActionBlock<Row> ActionBlock { get; set; }

		// methods

		public void Initialize()
		{
			if (StepObject == null)
			{
				throw new InvalidOperationException($"{nameof(StepObject)} is required");
			}

			if (Member == null)
			{
				throw new InvalidOperationException($"{nameof(Member)} is required");
			}

			if (Member is not PropertyInfo && Member is not FieldInfo)
			{
				throw new InvalidOperationException($"{nameof(Member)} must be a property or field");
			}

			if (Attribute == null)
			{
				throw new InvalidOperationException($"{nameof(Attribute)} is required");
			}

			if (Attribute is not SuccessOutboxAttribute)
			{
				throw new InvalidOperationException($"{nameof(Attribute)} must be of type {nameof(SuccessOutboxAttribute)}");
			}

			if (!StepObject.GetType().GetMethods(BindingFlags.Instance | BindingFlags.Public | BindingFlags.NonPublic).Any(x => x == Member))
			{
				throw new InvalidOperationException($"{nameof(Member)} must be a member of {nameof(StepObject)}'s type");
			}

			if (!Member.GetType().IsDefined(Attribute.GetType()))
			{
				throw new InvalidOperationException($"{nameof(Member)} must have attribute of type {nameof(SuccessOutboxAttribute)} defined");
			}

			var action = new Action<Row>(AddRow);
			var attribute = (SuccessOutboxAttribute)Attribute;
			
			if (Member is PropertyInfo property)
			{
				property.SetValue(StepObject, action);
			}
			else if (Member is FieldInfo field)
			{
				field.SetValue(StepObject, action);
			}

			Name = string.IsNullOrWhiteSpace(attribute.Name) ? Member.Name : attribute.Name;
		}

		public void AddRow(Row row)
		{
			ActionBlock?.Post(row);
		}
	}
}
