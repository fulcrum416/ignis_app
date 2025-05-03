using Ignis.Data.DbModel;
using Ignis.Models;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Ignis.Data
{
    public class AppDbContext:IdentityDbContext<AppUser>
    {
        public AppDbContext(DbContextOptions<AppDbContext> options) : base(options) { }
        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);

            //modelBuilder.Entity<AppLog>().HasNoKey();


            // Apply lowercase naming convention globally
            modelBuilder.UseLowerCaseNamingConvention();
        }

        public DbSet<AppLog> AppLogs { get; set; }
        public DbSet<Tag> Tags { get; set; }
        public DbSet<TagValue> TagValues { get; set; }
        public DbSet<TagDefinition> TagsDefinitions { get; set; }
    }
}
