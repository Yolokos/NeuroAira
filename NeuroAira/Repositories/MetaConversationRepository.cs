using Microsoft.EntityFrameworkCore;
using NeuroAira.Database;
using NeuroAira.Models;
using NeuroAira.Repositories.Base;

namespace NeuroAira.Repositories
{
	public class MetaConversationRepository : IMetaConversationRepository
	{
		private NeuroAiraDbContext context;

		public MetaConversationRepository (NeuroAiraDbContext context)
		{
			this.context = context;
		}

		public async Task<int> CreateNewMetaConversation (MetaConversation metaConversation)
		{
			var meta = await context.MetaConversations.AddAsync(metaConversation);

			return meta.Entity.Id;
		}

		public MetaConversation GetMetaConversationById (int userConversationId)
		{
			return context.MetaConversations.Where(c => c.UserConversationId == userConversationId).First();
		}

		public void UpdateMetaConversation (MetaConversation metaConversation)
		{
			context.MetaConversations.Update(metaConversation);
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
