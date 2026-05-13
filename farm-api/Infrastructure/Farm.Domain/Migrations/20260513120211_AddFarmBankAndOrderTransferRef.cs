using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Farm.Domain.Migrations
{
    /// <inheritdoc />
    public partial class AddFarmBankAndOrderTransferRef : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<DateTime>(
                name: "BankTransferConfirmedAt",
                table: "InvestmentOrder",
                type: "timestamp with time zone",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "TransferReference",
                table: "InvestmentOrder",
                type: "character varying(50)",
                maxLength: 50,
                nullable: false,
                defaultValue: "");

            migrationBuilder.AddColumn<string>(
                name: "BankAccountHolder",
                table: "Farm",
                type: "character varying(250)",
                maxLength: 250,
                nullable: false,
                defaultValue: "");

            migrationBuilder.AddColumn<string>(
                name: "BankAccountNumber",
                table: "Farm",
                type: "character varying(50)",
                maxLength: 50,
                nullable: false,
                defaultValue: "");

            migrationBuilder.AddColumn<string>(
                name: "BankBranch",
                table: "Farm",
                type: "character varying(120)",
                maxLength: 120,
                nullable: false,
                defaultValue: "");

            migrationBuilder.AddColumn<string>(
                name: "BankName",
                table: "Farm",
                type: "character varying(120)",
                maxLength: 120,
                nullable: false,
                defaultValue: "");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "BankTransferConfirmedAt",
                table: "InvestmentOrder");

            migrationBuilder.DropColumn(
                name: "TransferReference",
                table: "InvestmentOrder");

            migrationBuilder.DropColumn(
                name: "BankAccountHolder",
                table: "Farm");

            migrationBuilder.DropColumn(
                name: "BankAccountNumber",
                table: "Farm");

            migrationBuilder.DropColumn(
                name: "BankBranch",
                table: "Farm");

            migrationBuilder.DropColumn(
                name: "BankName",
                table: "Farm");
        }
    }
}
