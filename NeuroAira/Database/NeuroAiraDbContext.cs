using Microsoft.EntityFrameworkCore;
using NeuroAira.Models;

namespace NeuroAira.Database
{
	public class NeuroAiraDbContext : DbContext
	{
		public DbSet<UserConversation> UserConversations { get; set; }
		public DbSet<MetaConversation> MetaConversations { get; set; }

		private string pathDatabase = Path.Combine(Environment.CurrentDirectory, "NeuroAiraDb.db");

		protected override void OnConfiguring (DbContextOptionsBuilder optionsBuilder)
		{
			optionsBuilder.UseSqlite($"Data Source={pathDatabase}");
		}
	}
}
