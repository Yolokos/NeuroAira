namespace NeuroAira.Dtos
{
	public class ConversationMapModel
	{
		public string conversation_id { get; set; }
		public string action { get; set; }
		public string model { get; set; }
		public string provider { get; set; }
		public string jailbreak { get; set; }
		public MetaMapModel meta { get; set; }
	}
}
