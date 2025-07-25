using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace BussinessObjects.Migrations
{
    /// <inheritdoc />
    public partial class update1 : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "Paid",
                table: "Fines");

            migrationBuilder.RenameColumn(
                name: "Reason",
                table: "Fines",
                newName: "Status");

            migrationBuilder.RenameColumn(
                name: "PaidDate",
                table: "Fines",
                newName: "PaidAt");

            migrationBuilder.AddColumn<DateTime>(
                name: "CreatedAt",
                table: "Fines",
                type: "datetime2",
                nullable: false,
                defaultValue: new DateTime(1, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified));

            migrationBuilder.AddColumn<int>(
                name: "DaysLate",
                table: "Fines",
                type: "int",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.AddColumn<int>(
                name: "BookId",
                table: "BorrowRecords",
                type: "int",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.AddColumn<int>(
                name: "ExtendCount",
                table: "BorrowRecords",
                type: "int",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.CreateIndex(
                name: "IX_BorrowRecords_BookId",
                table: "BorrowRecords",
                column: "BookId");

            migrationBuilder.AddForeignKey(
                name: "FK_BorrowRecords_Books_BookId",
                table: "BorrowRecords",
                column: "BookId",
                principalTable: "Books",
                principalColumn: "BookId",
                onDelete: ReferentialAction.Restrict);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_BorrowRecords_Books_BookId",
                table: "BorrowRecords");

            migrationBuilder.DropIndex(
                name: "IX_BorrowRecords_BookId",
                table: "BorrowRecords");

            migrationBuilder.DropColumn(
                name: "CreatedAt",
                table: "Fines");

            migrationBuilder.DropColumn(
                name: "DaysLate",
                table: "Fines");

            migrationBuilder.DropColumn(
                name: "BookId",
                table: "BorrowRecords");

            migrationBuilder.DropColumn(
                name: "ExtendCount",
                table: "BorrowRecords");

            migrationBuilder.RenameColumn(
                name: "Status",
                table: "Fines",
                newName: "Reason");

            migrationBuilder.RenameColumn(
                name: "PaidAt",
                table: "Fines",
                newName: "PaidDate");

            migrationBuilder.AddColumn<bool>(
                name: "Paid",
                table: "Fines",
                type: "bit",
                nullable: false,
                defaultValue: false);
        }
    }
}
