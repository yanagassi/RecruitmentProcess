using EmployeeService.API.Models;
using Microsoft.EntityFrameworkCore;

namespace EmployeeService.API.Data
{
    public class ApplicationDbContext : DbContext
    {
        public DbSet<Employee> Employees { get; set; } = null!;
        public DbSet<User> Users { get; set; } = null!;

        public ApplicationDbContext(DbContextOptions<ApplicationDbContext> options) : base(options)
        {
        }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);

            modelBuilder.Entity<User>()
                .HasIndex(u => u.Email)
                .IsUnique();

            modelBuilder.Entity<Employee>()
                .HasIndex(e => e.Email)
                .IsUnique();

            modelBuilder.Entity<Employee>()
                .Property(e => e.Salary)
                .HasPrecision(18, 2);

            // Seed initial data
            modelBuilder.Entity<Employee>().HasData(
                new Employee
                {
                    Id = 1,
                    FirstName = "John",
                    LastName = "Doe",
                    Email = "john.doe@example.com",
                    Phone = "(555) 123-4567",
                    Age = 35,
                    Position = "Software Engineer",
                    Department = "Engineering",
                    Salary = 85000,
                    HireDate = DateTime.SpecifyKind(new DateTime(2020, 1, 15), DateTimeKind.Utc),
                    CreatedAt = DateTime.SpecifyKind(new DateTime(2024, 1, 1), DateTimeKind.Utc)
                },
                new Employee
                {
                    Id = 2,
                    FirstName = "Jane",
                    LastName = "Smith",
                    Email = "jane.smith@example.com",
                    Phone = "(555) 987-6543",
                    Age = 28,
                    Position = "Product Manager",
                    Department = "Product",
                    Salary = 95000,
                    HireDate = DateTime.SpecifyKind(new DateTime(2019, 6, 10), DateTimeKind.Utc),
                    CreatedAt = DateTime.SpecifyKind(new DateTime(2024, 1, 1), DateTimeKind.Utc)
                }
            );
        }
    }
}