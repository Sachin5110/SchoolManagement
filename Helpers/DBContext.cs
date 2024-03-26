using Microsoft.EntityFrameworkCore;
using System;
using Microsoft.Extensions.Configuration;
using school_management_backend.Models;

namespace school_management_backend.Helpers
{
    public class DBContext : DbContext
    {
        public DBContext()
        {
        }

        public DBContext(DbContextOptions<DBContext> options) : base(options)
        {

        }
        public DbSet<jwt_tokens> Jwt_Tokens { get; set; }
        public DbSet<auditlog> auditlog { get; set; }
        public DbSet<School> Schools { get; set; }
        public DbSet<Users> Users { get; set; }
        public DbSet<Roles> Roles { get; set; }
        public DbSet<Teacher> Teachers { get; set; }
        public DbSet<Student> Students { get; set; }
        protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
        {
            IConfigurationRoot configuration = new ConfigurationBuilder()
                .SetBasePath(AppDomain.CurrentDomain.BaseDirectory)
                .AddJsonFile("appsettings.json")
                .Build();
            optionsBuilder.UseSqlServer(configuration.GetConnectionString("DefaultConnection"));
        }
    }
}