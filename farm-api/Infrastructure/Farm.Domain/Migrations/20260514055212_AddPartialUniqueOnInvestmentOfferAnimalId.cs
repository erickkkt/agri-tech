using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Farm.Domain.Migrations
{
    /// <inheritdoc />
    public partial class AddPartialUniqueOnInvestmentOfferAnimalId : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropIndex(
                name: "IX_InvestmentOffer_AnimalId",
                table: "InvestmentOffer");

            migrationBuilder.CreateIndex(
                name: "IX_InvestmentOffer_AnimalId",
                table: "InvestmentOffer",
                column: "AnimalId",
                unique: true,
                filter: "\"Status\" = 1");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropIndex(
                name: "IX_InvestmentOffer_AnimalId",
                table: "InvestmentOffer");

            migrationBuilder.CreateIndex(
                name: "IX_InvestmentOffer_AnimalId",
                table: "InvestmentOffer",
                column: "AnimalId",
                unique: true);
        }
    }
}
