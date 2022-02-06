namespace FineDataFlow.Engine.Vars
{
	public class DecimalVar : Var<decimal>
	{
		internal override void ParseText()
		{
			if (string.IsNullOrWhiteSpace(Text))
			{
				return;
			}

			Value = decimal.Parse(Text);
		}

		internal override void StringifyValue()
		{
			Text = Value.ToString();
		}
	}
}
