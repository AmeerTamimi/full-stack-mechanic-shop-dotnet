using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace GOATY.Infrastructure.Data.Migrations
{
    /// <inheritdoc />
    public partial class AddWorkOrder : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "WorkOrders",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    State = table.Column<int>(type: "int", nullable: false),
                    TotalTime = table.Column<int>(type: "int", nullable: false),
                    TotalCost = table.Column<decimal>(type: "decimal(18,2)", nullable: false),
                    StartTime = table.Column<DateTime>(type: "datetime2", nullable: false),
                    Bay = table.Column<int>(type: "int", nullable: false),
                    VehicleId = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    CustomerId = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    EmployeeId = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    CreatedAtUtc = table.Column<DateTimeOffset>(type: "datetimeoffset", nullable: false),
                    CreatedBy = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    LastModifiedUtc = table.Column<DateTimeOffset>(type: "datetimeoffset", nullable: false),
                    LastModifiedBy = table.Column<string>(type: "nvarchar(max)", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_WorkOrders", x => x.Id);
                    table.ForeignKey(
                        name: "FK_WorkOrders_Customers_CustomerId",
                        column: x => x.CustomerId,
                        principalTable: "Customers",
                        principalColumn: "Id");
                    table.ForeignKey(
                        name: "FK_WorkOrders_Employees_EmployeeId",
                        column: x => x.EmployeeId,
                        principalTable: "Employees",
                        principalColumn: "Id");
                    table.ForeignKey(
                        name: "FK_WorkOrders_Vehicles_VehicleId",
                        column: x => x.VehicleId,
                        principalTable: "Vehicles",
                        principalColumn: "Id");
                });

            migrationBuilder.CreateTable(
                name: "WorkOrderRepairTasks",
                columns: table => new
                {
                    WorkOrderId = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    RepairTaskId = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    Time = table.Column<int>(type: "int", nullable: false),
                    Cost = table.Column<decimal>(type: "decimal(18,2)", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_WorkOrderRepairTasks", x => new { x.WorkOrderId, x.RepairTaskId });
                    table.ForeignKey(
                        name: "FK_WorkOrderRepairTasks_RepairTasks_RepairTaskId",
                        column: x => x.RepairTaskId,
                        principalTable: "RepairTasks",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_WorkOrderRepairTasks_WorkOrders_WorkOrderId",
                        column: x => x.WorkOrderId,
                        principalTable: "WorkOrders",
                        principalColumn: "Id");
                });

            migrationBuilder.CreateIndex(
                name: "IX_WorkOrderRepairTasks_RepairTaskId",
                table: "WorkOrderRepairTasks",
                column: "RepairTaskId");

            migrationBuilder.CreateIndex(
                name: "IX_WorkOrders_CustomerId",
                table: "WorkOrders",
                column: "CustomerId");

            migrationBuilder.CreateIndex(
                name: "IX_WorkOrders_EmployeeId",
                table: "WorkOrders",
                column: "EmployeeId");

            migrationBuilder.CreateIndex(
                name: "IX_WorkOrders_VehicleId",
                table: "WorkOrders",
                column: "VehicleId");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "WorkOrderRepairTasks");

            migrationBuilder.DropTable(
                name: "WorkOrders");
        }
    }
}
