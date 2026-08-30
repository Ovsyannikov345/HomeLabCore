using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace HomeLabCore.Infrastructure.Database.Migrations
{
    /// <inheritdoc />
    public partial class AddChatIdToMediaSubscription : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<long>(
                name: "ChatId",
                table: "MediaSubscriptions",
                type: "bigint",
                nullable: false,
                defaultValue: 0L);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "ChatId",
                table: "MediaSubscriptions");
        }
    }
}
