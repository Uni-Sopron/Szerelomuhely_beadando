using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace SzereloMuhely.Migrations
{
    /// <inheritdoc />
    public partial class UpdateModels : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.RenameColumn(
                name: "Status",
                table: "WorkSheets",
                newName: "IsOpen");

            migrationBuilder.RenameColumn(
                name: "RecruiterName",
                table: "WorkSheets",
                newName: "RecruiterId");

            migrationBuilder.AlterColumn<string>(
                name: "MechanicID",
                table: "WorkSheets",
                type: "nvarchar(450)",
                nullable: false,
                oldClrType: typeof(string),
                oldType: "nvarchar(max)");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.RenameColumn(
                name: "RecruiterId",
                table: "WorkSheets",
                newName: "RecruiterName");

            migrationBuilder.RenameColumn(
                name: "IsOpen",
                table: "WorkSheets",
                newName: "Status");

            migrationBuilder.AlterColumn<string>(
                name: "MechanicID",
                table: "WorkSheets",
                type: "nvarchar(max)",
                nullable: false,
                oldClrType: typeof(string),
                oldType: "nvarchar(450)");
        }
    }
}
