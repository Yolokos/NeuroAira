using NeuroAira.Models;

namespace NeuroAira.Repositories.Base
{
	public interface IUserConversationRepository : IDisposable
	{
		void CreateNewUserConversation (UserConversation userConversation);
		Task<UserConversation> GetUserConversationWithMetaById (long telegramId);
		Task<bool> IsUserConversationExist (long telegramId);
		Task Save ();
	}
}
