using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace EduSupport.web.Migrations
{
    /// <inheritdoc />
    public partial class AddFirstResponseSla : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<DateTime>(
                name: "FirstResponseDueAt",
                table: "Tickets",
                type: "datetime2",
                nullable: true);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "FirstResponseDueAt",
                table: "Tickets");
        }
    }
}
