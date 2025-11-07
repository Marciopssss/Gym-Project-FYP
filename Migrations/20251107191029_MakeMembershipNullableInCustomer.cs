using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Gym_Membership.Migrations
{
    /// <inheritdoc />
    public partial class MakeMembershipNullableInCustomer : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Customers_Memberships_MembershipID",
                table: "Customers");

            migrationBuilder.AlterColumn<int>(
                name: "MembershipID",
                table: "Customers",
                type: "int",
                nullable: true,
                oldClrType: typeof(int),
                oldType: "int");

            migrationBuilder.AddForeignKey(
                name: "FK_Customers_Memberships_MembershipID",
                table: "Customers",
                column: "MembershipID",
                principalTable: "Memberships",
                principalColumn: "MembershipID");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Customers_Memberships_MembershipID",
                table: "Customers");

            migrationBuilder.AlterColumn<int>(
                name: "MembershipID",
                table: "Customers",
                type: "int",
                nullable: false,
                defaultValue: 0,
                oldClrType: typeof(int),
                oldType: "int",
                oldNullable: true);

            migrationBuilder.AddForeignKey(
                name: "FK_Customers_Memberships_MembershipID",
                table: "Customers",
                column: "MembershipID",
                principalTable: "Memberships",
                principalColumn: "MembershipID",
                onDelete: ReferentialAction.Cascade);
        }
    }
}
