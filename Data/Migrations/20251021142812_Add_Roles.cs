using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Midyaf.Data.Migrations
{
    /// <inheritdoc />
    public partial class Add_Roles : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<string>(
                name: "ManagerId",
                table: "Propertys",
                type: "text",
                nullable: false,
                defaultValue: "");

            migrationBuilder.AddColumn<string>(
                name: "Role",
                table: "AspNetUsers",
                type: "text",
                nullable: true);

            migrationBuilder.CreateIndex(
                name: "IX_Propertys_ManagerId",
                table: "Propertys",
                column: "ManagerId");

            migrationBuilder.AddForeignKey(
                name: "FK_Propertys_AspNetUsers_ManagerId",
                table: "Propertys",
                column: "ManagerId",
                principalTable: "AspNetUsers",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Propertys_AspNetUsers_ManagerId",
                table: "Propertys");

            migrationBuilder.DropIndex(
                name: "IX_Propertys_ManagerId",
                table: "Propertys");

            migrationBuilder.DropColumn(
                name: "ManagerId",
                table: "Propertys");

            migrationBuilder.DropColumn(
                name: "Role",
                table: "AspNetUsers");
        }
    }
}
