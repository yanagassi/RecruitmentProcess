using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace IdentityService.API.Migrations
{
    public partial class SeedDefaultUser : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            var userId = Guid.NewGuid().ToString();
            var hasher = new Microsoft.AspNetCore.Identity.PasswordHasher<object>();
            var passwordHash = hasher.HashPassword(null, "Admin123!");
            
            migrationBuilder.InsertData(
                table: "AspNetUsers",
                columns: new[] { "Id", "UserName", "NormalizedUserName", "Email", "NormalizedEmail", "EmailConfirmed", "PasswordHash", "SecurityStamp", "ConcurrencyStamp", "FirstName", "LastName", "CreatedAt", "PhoneNumberConfirmed", "TwoFactorEnabled", "LockoutEnabled", "AccessFailedCount" },
                values: new object[] { userId, "admin@admin.com", "ADMIN@ADMIN.COM", "admin@admin.com", "ADMIN@ADMIN.COM", true, passwordHash, Guid.NewGuid().ToString(), Guid.NewGuid().ToString(), "Admin", "User", DateTime.UtcNow, false, false, true, 0 }
            );
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DeleteData(
                table: "AspNetUsers",
                keyColumn: "Id",
                keyValue: "admin-user-id");
        }
    }
}
