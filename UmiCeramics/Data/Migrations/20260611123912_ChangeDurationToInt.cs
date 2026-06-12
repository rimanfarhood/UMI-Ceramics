using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace UmiCeramics.Migrations
{
    /// <inheritdoc />
    public partial class ChangeDurationToInt : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
            name: "Duration",
            table: "Workshops");

            migrationBuilder.AddColumn<int>(
                name: "Duration",
                table: "Workshops",
                type: "int",
                nullable: false,
                defaultValue: 0);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AlterColumn<TimeSpan>(
                name: "Duration",
                table: "Workshops",
                type: "time",
                nullable: false,
                oldClrType: typeof(int),
                oldType: "int");
        }
    }
}
