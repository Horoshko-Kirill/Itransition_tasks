using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace CourseWork.Migrations
{
    /// <inheritdoc />
    public partial class UpdateCustom2 : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<int>(
                name: "CustomFieldId1",
                table: "CustomFieldValues",
                type: "integer",
                nullable: true);

            migrationBuilder.CreateIndex(
                name: "IX_CustomFieldValues_CustomFieldId1",
                table: "CustomFieldValues",
                column: "CustomFieldId1");

            migrationBuilder.AddForeignKey(
                name: "FK_CustomFieldValues_CustomFields_CustomFieldId1",
                table: "CustomFieldValues",
                column: "CustomFieldId1",
                principalTable: "CustomFields",
                principalColumn: "Id");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_CustomFieldValues_CustomFields_CustomFieldId1",
                table: "CustomFieldValues");

            migrationBuilder.DropIndex(
                name: "IX_CustomFieldValues_CustomFieldId1",
                table: "CustomFieldValues");

            migrationBuilder.DropColumn(
                name: "CustomFieldId1",
                table: "CustomFieldValues");
        }
    }
}
