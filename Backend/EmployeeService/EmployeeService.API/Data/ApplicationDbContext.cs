using EmployeeService.API.Models;
using Microsoft.EntityFrameworkCore;

namespace EmployeeService.API.Data
{
    public class ApplicationDbContext : DbContext
    {
        public DbSet<Employee> Employees { get; set; } = null!;
        public DbSet<EmployeePhone> EmployeePhones { get; set; } = null!;
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
                .HasIndex(e => e.DocNumber)
                .IsUnique();

            modelBuilder.Entity<Employee>()
                .Property(e => e.Salary)
                .HasPrecision(18, 2);

            // Configure Employee-Manager relationship
            modelBuilder.Entity<Employee>()
                .HasOne(e => e.Manager)
                .WithMany(e => e.Subordinates)
                .HasForeignKey(e => e.ManagerId)
                .OnDelete(DeleteBehavior.Restrict);

            // Configure Employee-Phone relationship
            modelBuilder.Entity<EmployeePhone>()
                .HasOne(p => p.Employee)
                .WithMany(e => e.Phones)
                .HasForeignKey(p => p.EmployeeId)
                .OnDelete(DeleteBehavior.Cascade);

            // Seed initial data
            modelBuilder.Entity<Employee>().HasData(
                new Employee
                {
                    Id = 1,
                    FirstName = "John",
                    LastName = "Doe",
                    Email = "john.doe@example.com",
                    DocNumber = "123.456.789-00",
                    Age = 35,
                    Position = "Software Engineer",
                    Department = "Engineering",
                    Salary = 85000,
                    PermissionLevel = PermissionLevel.Leader,
                    HireDate = DateTime.SpecifyKind(new DateTime(2020, 1, 15), DateTimeKind.Utc),
                    CreatedAt = DateTime.SpecifyKind(new DateTime(2024, 1, 1), DateTimeKind.Utc)
                },
                new Employee
                {
                    Id = 2,
                    FirstName = "Jane",
                    LastName = "Smith",
                    Email = "jane.smith@example.com",
                    DocNumber = "987.654.321-00",
                    Age = 28,
                    Position = "Product Manager",
                    Department = "Product",
                    Salary = 95000,
                    ManagerId = 1,
                    PermissionLevel = PermissionLevel.Employee,
                    HireDate = DateTime.SpecifyKind(new DateTime(2019, 6, 10), DateTimeKind.Utc),
                    CreatedAt = DateTime.SpecifyKind(new DateTime(2024, 1, 1), DateTimeKind.Utc)
                }
            );

            // Seed phone data
            modelBuilder.Entity<EmployeePhone>().HasData(
                new EmployeePhone
                {
                    Id = 1,
                    EmployeeId = 1,
                    PhoneNumber = "(555) 123-4567",
                    PhoneType = "Mobile",
                    IsPrimary = true,
                    CreatedAt = DateTime.SpecifyKind(new DateTime(2024, 1, 1), DateTimeKind.Utc)
                },
                new EmployeePhone
                {
                    Id = 2,
                    EmployeeId = 2,
                    PhoneNumber = "(555) 987-6543",
                    PhoneType = "Mobile",
                    IsPrimary = true,
                    CreatedAt = DateTime.SpecifyKind(new DateTime(2024, 1, 1), DateTimeKind.Utc)
                }
            );
        }
    }
}