using NeuroAira.Models;

namespace NeuroAira.Repositories.Base
{
	public interface IMetaConversationRepository : IDisposable
	{
		Task<int> CreateNewMetaConversation (MetaConversation metaConversation);
		MetaConversation GetMetaConversationById (int metaConversationId);
		void UpdateMetaConversation (MetaConversation metaConversation);
		Task Save ();
	}
}
