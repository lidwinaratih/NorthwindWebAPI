using Microsoft.EntityFrameworkCore.Migrations;

namespace Northwind.Entities.Migrations
{
    public partial class AddedRolesToDb : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.InsertData(
                table: "AspNetRoles",
                columns: new[] { "Id", "ConcurrencyStamp", "Name", "NormalizedName" },
                values: new object[] { "1c6767b4-a821-42b0-b5de-0cee18fc3cfa", "6137e673-979a-4504-8aea-e47e17c6c6ec", "Manager", "MANAGER" });

            migrationBuilder.InsertData(
                table: "AspNetRoles",
                columns: new[] { "Id", "ConcurrencyStamp", "Name", "NormalizedName" },
                values: new object[] { "b2c3dd40-9de8-4f59-a35a-a5314f06cda9", "b3e61797-3203-404d-893c-6ad32cab630f", "Administrator", "ADMINISTRATOR" });
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DeleteData(
                table: "AspNetRoles",
                keyColumn: "Id",
                keyValue: "1c6767b4-a821-42b0-b5de-0cee18fc3cfa");

            migrationBuilder.DeleteData(
                table: "AspNetRoles",
                keyColumn: "Id",
                keyValue: "b2c3dd40-9de8-4f59-a35a-a5314f06cda9");
        }
    }
}
