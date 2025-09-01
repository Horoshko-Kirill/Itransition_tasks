using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace CourseWork.Migrations
{
    /// <inheritdoc />
    public partial class Updatе : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<string>(
                name: "FixedValue",
                table: "CustomIdElements",
                type: "text",
                nullable: true);

            migrationBuilder.AddColumn<int>(
                name: "Order",
                table: "CustomIdElements",
                type: "integer",
                nullable: false,
                defaultValue: 0);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "FixedValue",
                table: "CustomIdElements");

            migrationBuilder.DropColumn(
                name: "Order",
                table: "CustomIdElements");
        }
    }
}
