using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Farm.Domain.Migrations
{
    /// <inheritdoc />
    public partial class AddAnimalIdToListing : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<Guid>(
                name: "AnimalId",
                table: "Listing",
                type: "uuid",
                nullable: true);

            migrationBuilder.CreateIndex(
                name: "IX_Listing_AnimalId",
                table: "Listing",
                column: "AnimalId");

            migrationBuilder.AddForeignKey(
                name: "FK_Listing_Animal_AnimalId",
                table: "Listing",
                column: "AnimalId",
                principalTable: "Animal",
                principalColumn: "Id",
                onDelete: ReferentialAction.SetNull);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Listing_Animal_AnimalId",
                table: "Listing");

            migrationBuilder.DropIndex(
                name: "IX_Listing_AnimalId",
                table: "Listing");

            migrationBuilder.DropColumn(
                name: "AnimalId",
                table: "Listing");
        }
    }
}
