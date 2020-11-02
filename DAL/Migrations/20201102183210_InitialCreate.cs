using Microsoft.EntityFrameworkCore.Migrations;

namespace DAL.Migrations
{
    public partial class InitialCreate : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "GameOptions",
                columns: table => new
                {
                    GameOptionId = table.Column<int>(nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    BoardWidth = table.Column<int>(nullable: false),
                    BoardHeight = table.Column<int>(nullable: false),
                    CanBoatsTouch = table.Column<int>(nullable: false),
                    MoveOnHit = table.Column<int>(nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_GameOptions", x => x.GameOptionId);
                });

            migrationBuilder.CreateTable(
                name: "GameSaveDatas",
                columns: table => new
                {
                    GameSaveDataId = table.Column<int>(nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    TimeStamp = table.Column<string>(nullable: true),
                    SerializedGameData = table.Column<string>(maxLength: 128, nullable: true),
                    GameOptionsId = table.Column<int>(nullable: false),
                    GameOptionId = table.Column<int>(nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_GameSaveDatas", x => x.GameSaveDataId);
                    table.ForeignKey(
                        name: "FK_GameSaveDatas_GameOptions_GameOptionId",
                        column: x => x.GameOptionId,
                        principalTable: "GameOptions",
                        principalColumn: "GameOptionId",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateIndex(
                name: "IX_GameSaveDatas_GameOptionId",
                table: "GameSaveDatas",
                column: "GameOptionId");
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "GameSaveDatas");

            migrationBuilder.DropTable(
                name: "GameOptions");
        }
    }
}
