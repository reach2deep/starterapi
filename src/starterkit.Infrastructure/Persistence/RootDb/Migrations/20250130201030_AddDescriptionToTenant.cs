using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace starterkit.Infrastructure.Persistence.RootDb.Migrations
{
    /// <inheritdoc />
    public partial class AddDescriptionToTenant : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<string>(
                name: "Description",
                table: "Tenants",
                type: "nvarchar(500)",
                maxLength: 500,
                nullable: false,
                defaultValue: "");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "Description",
                table: "Tenants");
        }
    }
}
