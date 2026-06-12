using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace FitnessTracker.Infrastructure.Migrations
{
    /// <inheritdoc />
    public partial class AddPastSessionIdToTemplate : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<int>(
                name: "PastWorkoutSessionId",
                table: "WorkoutTemplates",
                type: "int",
                nullable: true);

            migrationBuilder.UpdateData(
                table: "AspNetRoles",
                keyColumn: "Id",
                keyValue: "admin-role-id-111",
                column: "ConcurrencyStamp",
                value: "763c6342-b48c-48f8-9531-d366fedf3da3");

            migrationBuilder.UpdateData(
                table: "AspNetRoles",
                keyColumn: "Id",
                keyValue: "user-role-id-222",
                column: "ConcurrencyStamp",
                value: "b16c3274-df71-449c-8a84-fec3861e4ff7");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "PastWorkoutSessionId",
                table: "WorkoutTemplates");

            migrationBuilder.UpdateData(
                table: "AspNetRoles",
                keyColumn: "Id",
                keyValue: "admin-role-id-111",
                column: "ConcurrencyStamp",
                value: "670acfa6-444f-4702-9c65-1c28fa792318");

            migrationBuilder.UpdateData(
                table: "AspNetRoles",
                keyColumn: "Id",
                keyValue: "user-role-id-222",
                column: "ConcurrencyStamp",
                value: "9b2a9a6a-7f53-44b6-af34-b4aa9758de36");
        }
    }
}
