namespace FineDataFlow.Engine.Abstractions.Models
{
	/// <summary>
	/// Represents a <see cref="Hop"/> (a link between <see cref="Step"/>s in a <see cref="Flow"/>)
	/// </summary>
	public class Hop
	{
		/// <summary>
		/// An indication of wether or not the <see cref="Hop"/> is enabled/active 
		/// </summary>
		public virtual bool Enabled { get; set; } = true;

		/// <summary>
		/// The name of the <see cref="Step"/> from which the <see cref="Hop"/> links
		/// </summary>
		public virtual string FromStepName { get; set; }

		/// <summary>
		/// The name of the outbox of the <see cref="Step"/> from which the <see cref="Hop"/> links
		/// </summary>
		public virtual string FromOutboxName { get; set; }

		/// <summary>
		/// The name of the <see cref="Step"/> to which the <see cref="Hop"/> links
		/// </summary>
		public virtual string ToStepName { get; set; }

		/// <summary>
		/// The name of the inbox of the <see cref="Step"/> to which the <see cref="Hop"/> links
		/// </summary>
		public virtual string ToInboxName { get; set; }
	}
}