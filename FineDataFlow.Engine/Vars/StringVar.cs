namespace FineDataFlow.Engine.Vars
{
	public class StringVar : Var<string>
	{
		internal override void ParseText()
		{
			Value = Text;
		}

		internal override void StringifyValue()
		{
			Text = Value;
		}
	}
}
