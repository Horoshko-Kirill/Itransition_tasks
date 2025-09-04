using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace CourseWork.Migrations
{
    /// <inheritdoc />
    public partial class AddTags : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_InventoryTag_Inventories_InventoryId",
                table: "InventoryTag");

            migrationBuilder.DropForeignKey(
                name: "FK_InventoryTag_Tag_TagId",
                table: "InventoryTag");

            migrationBuilder.DropForeignKey(
                name: "FK_Reviews_Items_ItemId",
                table: "Reviews");

            migrationBuilder.DropIndex(
                name: "IX_Reviews_ItemId",
                table: "Reviews");

            migrationBuilder.DropPrimaryKey(
                name: "PK_Tag",
                table: "Tag");

            migrationBuilder.DropPrimaryKey(
                name: "PK_InventoryTag",
                table: "InventoryTag");

            migrationBuilder.DropColumn(
                name: "ItemId",
                table: "Reviews");

            migrationBuilder.RenameTable(
                name: "Tag",
                newName: "Tags");

            migrationBuilder.RenameTable(
                name: "InventoryTag",
                newName: "inventoryTags");

            migrationBuilder.RenameIndex(
                name: "IX_InventoryTag_TagId",
                table: "inventoryTags",
                newName: "IX_inventoryTags_TagId");

            migrationBuilder.RenameIndex(
                name: "IX_InventoryTag_InventoryId",
                table: "inventoryTags",
                newName: "IX_inventoryTags_InventoryId");

            migrationBuilder.AddPrimaryKey(
                name: "PK_Tags",
                table: "Tags",
                column: "Id");

            migrationBuilder.AddPrimaryKey(
                name: "PK_inventoryTags",
                table: "inventoryTags",
                column: "Id");

            migrationBuilder.AddForeignKey(
                name: "FK_inventoryTags_Inventories_InventoryId",
                table: "inventoryTags",
                column: "InventoryId",
                principalTable: "Inventories",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_inventoryTags_Tags_TagId",
                table: "inventoryTags",
                column: "TagId",
                principalTable: "Tags",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_inventoryTags_Inventories_InventoryId",
                table: "inventoryTags");

            migrationBuilder.DropForeignKey(
                name: "FK_inventoryTags_Tags_TagId",
                table: "inventoryTags");

            migrationBuilder.DropPrimaryKey(
                name: "PK_Tags",
                table: "Tags");

            migrationBuilder.DropPrimaryKey(
                name: "PK_inventoryTags",
                table: "inventoryTags");

            migrationBuilder.RenameTable(
                name: "Tags",
                newName: "Tag");

            migrationBuilder.RenameTable(
                name: "inventoryTags",
                newName: "InventoryTag");

            migrationBuilder.RenameIndex(
                name: "IX_inventoryTags_TagId",
                table: "InventoryTag",
                newName: "IX_InventoryTag_TagId");

            migrationBuilder.RenameIndex(
                name: "IX_inventoryTags_InventoryId",
                table: "InventoryTag",
                newName: "IX_InventoryTag_InventoryId");

            migrationBuilder.AddColumn<int>(
                name: "ItemId",
                table: "Reviews",
                type: "integer",
                nullable: true);

            migrationBuilder.AddPrimaryKey(
                name: "PK_Tag",
                table: "Tag",
                column: "Id");

            migrationBuilder.AddPrimaryKey(
                name: "PK_InventoryTag",
                table: "InventoryTag",
                column: "Id");

            migrationBuilder.CreateIndex(
                name: "IX_Reviews_ItemId",
                table: "Reviews",
                column: "ItemId");

            migrationBuilder.AddForeignKey(
                name: "FK_InventoryTag_Inventories_InventoryId",
                table: "InventoryTag",
                column: "InventoryId",
                principalTable: "Inventories",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_InventoryTag_Tag_TagId",
                table: "InventoryTag",
                column: "TagId",
                principalTable: "Tag",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_Reviews_Items_ItemId",
                table: "Reviews",
                column: "ItemId",
                principalTable: "Items",
                principalColumn: "Id");
        }
    }
}
