using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Text;

namespace Tooded_EFCore
{
    public class ToodedContext : DbContext
    {
        public DbSet<Toode> Toodetabel { get; set; }
        public DbSet<Kategooria> Kategooriatabel { get; set; }

        protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
        {
            optionsBuilder.UseSqlServer(
    @"Server=(localdb)\MSSQLLocalDB;Database=Tooded_DB_Uus;Trusted_Connection=True;Encrypt=False;");
        }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            modelBuilder.Entity<Kategooria>().ToTable("Kategooriatabel");
            modelBuilder.Entity<Toode>().ToTable("Toodetabel");
        }
    }
}
