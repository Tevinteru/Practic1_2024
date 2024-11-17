using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Practic1_2024.Migrations
{
    /// <inheritdoc />
    public partial class smarphoneid : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_OrderItems_Smartphones_ProductId",
                table: "OrderItems");

            migrationBuilder.RenameColumn(
                name: "ProductId",
                table: "OrderItems",
                newName: "SmartphoneId");

            migrationBuilder.RenameIndex(
                name: "IX_OrderItems_ProductId",
                table: "OrderItems",
                newName: "IX_OrderItems_SmartphoneId");

            migrationBuilder.AlterColumn<DateOnly>(
                name: "UpdatedAt",
                table: "Orders",
                type: "date",
                nullable: false,
                oldClrType: typeof(DateTime),
                oldType: "timestamp with time zone");

            migrationBuilder.AlterColumn<DateOnly>(
                name: "CreatedAt",
                table: "Orders",
                type: "date",
                nullable: false,
                oldClrType: typeof(DateTime),
                oldType: "timestamp with time zone");

            migrationBuilder.AddForeignKey(
                name: "FK_OrderItems_Smartphones_SmartphoneId",
                table: "OrderItems",
                column: "SmartphoneId",
                principalTable: "Smartphones",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_OrderItems_Smartphones_SmartphoneId",
                table: "OrderItems");

            migrationBuilder.RenameColumn(
                name: "SmartphoneId",
                table: "OrderItems",
                newName: "ProductId");

            migrationBuilder.RenameIndex(
                name: "IX_OrderItems_SmartphoneId",
                table: "OrderItems",
                newName: "IX_OrderItems_ProductId");

            migrationBuilder.AlterColumn<DateTime>(
                name: "UpdatedAt",
                table: "Orders",
                type: "timestamp with time zone",
                nullable: false,
                oldClrType: typeof(DateOnly),
                oldType: "date");

            migrationBuilder.AlterColumn<DateTime>(
                name: "CreatedAt",
                table: "Orders",
                type: "timestamp with time zone",
                nullable: false,
                oldClrType: typeof(DateOnly),
                oldType: "date");

            migrationBuilder.AddForeignKey(
                name: "FK_OrderItems_Smartphones_ProductId",
                table: "OrderItems",
                column: "ProductId",
                principalTable: "Smartphones",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);
        }
    }
}
