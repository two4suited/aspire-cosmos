using Microsoft.EntityFrameworkCore;
using aspire_cosmos.ApiService.Models;

namespace aspire_cosmos.ApiService.Data
{
    public class PersonDbContext : DbContext
    {
        public PersonDbContext(DbContextOptions<PersonDbContext> options) : base(options) { }

        public DbSet<Person> People => Set<Person>();

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            modelBuilder.Entity<Person>().ToContainer("People");
            modelBuilder.Entity<Person>().HasPartitionKey(p => p.Id);
            modelBuilder.Entity<Person>().Property(p => p.Id).ToJsonProperty("id");
        }
    }
}
