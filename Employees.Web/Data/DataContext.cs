using Employees.Web.Models;
using Microsoft.EntityFrameworkCore;
using System;

namespace Employees.Web.Data
{
    public class DataContext: DbContext
    {
        public DataContext(DbContextOptions<DataContext> options) : base(options)
        {

        }

        public DbSet<Employee> Employees { get; set; }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);

            modelBuilder.Entity<Employee>()
                .HasIndex(t => t.RFC)
                .IsUnique();

            modelBuilder.Entity<Employee>()
           .Property(p => p.Status)
           .HasConversion(
               v => v.ToString(),
               v => (EmployeeStatus)Enum.Parse(typeof(EmployeeStatus), v));
        }

    }
}
