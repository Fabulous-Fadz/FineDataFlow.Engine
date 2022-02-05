namespace FineDataFlow.Engine
{
	public abstract class Var<T>
	{
		public string Text { get; set; }
		public T Value { get; set; }
		internal abstract void ParseText();
		internal abstract void StringifyValue();
	}
}
