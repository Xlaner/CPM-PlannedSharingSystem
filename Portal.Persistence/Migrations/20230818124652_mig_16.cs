using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Portal.Persistence.Migrations
{
    /// <inheritdoc />
    public partial class mig_16 : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.RenameColumn(
                name: "Api",
                table: "etkinliks",
                newName: "ApiTitle");

            migrationBuilder.AddColumn<string>(
                name: "ApiId",
                table: "etkinliks",
                type: "nvarchar(max)",
                nullable: true);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "ApiId",
                table: "etkinliks");

            migrationBuilder.RenameColumn(
                name: "ApiTitle",
                table: "etkinliks",
                newName: "Api");
        }
    }
}
