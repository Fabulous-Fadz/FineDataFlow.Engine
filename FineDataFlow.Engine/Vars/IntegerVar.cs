namespace FineDataFlow.Engine.Vars
{
	public class IntegerVar : Var<int>
	{
		internal override void ParseText()
		{
			if (string.IsNullOrWhiteSpace(Text))
			{
				return;
			}

			Value = int.Parse(Text);
		}

		internal override void StringifyValue()
		{
			Text = Value.ToString();
		}
	}
}
