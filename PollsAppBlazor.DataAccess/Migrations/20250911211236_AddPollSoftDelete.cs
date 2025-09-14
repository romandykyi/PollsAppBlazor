﻿using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace PollsAppBlazor.Server.Migrations
{
    /// <inheritdoc />
    public partial class AddPollSoftDelete : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<bool>(
                name: "IsDeleted",
                table: "Polls",
                type: "bit",
                nullable: false,
                defaultValue: false);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "IsDeleted",
                table: "Polls");
        }
    }
}
