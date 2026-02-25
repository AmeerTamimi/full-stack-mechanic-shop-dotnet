using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace GOATY.Infrastructure.Data.Migrations
{
    /// <inheritdoc />
    public partial class AddRepairTask : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AlterColumn<string>(
                name: "Name",
                table: "Parts",
                type: "nvarchar(450)",
                nullable: true,
                oldClrType: typeof(string),
                oldType: "nvarchar(max)",
                oldNullable: true);

            migrationBuilder.CreateTable(
                name: "RepairTasks",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    Name = table.Column<string>(type: "nvarchar(450)", nullable: true),
                    Description = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    TimeEstimated = table.Column<decimal>(type: "decimal(18,2)", nullable: false),
                    CostEstimated = table.Column<decimal>(type: "decimal(18,2)", nullable: false),
                    CreatedAtUtc = table.Column<DateTimeOffset>(type: "datetimeoffset", nullable: false),
                    CreatedBy = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    LastModifiedUtc = table.Column<DateTimeOffset>(type: "datetimeoffset", nullable: false),
                    LastModifiedBy = table.Column<string>(type: "nvarchar(max)", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_RepairTasks", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "RepairTaskDetails",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    RepairTaskId = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    PartId = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    Quantity = table.Column<int>(type: "int", nullable: false),
                    UnitPrice = table.Column<decimal>(type: "decimal(18,2)", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_RepairTaskDetails", x => x.Id);
                    table.ForeignKey(
                        name: "FK_RepairTaskDetails_Parts_PartId",
                        column: x => x.PartId,
                        principalTable: "Parts",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_RepairTaskDetails_RepairTasks_RepairTaskId",
                        column: x => x.RepairTaskId,
                        principalTable: "RepairTasks",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateIndex(
                name: "IX_Parts_Name",
                table: "Parts",
                column: "Name");

            migrationBuilder.CreateIndex(
                name: "IX_RepairTaskDetails_PartId",
                table: "RepairTaskDetails",
                column: "PartId");

            migrationBuilder.CreateIndex(
                name: "IX_RepairTaskDetails_RepairTaskId",
                table: "RepairTaskDetails",
                column: "RepairTaskId");

            migrationBuilder.CreateIndex(
                name: "IX_RepairTasks_Name",
                table: "RepairTasks",
                column: "Name");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "RepairTaskDetails");

            migrationBuilder.DropTable(
                name: "RepairTasks");

            migrationBuilder.DropIndex(
                name: "IX_Parts_Name",
                table: "Parts");

            migrationBuilder.AlterColumn<string>(
                name: "Name",
                table: "Parts",
                type: "nvarchar(max)",
                nullable: true,
                oldClrType: typeof(string),
                oldType: "nvarchar(450)",
                oldNullable: true);
        }
    }
}
