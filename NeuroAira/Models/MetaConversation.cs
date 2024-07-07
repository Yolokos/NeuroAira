namespace NeuroAira.Models
{
	public class MetaConversation
	{
		public int Id { get; set; }

		public string? Content { get; set; }

		public int UserConversationId { get; set; }
		public UserConversation UserConversation { get; set; }
	}
}
