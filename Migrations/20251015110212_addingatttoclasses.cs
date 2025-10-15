using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Gym_Membership.Migrations
{
    /// <inheritdoc />
    public partial class addingatttoclasses : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_ClassesCustomer_Customers_CustomersCustomerID",
                table: "ClassesCustomer");

            migrationBuilder.DropPrimaryKey(
                name: "PK_ClassesCustomer",
                table: "ClassesCustomer");

            migrationBuilder.DropIndex(
                name: "IX_ClassesCustomer_CustomersCustomerID",
                table: "ClassesCustomer");

            migrationBuilder.RenameColumn(
                name: "CustomersCustomerID",
                table: "ClassesCustomer",
                newName: "ClassID");

            migrationBuilder.AddColumn<int>(
                name: "ClassID",
                table: "Customers",
                type: "int",
                nullable: true);

            migrationBuilder.AddPrimaryKey(
                name: "PK_ClassesCustomer",
                table: "ClassesCustomer",
                columns: new[] { "ClassID", "ClassesClassID" });

            migrationBuilder.CreateIndex(
                name: "IX_ClassesCustomer_ClassesClassID",
                table: "ClassesCustomer",
                column: "ClassesClassID");

            migrationBuilder.AddForeignKey(
                name: "FK_ClassesCustomer_Customers_ClassID",
                table: "ClassesCustomer",
                column: "ClassID",
                principalTable: "Customers",
                principalColumn: "CustomerID",
                onDelete: ReferentialAction.Cascade);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_ClassesCustomer_Customers_ClassID",
                table: "ClassesCustomer");

            migrationBuilder.DropPrimaryKey(
                name: "PK_ClassesCustomer",
                table: "ClassesCustomer");

            migrationBuilder.DropIndex(
                name: "IX_ClassesCustomer_ClassesClassID",
                table: "ClassesCustomer");

            migrationBuilder.DropColumn(
                name: "ClassID",
                table: "Customers");

            migrationBuilder.RenameColumn(
                name: "ClassID",
                table: "ClassesCustomer",
                newName: "CustomersCustomerID");

            migrationBuilder.AddPrimaryKey(
                name: "PK_ClassesCustomer",
                table: "ClassesCustomer",
                columns: new[] { "ClassesClassID", "CustomersCustomerID" });

            migrationBuilder.CreateIndex(
                name: "IX_ClassesCustomer_CustomersCustomerID",
                table: "ClassesCustomer",
                column: "CustomersCustomerID");

            migrationBuilder.AddForeignKey(
                name: "FK_ClassesCustomer_Customers_CustomersCustomerID",
                table: "ClassesCustomer",
                column: "CustomersCustomerID",
                principalTable: "Customers",
                principalColumn: "CustomerID",
                onDelete: ReferentialAction.Cascade);
        }
    }
}
