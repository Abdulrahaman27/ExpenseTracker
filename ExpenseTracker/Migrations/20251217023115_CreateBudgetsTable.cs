using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace ExpenseTracker.Migrations
{
    /// <inheritdoc />
    public partial class CreateBudgetsTable : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AlterColumn<string>(
                name: "Description",
                table: "Budgets",
                type: "nvarchar(500)",
                maxLength: 500,
                nullable: true,
                oldClrType: typeof(string),
                oldType: "nvarchar(200)",
                oldMaxLength: 200,
                oldNullable: true);

            migrationBuilder.AlterColumn<int>(
                name: "CategoryId",
                table: "Budgets",
                type: "int",
                nullable: true,
                oldClrType: typeof(int),
                oldType: "int");

            migrationBuilder.AddColumn<DateTime>(
                name: "CreatedDate",
                table: "Budgets",
                type: "datetime2",
                nullable: false,
                defaultValue: new DateTime(1, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified));

            migrationBuilder.AddColumn<DateTime>(
                name: "ModifiedDate",
                table: "Budgets",
                type: "datetime2",
                nullable: false,
                defaultValue: new DateTime(1, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified));

            migrationBuilder.AddColumn<int>(
                name: "Period",
                table: "Budgets",
                type: "int",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.UpdateData(
                table: "Categories",
                keyColumn: "Id",
                keyValue: 1,
                columns: new[] { "CreatedDate", "ModifiedDate" },
                values: new object[] { new DateTime(2025, 12, 17, 2, 31, 14, 591, DateTimeKind.Local).AddTicks(4977), new DateTime(2025, 12, 17, 2, 31, 14, 591, DateTimeKind.Local).AddTicks(5033) });

            migrationBuilder.UpdateData(
                table: "Categories",
                keyColumn: "Id",
                keyValue: 2,
                columns: new[] { "CreatedDate", "ModifiedDate" },
                values: new object[] { new DateTime(2025, 12, 17, 2, 31, 14, 591, DateTimeKind.Local).AddTicks(5042), new DateTime(2025, 12, 17, 2, 31, 14, 591, DateTimeKind.Local).AddTicks(5044) });

            migrationBuilder.UpdateData(
                table: "Categories",
                keyColumn: "Id",
                keyValue: 3,
                columns: new[] { "CreatedDate", "ModifiedDate" },
                values: new object[] { new DateTime(2025, 12, 17, 2, 31, 14, 591, DateTimeKind.Local).AddTicks(5048), new DateTime(2025, 12, 17, 2, 31, 14, 591, DateTimeKind.Local).AddTicks(5050) });

            migrationBuilder.UpdateData(
                table: "Categories",
                keyColumn: "Id",
                keyValue: 4,
                columns: new[] { "CreatedDate", "ModifiedDate" },
                values: new object[] { new DateTime(2025, 12, 17, 2, 31, 14, 591, DateTimeKind.Local).AddTicks(5055), new DateTime(2025, 12, 17, 2, 31, 14, 591, DateTimeKind.Local).AddTicks(5056) });

            migrationBuilder.UpdateData(
                table: "Categories",
                keyColumn: "Id",
                keyValue: 5,
                columns: new[] { "CreatedDate", "ModifiedDate" },
                values: new object[] { new DateTime(2025, 12, 17, 2, 31, 14, 591, DateTimeKind.Local).AddTicks(5060), new DateTime(2025, 12, 17, 2, 31, 14, 591, DateTimeKind.Local).AddTicks(5062) });

            migrationBuilder.UpdateData(
                table: "Categories",
                keyColumn: "Id",
                keyValue: 6,
                columns: new[] { "CreatedDate", "ModifiedDate" },
                values: new object[] { new DateTime(2025, 12, 17, 2, 31, 14, 591, DateTimeKind.Local).AddTicks(5066), new DateTime(2025, 12, 17, 2, 31, 14, 591, DateTimeKind.Local).AddTicks(5067) });
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "CreatedDate",
                table: "Budgets");

            migrationBuilder.DropColumn(
                name: "ModifiedDate",
                table: "Budgets");

            migrationBuilder.DropColumn(
                name: "Period",
                table: "Budgets");

            migrationBuilder.AlterColumn<string>(
                name: "Description",
                table: "Budgets",
                type: "nvarchar(200)",
                maxLength: 200,
                nullable: true,
                oldClrType: typeof(string),
                oldType: "nvarchar(500)",
                oldMaxLength: 500,
                oldNullable: true);

            migrationBuilder.AlterColumn<int>(
                name: "CategoryId",
                table: "Budgets",
                type: "int",
                nullable: false,
                defaultValue: 0,
                oldClrType: typeof(int),
                oldType: "int",
                oldNullable: true);

            migrationBuilder.UpdateData(
                table: "Categories",
                keyColumn: "Id",
                keyValue: 1,
                columns: new[] { "CreatedDate", "ModifiedDate" },
                values: new object[] { new DateTime(2025, 12, 17, 1, 46, 49, 236, DateTimeKind.Local).AddTicks(1418), new DateTime(2025, 12, 17, 1, 46, 49, 236, DateTimeKind.Local).AddTicks(1468) });

            migrationBuilder.UpdateData(
                table: "Categories",
                keyColumn: "Id",
                keyValue: 2,
                columns: new[] { "CreatedDate", "ModifiedDate" },
                values: new object[] { new DateTime(2025, 12, 17, 1, 46, 49, 236, DateTimeKind.Local).AddTicks(1475), new DateTime(2025, 12, 17, 1, 46, 49, 236, DateTimeKind.Local).AddTicks(1476) });

            migrationBuilder.UpdateData(
                table: "Categories",
                keyColumn: "Id",
                keyValue: 3,
                columns: new[] { "CreatedDate", "ModifiedDate" },
                values: new object[] { new DateTime(2025, 12, 17, 1, 46, 49, 236, DateTimeKind.Local).AddTicks(1480), new DateTime(2025, 12, 17, 1, 46, 49, 236, DateTimeKind.Local).AddTicks(1481) });

            migrationBuilder.UpdateData(
                table: "Categories",
                keyColumn: "Id",
                keyValue: 4,
                columns: new[] { "CreatedDate", "ModifiedDate" },
                values: new object[] { new DateTime(2025, 12, 17, 1, 46, 49, 236, DateTimeKind.Local).AddTicks(1484), new DateTime(2025, 12, 17, 1, 46, 49, 236, DateTimeKind.Local).AddTicks(1486) });

            migrationBuilder.UpdateData(
                table: "Categories",
                keyColumn: "Id",
                keyValue: 5,
                columns: new[] { "CreatedDate", "ModifiedDate" },
                values: new object[] { new DateTime(2025, 12, 17, 1, 46, 49, 236, DateTimeKind.Local).AddTicks(1488), new DateTime(2025, 12, 17, 1, 46, 49, 236, DateTimeKind.Local).AddTicks(1490) });

            migrationBuilder.UpdateData(
                table: "Categories",
                keyColumn: "Id",
                keyValue: 6,
                columns: new[] { "CreatedDate", "ModifiedDate" },
                values: new object[] { new DateTime(2025, 12, 17, 1, 46, 49, 236, DateTimeKind.Local).AddTicks(1493), new DateTime(2025, 12, 17, 1, 46, 49, 236, DateTimeKind.Local).AddTicks(1494) });
        }
    }
}
