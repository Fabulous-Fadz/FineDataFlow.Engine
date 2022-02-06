namespace FineDataFlow.Engine.Vars
{
	public class LongVar : Var<long>
	{
		internal override void ParseText()
		{
			if (string.IsNullOrWhiteSpace(Text))
			{
				return;
			}

			Value = long.Parse(Text);
		}

		internal override void StringifyValue()
		{
			Text = Value.ToString();
		}
	}
}
