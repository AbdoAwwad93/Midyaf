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
                table: "Hotels",
                type: "text",
                nullable: false,
                defaultValue: "");

            migrationBuilder.AddColumn<string>(
                name: "Role",
                table: "AspNetUsers",
                type: "text",
                nullable: true);

            migrationBuilder.CreateIndex(
                name: "IX_Hotels_ManagerId",
                table: "Hotels",
                column: "ManagerId");

            migrationBuilder.AddForeignKey(
                name: "FK_Hotels_AspNetUsers_ManagerId",
                table: "Hotels",
                column: "ManagerId",
                principalTable: "AspNetUsers",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Hotels_AspNetUsers_ManagerId",
                table: "Hotels");

            migrationBuilder.DropIndex(
                name: "IX_Hotels_ManagerId",
                table: "Hotels");

            migrationBuilder.DropColumn(
                name: "ManagerId",
                table: "Hotels");

            migrationBuilder.DropColumn(
                name: "Role",
                table: "AspNetUsers");
        }
    }
}
