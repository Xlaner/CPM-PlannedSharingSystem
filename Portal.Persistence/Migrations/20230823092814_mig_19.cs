using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Portal.Persistence.Migrations
{
    /// <inheritdoc />
    public partial class mig_19 : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "HepsiniSil",
                table: "etkinliks");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<bool>(
                name: "HepsiniSil",
                table: "etkinliks",
                type: "bit",
                nullable: false,
                defaultValue: false);
        }
    }
}
