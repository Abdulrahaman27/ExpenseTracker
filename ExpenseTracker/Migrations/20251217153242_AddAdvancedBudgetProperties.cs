using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace ExpenseTracker.Migrations
{
    /// <inheritdoc />
    public partial class AddAdvancedBudgetProperties : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AlterColumn<DateTime>(
                name: "EndDate",
                table: "Budgets",
                type: "datetime2",
                nullable: true,
                oldClrType: typeof(DateTime),
                oldType: "datetime2");

            migrationBuilder.AddColumn<DateTime>(
                name: "LastResetDate",
                table: "Budgets",
                type: "datetime2",
                nullable: true);

            migrationBuilder.AddColumn<DateTime>(
                name: "NextResetDate",
                table: "Budgets",
                type: "datetime2",
                nullable: true);

            migrationBuilder.AddColumn<bool>(
                name: "NotifyOnExceed",
                table: "Budgets",
                type: "bit",
                nullable: false,
                defaultValue: false);

            migrationBuilder.AddColumn<bool>(
                name: "NotifyOnWarning",
                table: "Budgets",
                type: "bit",
                nullable: false,
                defaultValue: false);

            migrationBuilder.AddColumn<int>(
                name: "WarningThreshold",
                table: "Budgets",
                type: "int",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.CreateTable(
                name: "BudgetNotifications",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    BudgetId = table.Column<int>(type: "int", nullable: false),
                    Message = table.Column<string>(type: "nvarchar(500)", maxLength: 500, nullable: false),
                    Type = table.Column<int>(type: "int", nullable: false),
                    IsRead = table.Column<bool>(type: "bit", nullable: false),
                    CreatedDate = table.Column<DateTime>(type: "datetime2", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_BudgetNotifications", x => x.Id);
                    table.ForeignKey(
                        name: "FK_BudgetNotifications_Budgets_BudgetId",
                        column: x => x.BudgetId,
                        principalTable: "Budgets",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.UpdateData(
                table: "Categories",
                keyColumn: "Id",
                keyValue: 1,
                columns: new[] { "CreatedDate", "ModifiedDate" },
                values: new object[] { new DateTime(2025, 12, 17, 15, 32, 41, 321, DateTimeKind.Local).AddTicks(9349), new DateTime(2025, 12, 17, 15, 32, 41, 321, DateTimeKind.Local).AddTicks(9411) });

            migrationBuilder.UpdateData(
                table: "Categories",
                keyColumn: "Id",
                keyValue: 2,
                columns: new[] { "CreatedDate", "ModifiedDate" },
                values: new object[] { new DateTime(2025, 12, 17, 15, 32, 41, 321, DateTimeKind.Local).AddTicks(9419), new DateTime(2025, 12, 17, 15, 32, 41, 321, DateTimeKind.Local).AddTicks(9421) });

            migrationBuilder.UpdateData(
                table: "Categories",
                keyColumn: "Id",
                keyValue: 3,
                columns: new[] { "CreatedDate", "ModifiedDate" },
                values: new object[] { new DateTime(2025, 12, 17, 15, 32, 41, 321, DateTimeKind.Local).AddTicks(9425), new DateTime(2025, 12, 17, 15, 32, 41, 321, DateTimeKind.Local).AddTicks(9427) });

            migrationBuilder.UpdateData(
                table: "Categories",
                keyColumn: "Id",
                keyValue: 4,
                columns: new[] { "CreatedDate", "ModifiedDate" },
                values: new object[] { new DateTime(2025, 12, 17, 15, 32, 41, 321, DateTimeKind.Local).AddTicks(9430), new DateTime(2025, 12, 17, 15, 32, 41, 321, DateTimeKind.Local).AddTicks(9432) });

            migrationBuilder.UpdateData(
                table: "Categories",
                keyColumn: "Id",
                keyValue: 5,
                columns: new[] { "CreatedDate", "ModifiedDate" },
                values: new object[] { new DateTime(2025, 12, 17, 15, 32, 41, 321, DateTimeKind.Local).AddTicks(9435), new DateTime(2025, 12, 17, 15, 32, 41, 321, DateTimeKind.Local).AddTicks(9437) });

            migrationBuilder.UpdateData(
                table: "Categories",
                keyColumn: "Id",
                keyValue: 6,
                columns: new[] { "CreatedDate", "ModifiedDate" },
                values: new object[] { new DateTime(2025, 12, 17, 15, 32, 41, 321, DateTimeKind.Local).AddTicks(9441), new DateTime(2025, 12, 17, 15, 32, 41, 321, DateTimeKind.Local).AddTicks(9442) });

            migrationBuilder.CreateIndex(
                name: "IX_BudgetNotifications_BudgetId",
                table: "BudgetNotifications",
                column: "BudgetId");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "BudgetNotifications");

            migrationBuilder.DropColumn(
                name: "LastResetDate",
                table: "Budgets");

            migrationBuilder.DropColumn(
                name: "NextResetDate",
                table: "Budgets");

            migrationBuilder.DropColumn(
                name: "NotifyOnExceed",
                table: "Budgets");

            migrationBuilder.DropColumn(
                name: "NotifyOnWarning",
                table: "Budgets");

            migrationBuilder.DropColumn(
                name: "WarningThreshold",
                table: "Budgets");

            migrationBuilder.AlterColumn<DateTime>(
                name: "EndDate",
                table: "Budgets",
                type: "datetime2",
                nullable: false,
                defaultValue: new DateTime(1, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified),
                oldClrType: typeof(DateTime),
                oldType: "datetime2",
                oldNullable: true);

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
    }
}
