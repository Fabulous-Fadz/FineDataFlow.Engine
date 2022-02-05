using System;

namespace FineDataFlow.Engine.Options
{
	public class BooleanVar : Var<bool>
	{
		public override void ParseText()
		{
			if (string.IsNullOrWhiteSpace(Text))
			{
				return;
			}

			if (Text.Equals("True", StringComparison.OrdinalIgnoreCase) || Text.Equals("Yes", StringComparison.OrdinalIgnoreCase))
			{
				Value = true;
			}

			Value = bool.Parse(Text);
		}

		public override void StringifyValue()
		{
			Text = Value.ToString();
		}
	}
}
