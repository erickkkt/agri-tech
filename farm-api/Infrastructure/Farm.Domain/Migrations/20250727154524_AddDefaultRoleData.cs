using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Farm.Domain.Migrations
{
    /// <inheritdoc />
    public partial class AddDefaultRoleData : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.InsertData(
                table: "Role",
                columns: new[] { "Id", "Description", "IsActive", "Name" },
                values: new object[] { new Guid("68dffda0-b650-44ac-a599-6cdbdfd641e4"), "System Administrator", true, "HO.SYSADMIN" });
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DeleteData(
                table: "Role",
                keyColumn: "Id",
                keyValue: new Guid("68dffda0-b650-44ac-a599-6cdbdfd641e4"));
        }
    }
}
