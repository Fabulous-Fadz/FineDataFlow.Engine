namespace FineDataFlow.Engine
{
	public abstract class Var<T>
	{
		public string Text { get; internal set; }
		public T Value { get; internal set; }
		internal abstract void ParseText();
		internal abstract void StringifyValue();
	}
}
