using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace EmployeeService.API.Migrations
{
    public partial class SeedMoreEmployees : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            // Inserir funcionários de exemplo
            migrationBuilder.InsertData(
                table: "Employees",
                columns: new[] { "Id", "FirstName", "LastName", "Email", "Phone", "Age", "Position", "Department", "Salary", "HireDate", "CreatedAt" },
                values: new object[,]
                {
                    {
                        3,
                        "Carlos",
                        "Silva",
                        "carlos.silva@example.com",
                        "(555) 111-2222",
                        32,
                        "DevOps Engineer",
                        "Engineering",
                        90000m,
                        new DateTime(2021, 3, 20, 0, 0, 0, DateTimeKind.Utc),
                        new DateTime(2024, 1, 1, 0, 0, 0, DateTimeKind.Utc)
                    },
                    {
                        4,
                        "Ana",
                        "Costa",
                        "ana.costa@example.com",
                        "(555) 333-4444",
                        29,
                        "UX Designer",
                        "Design",
                        75000m,
                        new DateTime(2022, 1, 10, 0, 0, 0, DateTimeKind.Utc),
                        new DateTime(2024, 1, 1, 0, 0, 0, DateTimeKind.Utc)
                    },
                    {
                        5,
                        "Roberto",
                        "Santos",
                        "roberto.santos@example.com",
                        "(555) 555-6666",
                        41,
                        "Tech Lead",
                        "Engineering",
                        120000m,
                        new DateTime(2018, 8, 15, 0, 0, 0, DateTimeKind.Utc),
                        new DateTime(2024, 1, 1, 0, 0, 0, DateTimeKind.Utc)
                    },
                    {
                        6,
                        "Maria",
                        "Oliveira",
                        "maria.oliveira@example.com",
                        "(555) 777-8888",
                        26,
                        "Marketing Specialist",
                        "Marketing",
                        65000m,
                        new DateTime(2023, 2, 5, 0, 0, 0, DateTimeKind.Utc),
                        new DateTime(2024, 1, 1, 0, 0, 0, DateTimeKind.Utc)
                    },
                    {
                        7,
                        "Pedro",
                        "Ferreira",
                        "pedro.ferreira@example.com",
                        "(555) 999-0000",
                        38,
                        "Sales Manager",
                        "Sales",
                        110000m,
                        new DateTime(2019, 11, 30, 0, 0, 0, DateTimeKind.Utc),
                        new DateTime(2024, 1, 1, 0, 0, 0, DateTimeKind.Utc)
                    },
                    {
                        8,
                        "Lucia",
                        "Almeida",
                        "lucia.almeida@example.com",
                        "(555) 222-3333",
                        33,
                        "HR Manager",
                        "Human Resources",
                        85000m,
                        new DateTime(2020, 7, 12, 0, 0, 0, DateTimeKind.Utc),
                        new DateTime(2024, 1, 1, 0, 0, 0, DateTimeKind.Utc)
                    }
                }
            );
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            // Remover funcionários de exemplo
            migrationBuilder.DeleteData(
                table: "Employees",
                keyColumn: "Id",
                keyValues: new object[] { 3, 4, 5, 6, 7, 8 }
            );
        }
    }
}
