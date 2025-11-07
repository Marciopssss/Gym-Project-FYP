using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Gym_Membership.Migrations
{
    /// <inheritdoc />
    public partial class useremailsubscription : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<string>(
                name: "UserEmail",
                table: "Subscriptions",
                type: "nvarchar(max)",
                nullable: false,
                defaultValue: "");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "UserEmail",
                table: "Subscriptions");
        }
    }
}
