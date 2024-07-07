namespace NeuroAira.Models
{
	public class MetaConversationContent
	{
		public List<ConversationDetails> Prompts { get; set; }
		public List<ConversationDetails> Conversations { get; set; }
		public bool InternetAccess { get; set; }
		public int ContentType { get; set; }
	}
}
