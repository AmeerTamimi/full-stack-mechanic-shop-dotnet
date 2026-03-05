using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace GOATY.Infrastructure.Data.Migrations
{
    /// <inheritdoc />
    public partial class UpdateWorkOrder : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_WorkOrderRepairTasks_WorkOrders_WorkOrderId",
                table: "WorkOrderRepairTasks");

            migrationBuilder.AlterColumn<DateTimeOffset>(
                name: "StartTime",
                table: "WorkOrders",
                type: "datetimeoffset",
                nullable: false,
                oldClrType: typeof(DateTime),
                oldType: "datetime2");

            migrationBuilder.AddColumn<DateTimeOffset>(
                name: "EndTime",
                table: "WorkOrders",
                type: "datetimeoffset",
                nullable: false,
                defaultValue: new DateTimeOffset(new DateTime(1, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified), new TimeSpan(0, 0, 0, 0, 0)));

            migrationBuilder.AddForeignKey(
                name: "FK_WorkOrderRepairTasks_WorkOrders_WorkOrderId",
                table: "WorkOrderRepairTasks",
                column: "WorkOrderId",
                principalTable: "WorkOrders",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_WorkOrderRepairTasks_WorkOrders_WorkOrderId",
                table: "WorkOrderRepairTasks");

            migrationBuilder.DropColumn(
                name: "EndTime",
                table: "WorkOrders");

            migrationBuilder.AlterColumn<DateTime>(
                name: "StartTime",
                table: "WorkOrders",
                type: "datetime2",
                nullable: false,
                oldClrType: typeof(DateTimeOffset),
                oldType: "datetimeoffset");

            migrationBuilder.AddForeignKey(
                name: "FK_WorkOrderRepairTasks_WorkOrders_WorkOrderId",
                table: "WorkOrderRepairTasks",
                column: "WorkOrderId",
                principalTable: "WorkOrders",
                principalColumn: "Id");
        }
    }
}
