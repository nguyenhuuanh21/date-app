using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace DateApp.Data.Migrations
{
    /// <inheritdoc />
    public partial class likeEntity : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "MemberLikes",
                columns: table => new
                {
                    SourceMemberId = table.Column<string>(type: "text", nullable: false),
                    TargetMemberId = table.Column<string>(type: "text", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_MemberLikes", x => new { x.SourceMemberId, x.TargetMemberId });
                    table.ForeignKey(
                        name: "FK_MemberLikes_Members_SourceMemberId",
                        column: x => x.SourceMemberId,
                        principalTable: "Members",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_MemberLikes_Members_TargetMemberId",
                        column: x => x.TargetMemberId,
                        principalTable: "Members",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateIndex(
                name: "IX_MemberLikes_TargetMemberId",
                table: "MemberLikes",
                column: "TargetMemberId");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "MemberLikes");
        }
    }
}
