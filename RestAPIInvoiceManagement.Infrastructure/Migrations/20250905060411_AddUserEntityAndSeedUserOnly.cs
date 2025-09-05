using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace RestAPIInvoiceManagement.Infrastructure.Migrations
{
    /// <inheritdoc />
    public partial class AddUserEntityAndSeedUserOnly : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "Users",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "TEXT", nullable: false),
                    Username = table.Column<string>(type: "TEXT", maxLength: 50, nullable: false),
                    PasswordHash = table.Column<string>(type: "TEXT", maxLength: 255, nullable: false),
                    CreatedAt = table.Column<DateTime>(type: "TEXT", nullable: false),
                    UpdatedAt = table.Column<DateTime>(type: "TEXT", nullable: true),
                    IsDeleted = table.Column<bool>(type: "INTEGER", nullable: false, defaultValue: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Users", x => x.Id);
                });

            migrationBuilder.InsertData(
                table: "Users",
                columns: new[] { "Id", "CreatedAt", "PasswordHash", "UpdatedAt", "Username" },
                values: new object[] { new Guid("bca91c90-738d-43b7-bba4-2450c0dd9e9d"), new DateTime(2025, 9, 5, 6, 4, 10, 796, DateTimeKind.Utc).AddTicks(4891), "$2a$11$LyURKqU7evKNyf.sEHLSb.MdfNkOT1K.m406E/nxUaSrOiPtTcGaO", null, "admin" });

            migrationBuilder.CreateIndex(
                name: "IX_Users_Username",
                table: "Users",
                column: "Username",
                unique: true);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "Users");
        }
    }
}
