using FineDataFlow.Engine.Abstractions;
using System;
using System.Linq;
using System.Reflection;
using System.Threading.Tasks.Dataflow;

namespace FineDataFlow.Engine.Implementations
{
	internal class RowStreamInboxImpl : IRowStreamInbox
	{
		// fields

		private string _name;
		private Action<Row> _action;

		// properties

		public string Name => _name;
		public object StepObject { get; set; }
		public MemberInfo Member { get; set; }
		public IOutbox FromOutbox { get; set; }
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

			if (Member is not MethodInfo)
			{
				throw new InvalidOperationException($"{nameof(Member)} must be a method");
			}

			if (Attribute == null)
			{
				throw new InvalidOperationException($"{nameof(Attribute)} is required");
			}

			if (Attribute is not RowStreamInboxAttribute)
			{
				throw new InvalidOperationException($"{nameof(Attribute)} must be of type {nameof(RowStreamInboxAttribute)}");
			}

			if (!StepObject.GetType().GetMethods(BindingFlags.Instance | BindingFlags.Public | BindingFlags.NonPublic).Any(x => x == Member))
			{
				throw new InvalidOperationException($"{nameof(Member)} must be a member of {nameof(StepObject)}'s type");
			}

			if (!Member.GetType().IsDefined(Attribute.GetType()))
			{
				throw new InvalidOperationException($"{nameof(Member)} must have attribute of type {nameof(RowStreamInboxAttribute)} defined");
			}

			var method = (MethodInfo)Member;
			var attribute = (RowStreamInboxAttribute)Attribute;

			_name = string.IsNullOrWhiteSpace(attribute.Name) ? Member.Name : attribute.Name;
			_action = (Action<Row>)method.CreateDelegate(typeof(Action<Row>), StepObject);
		}

		public void ProcessRow(Row row)
		{
			_action(row);
		}
	}
}
