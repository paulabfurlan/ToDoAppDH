using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace ToDoApp.API.Migrations
{
    /// <inheritdoc />
    public partial class SeedingdataforUsers : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.InsertData(
                table: "Users",
                columns: new[] { "Id", "Email", "LastName", "Name" },
                values: new object[] { new Guid("b21d8b6d-3242-4c5e-8fa9-c032a60a91a4"), "email@test.com", "Furlan", "Paula" });
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DeleteData(
                table: "Users",
                keyColumn: "Id",
                keyValue: new Guid("b21d8b6d-3242-4c5e-8fa9-c032a60a91a4"));
        }
    }
}
