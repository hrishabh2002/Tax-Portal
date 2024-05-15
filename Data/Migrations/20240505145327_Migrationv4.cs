using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace AspAssignment.Data.Migrations
{
    /// <inheritdoc />
    public partial class Migrationv4 : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<bool>(
                name: "Req",
                table: "IncomeTaxes",
                type: "bit",
                nullable: false,
                defaultValue: false);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "Req",
                table: "IncomeTaxes");
        }
    }
}
