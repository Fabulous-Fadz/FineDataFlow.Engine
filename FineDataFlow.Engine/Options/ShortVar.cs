namespace FineDataFlow.Engine.Options
{
	public class ShortVar : Var<short>
	{
		public override void ParseText()
		{
			if (string.IsNullOrWhiteSpace(Text))
			{
				return;
			}

			Value = short.Parse(Text);
		}

		public override void StringifyValue()
		{
			Text = Value.ToString();
		}
	}
}
