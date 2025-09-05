using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace RestAPIInvoiceManagement.Infrastructure.Migrations
{
    /// <inheritdoc />
    public partial class RemoveSeedData : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DeleteData(
                table: "Users",
                keyColumn: "Id",
                keyValue: new Guid("123e4567-e89b-12d3-a456-426614174000"));
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.InsertData(
                table: "Users",
                columns: new[] { "Id", "CreatedAt", "PasswordHash", "UpdatedAt", "Username" },
                values: new object[] { new Guid("123e4567-e89b-12d3-a456-426614174000"), new DateTime(2025, 9, 1, 0, 0, 0, 0, DateTimeKind.Utc), "$2a$11$8wLU0oiAW2ihNBoz2xv74OrXC3TgDXRdLzMp4T00KBO2w.e7wnTYq", null, "admin" });
        }
    }
}
