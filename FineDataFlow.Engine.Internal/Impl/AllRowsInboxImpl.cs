using FineDataFlow.Engine.Abstractions;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Threading.Tasks.Dataflow;

namespace FineDataFlow.Engine.Internal.Impl
{
	internal class AllRowsInboxImpl : IAllRowsInbox
	{
		// properties

		public string Name { get; set; }
		public MemberInfo Member { get; set; }
		public IOutbox FromOutbox { get; set; }
		public Attribute Attribute { get; set; }
		public object StepObject { get; set; }
		public List<Row> Rows { get; set; } = new();
		public ActionBlock<Row> ActionBlock { get; set; }
		public Action<IEnumerable<Row>> Action { get; set; }

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

			if (Attribute is not AllRowsInboxAttribute)
			{
				throw new InvalidOperationException($"{nameof(Attribute)} must be of type {nameof(AllRowsInboxAttribute)}");
			}

			if (!StepObject.GetType().GetMethods(BindingFlags.Instance | BindingFlags.Public | BindingFlags.NonPublic).Any(x => x == Member))
			{
				throw new InvalidOperationException($"{nameof(Member)} must be a member of {nameof(StepObject)}'s type");
			}

			if (!Member.GetType().IsDefined(Attribute.GetType()))
			{
				throw new InvalidOperationException($"{nameof(Member)} must have attribute of type {nameof(AllRowsInboxAttribute)} defined");
			}
			
			var method = (MethodInfo)Member;
			var attribute = (AllRowsInboxAttribute)Attribute;
			
			Rows = new();
			Name = string.IsNullOrWhiteSpace(attribute.Name) ? Member.Name : attribute.Name;
			Action = (Action<IEnumerable<Row>>)method.CreateDelegate(typeof(Action<IEnumerable<Row>>), StepObject);
		}

		public void ProcessRow(Row row)
		{
			Rows.Add(row);
			
			if (row == null)
			{
				Action(Rows);

				Rows.Clear();
				Rows = null;
			}
		}
	}
}
