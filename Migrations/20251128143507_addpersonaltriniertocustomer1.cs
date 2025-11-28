using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Gym_Membership.Migrations
{
    /// <inheritdoc />
    public partial class addpersonaltriniertocustomer1 : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Customers_Users_UserId",
                table: "Customers");

            migrationBuilder.DropForeignKey(
                name: "FK_Subscriptions_Customers_CustomerID",
                table: "Subscriptions");

            migrationBuilder.DropForeignKey(
                name: "FK_Subscriptions_Memberships_MembershipID",
                table: "Subscriptions");

            migrationBuilder.AddColumn<int>(
                name: "MembershipID1",
                table: "Subscriptions",
                type: "int",
                nullable: true);

            migrationBuilder.AddColumn<int>(
                name: "PersonalTrainerId",
                table: "Customers",
                type: "int",
                nullable: true);

            migrationBuilder.AddColumn<DateTime>(
                name: "PersonalTrainingTime",
                table: "Customers",
                type: "datetime2",
                nullable: true);

            migrationBuilder.AddColumn<int>(
                name: "UserId1",
                table: "Customers",
                type: "int",
                nullable: true);

            migrationBuilder.CreateIndex(
                name: "IX_Subscriptions_MembershipID1",
                table: "Subscriptions",
                column: "MembershipID1");

            migrationBuilder.CreateIndex(
                name: "IX_Customers_PersonalTrainerId",
                table: "Customers",
                column: "PersonalTrainerId");

            migrationBuilder.CreateIndex(
                name: "IX_Customers_UserId1",
                table: "Customers",
                column: "UserId1",
                unique: true,
                filter: "[UserId1] IS NOT NULL");

            migrationBuilder.AddForeignKey(
                name: "FK_Customers_Users_PersonalTrainerId",
                table: "Customers",
                column: "PersonalTrainerId",
                principalTable: "Users",
                principalColumn: "UserId",
                onDelete: ReferentialAction.SetNull);

            migrationBuilder.AddForeignKey(
                name: "FK_Customers_Users_UserId",
                table: "Customers",
                column: "UserId",
                principalTable: "Users",
                principalColumn: "UserId",
                onDelete: ReferentialAction.Restrict);

            migrationBuilder.AddForeignKey(
                name: "FK_Customers_Users_UserId1",
                table: "Customers",
                column: "UserId1",
                principalTable: "Users",
                principalColumn: "UserId");

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

            migrationBuilder.AddForeignKey(
                name: "FK_Subscriptions_Memberships_MembershipID1",
                table: "Subscriptions",
                column: "MembershipID1",
                principalTable: "Memberships",
                principalColumn: "MembershipID");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Customers_Users_PersonalTrainerId",
                table: "Customers");

            migrationBuilder.DropForeignKey(
                name: "FK_Customers_Users_UserId",
                table: "Customers");

            migrationBuilder.DropForeignKey(
                name: "FK_Customers_Users_UserId1",
                table: "Customers");

            migrationBuilder.DropForeignKey(
                name: "FK_Subscriptions_Customers_CustomerID",
                table: "Subscriptions");

            migrationBuilder.DropForeignKey(
                name: "FK_Subscriptions_Memberships_MembershipID",
                table: "Subscriptions");

            migrationBuilder.DropForeignKey(
                name: "FK_Subscriptions_Memberships_MembershipID1",
                table: "Subscriptions");

            migrationBuilder.DropIndex(
                name: "IX_Subscriptions_MembershipID1",
                table: "Subscriptions");

            migrationBuilder.DropIndex(
                name: "IX_Customers_PersonalTrainerId",
                table: "Customers");

            migrationBuilder.DropIndex(
                name: "IX_Customers_UserId1",
                table: "Customers");

            migrationBuilder.DropColumn(
                name: "MembershipID1",
                table: "Subscriptions");

            migrationBuilder.DropColumn(
                name: "PersonalTrainerId",
                table: "Customers");

            migrationBuilder.DropColumn(
                name: "PersonalTrainingTime",
                table: "Customers");

            migrationBuilder.DropColumn(
                name: "UserId1",
                table: "Customers");

            migrationBuilder.AddForeignKey(
                name: "FK_Customers_Users_UserId",
                table: "Customers",
                column: "UserId",
                principalTable: "Users",
                principalColumn: "UserId",
                onDelete: ReferentialAction.Cascade);

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
    }
}
