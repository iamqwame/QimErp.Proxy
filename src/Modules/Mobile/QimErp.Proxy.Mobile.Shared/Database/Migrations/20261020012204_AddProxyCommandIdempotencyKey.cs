using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace QimErp.Proxy.Mobile.Shared.Database.Migrations
{
    /// <inheritdoc />
    public partial class AddProxyCommandIdempotencyKey : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<string>(
                name: "IdempotencyKey",
                table: "ProxyCommands",
                type: "character varying(128)",
                maxLength: 128,
                nullable: false,
                defaultValue: "");

            migrationBuilder.CreateIndex(
                name: "UX_ProxyCommands_TenantId_IdempotencyKey",
                table: "ProxyCommands",
                columns: new[] { "TenantId", "IdempotencyKey" },
                unique: true);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropIndex(
                name: "UX_ProxyCommands_TenantId_IdempotencyKey",
                table: "ProxyCommands");

            migrationBuilder.DropColumn(
                name: "IdempotencyKey",
                table: "ProxyCommands");
        }
    }
}
