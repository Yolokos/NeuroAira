namespace NeuroAira.Models
{
	public class UserConversation
	{ 
		public int Id { get; set; }
		public long ConversationId { get; set; }
		public string Action { get; set; }
		public int Model { get; set; }
		public string Provider { get; set; }
		public string Jailbreak { get; set; }
		
		public MetaConversation MetaConversation { get; set; }
	}
}
