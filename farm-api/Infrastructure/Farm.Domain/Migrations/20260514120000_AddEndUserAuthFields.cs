using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Farm.Domain.Migrations
{
    /// <summary>
    /// Phase 4 - End-user auth. Adds PasswordHash / AuthProvider / ExternalId /
    /// DisplayName to the User table, plus a unique index on EmailAddress so
    /// /auth/register can rely on the DB to reject duplicates as a final safety net.
    /// </summary>
    public partial class AddEndUserAuthFields : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<int>(
                name: "AuthProvider",
                table: "User",
                type: "integer",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.AddColumn<string>(
                name: "DisplayName",
                table: "User",
                type: "character varying(250)",
                maxLength: 250,
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "ExternalId",
                table: "User",
                type: "character varying(250)",
                maxLength: 250,
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "PasswordHash",
                table: "User",
                type: "character varying(500)",
                maxLength: 500,
                nullable: true);

            // Note: if the existing User table has duplicate emails (e.g. test data),
            // this CREATE UNIQUE INDEX will fail. Run a SELECT to dedupe first:
            //   SELECT "EmailAddress", COUNT(*) FROM "User" GROUP BY 1 HAVING COUNT(*) > 1;
            migrationBuilder.CreateIndex(
                name: "IX_User_EmailAddress_Unique",
                table: "User",
                column: "EmailAddress",
                unique: true);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropIndex(
                name: "IX_User_EmailAddress_Unique",
                table: "User");

            migrationBuilder.DropColumn(name: "AuthProvider",  table: "User");
            migrationBuilder.DropColumn(name: "DisplayName",   table: "User");
            migrationBuilder.DropColumn(name: "ExternalId",    table: "User");
            migrationBuilder.DropColumn(name: "PasswordHash",  table: "User");
        }
    }
}
