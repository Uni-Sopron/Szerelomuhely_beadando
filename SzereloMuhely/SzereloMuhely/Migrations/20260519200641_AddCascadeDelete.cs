using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace SzereloMuhely.Migrations
{
    /// <inheritdoc />
    public partial class AddCascadeDelete : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Vehicles_WorkSheets_WorkSheetID",
                table: "Vehicles");

            migrationBuilder.DropForeignKey(
                name: "FK_WorkItems_WorkItems_Material_WorkProcessID",
                table: "WorkItems");

            migrationBuilder.DropForeignKey(
                name: "FK_WorkItems_WorkItems_WorkProcessID",
                table: "WorkItems");

            migrationBuilder.DropForeignKey(
                name: "FK_WorkItems_WorkSheets_WorkSheetID",
                table: "WorkItems");

            migrationBuilder.AddForeignKey(
                name: "FK_Vehicles_WorkSheets_WorkSheetID",
                table: "Vehicles",
                column: "WorkSheetID",
                principalTable: "WorkSheets",
                principalColumn: "ID");

            migrationBuilder.AddForeignKey(
                name: "FK_WorkItems_WorkItems_Material_WorkProcessID",
                table: "WorkItems",
                column: "Material_WorkProcessID",
                principalTable: "WorkItems",
                principalColumn: "ID");

            migrationBuilder.AddForeignKey(
                name: "FK_WorkItems_WorkItems_WorkProcessID",
                table: "WorkItems",
                column: "WorkProcessID",
                principalTable: "WorkItems",
                principalColumn: "ID");

            migrationBuilder.AddForeignKey(
                name: "FK_WorkItems_WorkSheets_WorkSheetID",
                table: "WorkItems",
                column: "WorkSheetID",
                principalTable: "WorkSheets",
                principalColumn: "ID");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Vehicles_WorkSheets_WorkSheetID",
                table: "Vehicles");

            migrationBuilder.DropForeignKey(
                name: "FK_WorkItems_WorkItems_Material_WorkProcessID",
                table: "WorkItems");

            migrationBuilder.DropForeignKey(
                name: "FK_WorkItems_WorkItems_WorkProcessID",
                table: "WorkItems");

            migrationBuilder.DropForeignKey(
                name: "FK_WorkItems_WorkSheets_WorkSheetID",
                table: "WorkItems");

            migrationBuilder.AddForeignKey(
                name: "FK_Vehicles_WorkSheets_WorkSheetID",
                table: "Vehicles",
                column: "WorkSheetID",
                principalTable: "WorkSheets",
                principalColumn: "ID",
                onDelete: ReferentialAction.Restrict);

            migrationBuilder.AddForeignKey(
                name: "FK_WorkItems_WorkItems_Material_WorkProcessID",
                table: "WorkItems",
                column: "Material_WorkProcessID",
                principalTable: "WorkItems",
                principalColumn: "ID",
                onDelete: ReferentialAction.Restrict);

            migrationBuilder.AddForeignKey(
                name: "FK_WorkItems_WorkItems_WorkProcessID",
                table: "WorkItems",
                column: "WorkProcessID",
                principalTable: "WorkItems",
                principalColumn: "ID",
                onDelete: ReferentialAction.Restrict);

            migrationBuilder.AddForeignKey(
                name: "FK_WorkItems_WorkSheets_WorkSheetID",
                table: "WorkItems",
                column: "WorkSheetID",
                principalTable: "WorkSheets",
                principalColumn: "ID",
                onDelete: ReferentialAction.Restrict);
        }
    }
}
