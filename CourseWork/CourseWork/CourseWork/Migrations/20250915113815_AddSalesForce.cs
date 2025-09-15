using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace CourseWork.Migrations
{
    /// <inheritdoc />
    public partial class AddSalesForce : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "SalesforceAccountId",
                table: "AspNetUsers");

            migrationBuilder.DropColumn(
                name: "SalesforceContactId",
                table: "AspNetUsers");

            migrationBuilder.AddColumn<bool>(
                name: "SalesForce",
                table: "AspNetUsers",
                type: "boolean",
                nullable: true);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "SalesForce",
                table: "AspNetUsers");

            migrationBuilder.AddColumn<string>(
                name: "SalesforceAccountId",
                table: "AspNetUsers",
                type: "text",
                nullable: false,
                defaultValue: "");

            migrationBuilder.AddColumn<string>(
                name: "SalesforceContactId",
                table: "AspNetUsers",
                type: "text",
                nullable: false,
                defaultValue: "");
        }
    }
}
