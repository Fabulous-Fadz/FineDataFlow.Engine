namespace FineDataFlow.Engine.Options
{
	public class DecimalVar : Var<decimal>
	{
		public override void ParseText()
		{
			if (string.IsNullOrWhiteSpace(Text))
			{
				return;
			}

			Value = decimal.Parse(Text);
		}

		public override void StringifyValue()
		{
			Text = Value.ToString();
		}
	}
}
