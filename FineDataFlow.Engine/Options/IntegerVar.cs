namespace FineDataFlow.Engine.Options
{
	public class IntegerVar : Var<int>
	{
		public override void ParseText()
		{
			if (string.IsNullOrWhiteSpace(Text))
			{
				return;
			}

			Value = int.Parse(Text);
		}

		public override void StringifyValue()
		{
			Text = Value.ToString();
		}
	}
}
