using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace BiblioServer.Migrations
{
    /// <inheritdoc />
    public partial class ActivitiesTime : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.RenameColumn(
                name: "Joined",
                table: "Activities",
                newName: "Time");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.RenameColumn(
                name: "Time",
                table: "Activities",
                newName: "Joined");
        }
    }
}
