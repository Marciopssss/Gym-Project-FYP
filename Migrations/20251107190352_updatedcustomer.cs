using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Gym_Membership.Migrations
{
    /// <inheritdoc />
    public partial class updatedcustomer : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<int>(
                name: "CustomerID1",
                table: "Subscriptions",
                type: "int",
                nullable: true);

            migrationBuilder.CreateIndex(
                name: "IX_Subscriptions_CustomerID1",
                table: "Subscriptions",
                column: "CustomerID1");

            migrationBuilder.AddForeignKey(
                name: "FK_Subscriptions_Customers_CustomerID1",
                table: "Subscriptions",
                column: "CustomerID1",
                principalTable: "Customers",
                principalColumn: "CustomerID");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Subscriptions_Customers_CustomerID1",
                table: "Subscriptions");

            migrationBuilder.DropIndex(
                name: "IX_Subscriptions_CustomerID1",
                table: "Subscriptions");

            migrationBuilder.DropColumn(
                name: "CustomerID1",
                table: "Subscriptions");
        }
    }
}
