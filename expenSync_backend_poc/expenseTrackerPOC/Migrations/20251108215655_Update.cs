using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace expenseTrackerPOC.Migrations
{
    /// <inheritdoc />
    public partial class Update : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<int>(
                name: "ModeOfPaymentId",
                table: "Transactions",
                type: "int",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.AddColumn<string>(
                name: "ReciverSenderName",
                table: "Transactions",
                type: "nvarchar(max)",
                nullable: false,
                defaultValue: "");

            migrationBuilder.CreateTable(
                name: "ModeOfPayments",
                columns: table => new
                {
                    ModeOfPaymentId = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    ModeOfPaymentName = table.Column<string>(type: "nvarchar(max)", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_ModeOfPayments", x => x.ModeOfPaymentId);
                });

            migrationBuilder.CreateIndex(
                name: "IX_Transactions_ModeOfPaymentId",
                table: "Transactions",
                column: "ModeOfPaymentId");

            migrationBuilder.AddForeignKey(
                name: "FK_Transactions_ModeOfPayments_ModeOfPaymentId",
                table: "Transactions",
                column: "ModeOfPaymentId",
                principalTable: "ModeOfPayments",
                principalColumn: "ModeOfPaymentId",
                onDelete: ReferentialAction.Cascade);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Transactions_ModeOfPayments_ModeOfPaymentId",
                table: "Transactions");

            migrationBuilder.DropTable(
                name: "ModeOfPayments");

            migrationBuilder.DropIndex(
                name: "IX_Transactions_ModeOfPaymentId",
                table: "Transactions");

            migrationBuilder.DropColumn(
                name: "ModeOfPaymentId",
                table: "Transactions");

            migrationBuilder.DropColumn(
                name: "ReciverSenderName",
                table: "Transactions");
        }
    }
}
