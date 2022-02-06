namespace FineDataFlow.Engine.Vars
{
	public class FloatVar : Var<float>
	{
		internal override void ParseText()
		{
			if (string.IsNullOrWhiteSpace(Text))
			{
				return;
			}

			Value = float.Parse(Text);
		}

		internal override void StringifyValue()
		{
			Text = Value.ToString();
		}
	}
}
