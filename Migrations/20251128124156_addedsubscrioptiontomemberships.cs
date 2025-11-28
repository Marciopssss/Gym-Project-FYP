using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Gym_Membership.Migrations
{
    /// <inheritdoc />
    public partial class addedsubscrioptiontomemberships : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Subscriptions_Customers_CustomerID",
                table: "Subscriptions");

            migrationBuilder.DropForeignKey(
                name: "FK_Subscriptions_Memberships_MembershipID",
                table: "Subscriptions");

            migrationBuilder.AddForeignKey(
                name: "FK_Subscriptions_Customers_CustomerID",
                table: "Subscriptions",
                column: "CustomerID",
                principalTable: "Customers",
                principalColumn: "CustomerID",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_Subscriptions_Memberships_MembershipID",
                table: "Subscriptions",
                column: "MembershipID",
                principalTable: "Memberships",
                principalColumn: "MembershipID",
                onDelete: ReferentialAction.Cascade);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Subscriptions_Customers_CustomerID",
                table: "Subscriptions");

            migrationBuilder.DropForeignKey(
                name: "FK_Subscriptions_Memberships_MembershipID",
                table: "Subscriptions");

            migrationBuilder.AddForeignKey(
                name: "FK_Subscriptions_Customers_CustomerID",
                table: "Subscriptions",
                column: "CustomerID",
                principalTable: "Customers",
                principalColumn: "CustomerID",
                onDelete: ReferentialAction.Restrict);

            migrationBuilder.AddForeignKey(
                name: "FK_Subscriptions_Memberships_MembershipID",
                table: "Subscriptions",
                column: "MembershipID",
                principalTable: "Memberships",
                principalColumn: "MembershipID",
                onDelete: ReferentialAction.Restrict);
        }
    }
}
