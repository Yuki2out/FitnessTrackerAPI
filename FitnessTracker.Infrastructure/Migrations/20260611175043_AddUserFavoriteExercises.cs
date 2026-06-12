using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace FitnessTracker.Infrastructure.Migrations
{
    /// <inheritdoc />
    public partial class AddUserFavoriteExercises : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "UserFavoriteExercises",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    UserId = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    ExerciseId = table.Column<int>(type: "int", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_UserFavoriteExercises", x => x.Id);
                    table.ForeignKey(
                        name: "FK_UserFavoriteExercises_Exercises_ExerciseId",
                        column: x => x.ExerciseId,
                        principalTable: "Exercises",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.UpdateData(
                table: "AspNetRoles",
                keyColumn: "Id",
                keyValue: "admin-role-id-111",
                column: "ConcurrencyStamp",
                value: "65292f58-df3d-4fbc-8e0a-037268e97962");

            migrationBuilder.UpdateData(
                table: "AspNetRoles",
                keyColumn: "Id",
                keyValue: "user-role-id-222",
                column: "ConcurrencyStamp",
                value: "fcbb934a-b550-4fad-a989-bb1f9c8019e8");

            migrationBuilder.CreateIndex(
                name: "IX_UserFavoriteExercises_ExerciseId",
                table: "UserFavoriteExercises",
                column: "ExerciseId");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "UserFavoriteExercises");

            migrationBuilder.UpdateData(
                table: "AspNetRoles",
                keyColumn: "Id",
                keyValue: "admin-role-id-111",
                column: "ConcurrencyStamp",
                value: "6c273598-ea46-426a-8260-962f82a5a50f");

            migrationBuilder.UpdateData(
                table: "AspNetRoles",
                keyColumn: "Id",
                keyValue: "user-role-id-222",
                column: "ConcurrencyStamp",
                value: "f06e0e68-e1eb-4608-b7d4-1c5fc11f72b4");
        }
    }
}
