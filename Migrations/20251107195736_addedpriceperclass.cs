using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Gym_Membership.Migrations
{
    /// <inheritdoc />
    public partial class addedpriceperclass : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<decimal>(
                name: "PricePerSession",
                table: "Classes",
                type: "decimal(18,2)",
                nullable: false,
                defaultValue: 0m);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "PricePerSession",
                table: "Classes");
        }
    }
}
