using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace EduSupport.web.Migrations
{
    /// <inheritdoc />
    public partial class AddPendingReason : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<int>(
                name: "PendingReason",
                table: "Tickets",
                type: "int",
                nullable: true);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "PendingReason",
                table: "Tickets");
        }
    }
}
