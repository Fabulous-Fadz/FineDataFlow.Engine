using System;

namespace FineDataFlow.Engine.Options
{
	public class BooleanVar : Var<bool>
	{
		internal override void ParseText()
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

		internal override void StringifyValue()
		{
			Text = Value.ToString();
		}
	}
}
