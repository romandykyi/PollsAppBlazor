using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace PollsAppBlazor.Server.Migrations
{
    /// <inheritdoc />
    public partial class CascadeBehavior : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Polls_AspNetUsers_CreatorId",
                table: "Polls");

            migrationBuilder.AddForeignKey(
                name: "FK_Polls_AspNetUsers_CreatorId",
                table: "Polls",
                column: "CreatorId",
                principalTable: "AspNetUsers",
                principalColumn: "Id");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Polls_AspNetUsers_CreatorId",
                table: "Polls");

            migrationBuilder.AddForeignKey(
                name: "FK_Polls_AspNetUsers_CreatorId",
                table: "Polls",
                column: "CreatorId",
                principalTable: "AspNetUsers",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);
        }
    }
}
