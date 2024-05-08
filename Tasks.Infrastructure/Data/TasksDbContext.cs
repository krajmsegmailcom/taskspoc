using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Humanizer;
using Tasks.Core.Entities;
namespace Tasks.Infrastructure.Data
{
    public class TasksDbContext : DbContext
    {
        public TasksDbContext(DbContextOptions options) : base(options)
        {
        }
       
        protected TasksDbContext(string connectionString) : base(GetOptions(connectionString))
        {
        }
        private static DbContextOptions GetOptions(string connString)
        {
            return SqliteDbContextOptionsBuilderExtensions.UseSqlite(new DbContextOptionsBuilder(), connString).Options;
        }
        public DbSet<Core.Entities.Task> Tasks { get; set; }
        public DbSet<Core.Entities.Activity> Activities { get; set; }
        public DbSet<Core.Entities.User> Users { get; set; }
        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            modelBuilder.Entity<User>().HasData(
 new User { Id = 1, Username = "admin", Password = "password" },
 new User { Id = 2, Username = "user2", Password = "password" },
 new User { Id = 3, Username = "user3", Password = "password" },
 new User { Id = 4, Username = "user4", Password = "password" }
 );

            base.OnModelCreating(modelBuilder);
         
        }
    }
}
