using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace HomeLabCore.Infrastructure.Database.Migrations
{
    /// <inheritdoc />
    public partial class AddMediaSubscriptions : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "MediaSubscriptions",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uuid", nullable: false),
                    UserId = table.Column<long>(type: "bigint", nullable: false),
                    MediaType = table.Column<int>(type: "integer", nullable: false),
                    MediaExternalId = table.Column<int>(type: "integer", nullable: false),
                    CreatedAt = table.Column<DateTimeOffset>(type: "timestamp with time zone", nullable: false),
                    UpdatedAt = table.Column<DateTimeOffset>(type: "timestamp with time zone", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_MediaSubscriptions", x => x.Id);
                });

            migrationBuilder.CreateIndex(
                name: "IX_MediaSubscriptions_MediaExternalId_MediaType",
                table: "MediaSubscriptions",
                columns: new[] { "MediaExternalId", "MediaType" });
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "MediaSubscriptions");
        }
    }
}
