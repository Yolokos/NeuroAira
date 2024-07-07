using Microsoft.EntityFrameworkCore;
using NeuroAira.Database;
using NeuroAira.Models;
using NeuroAira.Repositories.Base;

namespace NeuroAira.Repositories
{
	public class UserConversationRepository : IUserConversationRepository
	{
		private NeuroAiraDbContext context;

		public UserConversationRepository (NeuroAiraDbContext context)
		{
			this.context = context;
		}

		public void CreateNewUserConversation (UserConversation userConversation)
		{
			context.UserConversations.Add(userConversation);
		}

		public Task<bool> IsUserConversationExist (long telegramId)
		{
			return context.UserConversations.AnyAsync(uc => uc.ConversationId == telegramId);
		}

		public Task<UserConversation> GetUserConversationWithMetaById (long telegramId)
		{
			return context.UserConversations.Where(c => c.ConversationId == telegramId).FirstAsync();
		}

		public Task Save ()
		{
			return context.SaveChangesAsync();
		}

		private bool disposed = false;

		protected virtual void Dispose (bool disposing)
		{
			if (!this.disposed)
			{
				if (disposing)
				{
					context.Dispose();
				}
			}
			this.disposed = true;
		}

		public void Dispose ()
		{
			Dispose(true);
			GC.SuppressFinalize(this);
		}
	}
}
