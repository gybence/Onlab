using DataAccessLibrary.Model;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Text;

namespace DataAccessLibrary
{
	public class AppDbContext : DbContext
	{
		public DbSet<User> Users { get; set; }
		public DbSet<RssFeed> RssFeeds { get; set; }

		public DbSet<Subscription> Subscriptions { get; set; }

		protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
		{
			optionsBuilder.UseSqlite("Data Source=database.db");
		}
		protected override void OnModelCreating(ModelBuilder modelBuilder)
		{
			modelBuilder.HasChangeTrackingStrategy(ChangeTrackingStrategy.ChangedNotifications);

			//TODO: code-first, fluent
			//modelBuilder.Entity<User>().Property(t => t.UserName).IsRequired();
		}
	}
}
