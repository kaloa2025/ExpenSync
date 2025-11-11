using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace expenseTrackerPOC.Migrations
{
    /// <inheritdoc />
    public partial class UserUpdateForForgotPassword : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AlterColumn<DateTime>(
                name: "PasswordResetExpiry",
                table: "Users",
                type: "datetime2",
                nullable: true,
                oldClrType: typeof(int),
                oldType: "int",
                oldNullable: true);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AlterColumn<int>(
                name: "PasswordResetExpiry",
                table: "Users",
                type: "int",
                nullable: true,
                oldClrType: typeof(DateTime),
                oldType: "datetime2",
                oldNullable: true);
        }
    }
}
