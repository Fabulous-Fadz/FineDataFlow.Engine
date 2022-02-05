namespace FineDataFlow.Engine.Options
{
	public class FloatVar : Var<float>
	{
		public override void ParseText()
		{
			if (string.IsNullOrWhiteSpace(Text))
			{
				return;
			}

			Value = float.Parse(Text);
		}

		public override void StringifyValue()
		{
			Text = Value.ToString();
		}
	}
}
