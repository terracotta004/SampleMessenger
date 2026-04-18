using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace MauiMessenger.Infrastructure.Migrations
{
    /// <inheritdoc />
    public partial class AddParticipantTypeToUsers : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<int>(
                name: "ParticipantType",
                table: "Users",
                type: "integer",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.Sql("""UPDATE "Users" SET "ParticipantType" = 0;""");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "ParticipantType",
                table: "Users");
        }
    }
}
