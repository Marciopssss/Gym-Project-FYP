using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Gym_Membership.Migrations
{
    /// <inheritdoc />
    public partial class adddesctoclasses : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<string>(
                name: "Description",
                table: "Classes",
                type: "nvarchar(max)",
                nullable: false,
                defaultValue: "");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "Description",
                table: "Classes");
        }
    }
}
