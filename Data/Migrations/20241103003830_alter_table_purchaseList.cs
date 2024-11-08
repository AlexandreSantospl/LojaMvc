using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace SetorDeCompras.Migrations
{
    /// <inheritdoc />
    public partial class alter_table_purchaseList : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<int>(
                name: "Quantidade",
                table: "PurchaseList",
                type: "integer",
                nullable: false,
                defaultValue: 0);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "Quantidade",
                table: "PurchaseList");
        }
    }
}
