using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace PollsAppBlazor.Server.Migrations
{
    /// <inheritdoc />
    public partial class PollExpiryDate : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<DateTimeOffset>(
                name: "ExpiryDate",
                table: "Polls",
                type: "datetimeoffset",
                nullable: true);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "ExpiryDate",
                table: "Polls");
        }
    }
}
