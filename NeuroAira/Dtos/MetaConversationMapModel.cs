namespace NeuroAira.Dtos
{
	public class MetaConversationMapModel
	{
		public List<RoleContentMapModel> conversation { get; set; }
		public bool internet_access { get; set; }
		public string content_type { get; set; }
		public List<RoleContentMapModel> parts { get; set; }
	}
}
