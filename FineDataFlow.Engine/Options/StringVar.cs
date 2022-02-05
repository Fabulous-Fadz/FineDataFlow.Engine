namespace FineDataFlow.Engine.Options
{
	public class StringVar : Var<string>
	{
		public override void ParseText()
		{
			Value = Text;
		}

		public override void StringifyValue()
		{
			Text = Value;
		}
	}
}
