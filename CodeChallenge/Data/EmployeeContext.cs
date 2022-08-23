using CodeChallenge.Models;
using Microsoft.EntityFrameworkCore;

namespace CodeChallenge.Data
{
    public class EmployeeContext : DbContext
    {
        public EmployeeContext(DbContextOptions<EmployeeContext> options) : base(options)
        {

        }

        protected override void OnModelCreating(ModelBuilder builder)
        {
            // define primary key for Compensation
            builder.Entity<Compensation>()
                .HasKey(c => c.Id);

            // configure one-to-one relationship between Employee and Compensation
            builder.Entity<Employee>()
                .HasOne(e => e.Compensation)
                .WithOne(c => c.Employee)
                .HasForeignKey<Compensation>(c => c.EmployeeId);
        }

        public DbSet<Employee> Employees { get; set; }

        public DbSet<Compensation> Compensations { get; set; }
    }
}
