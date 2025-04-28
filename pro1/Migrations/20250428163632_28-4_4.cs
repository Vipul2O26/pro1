using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace pro1.Migrations
{
    /// <inheritdoc />
    public partial class _284_4 : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<int>(
                name: "CreatedByUserID",
                table: "SubjectUnits",
                type: "int",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.CreateIndex(
                name: "IX_SubjectUnits_CreatedByUserID",
                table: "SubjectUnits",
                column: "CreatedByUserID");

            migrationBuilder.AddForeignKey(
                name: "FK_SubjectUnits_Users_CreatedByUserID",
                table: "SubjectUnits",
                column: "CreatedByUserID",
                principalTable: "Users",
                principalColumn: "UserID",
                onDelete: ReferentialAction.Cascade);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_SubjectUnits_Users_CreatedByUserID",
                table: "SubjectUnits");

            migrationBuilder.DropIndex(
                name: "IX_SubjectUnits_CreatedByUserID",
                table: "SubjectUnits");

            migrationBuilder.DropColumn(
                name: "CreatedByUserID",
                table: "SubjectUnits");
        }
    }
}
