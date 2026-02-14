using System;
using Microsoft.EntityFrameworkCore.Migrations;

namespace DataLayer.Migrations
{
    public partial class ImproveEntities : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            #region Custom

            migrationBuilder.CreateIndex(
                name: "IX_Friendships_UserId1_UserId2",
                table: "Friendships",
                columns: new[] { "UserId1", "UserId2" },
                unique: true,
                filter: "[UserId1] IS NOT NULL AND [UserId2] IS NOT NULL");

            #endregion

            migrationBuilder.DropForeignKey(
                name: "FK_Attachments_AspNetUsers_UserId",
                table: "Attachments");

            migrationBuilder.DropForeignKey(
                name: "FK_Attachments_Posts_PostId",
                table: "Attachments");

            migrationBuilder.DropForeignKey(
                name: "FK_Friendships_AspNetUsers_UserId1",
                table: "Friendships");

            migrationBuilder.DropForeignKey(
                name: "FK_Friendships_AspNetUsers_UserId2",
                table: "Friendships");

            migrationBuilder.DropForeignKey(
                name: "FK_Posts_Attachments_FileId",
                table: "Posts");

            migrationBuilder.DropTable(
                name: "Dislikes");

            migrationBuilder.DropTable(
                name: "Likes");

            migrationBuilder.DropIndex(
                name: "IX_Friendships_UserId1_UserId2",
                table: "Friendships");

            migrationBuilder.DropIndex(
                name: "IX_Friendships_UserId2",
                table: "Friendships");

            migrationBuilder.DropCheckConstraint(
                name: "Friendship_UserId1_And_UserId2_CantBeEqual",
                table: "Friendships");

            migrationBuilder.DropIndex(
                name: "IX_Attachments_PostId",
                table: "Attachments");

            migrationBuilder.DropColumn(
                name: "PostId",
                table: "Attachments");

            migrationBuilder.DropColumn(
                name: "Avatar",
                table: "AspNetUsers");

            migrationBuilder.DropColumn(
                name: "Banned",
                table: "AspNetUsers");

            migrationBuilder.RenameColumn(
                name: "FileId",
                table: "Posts",
                newName: "ImageId");

            migrationBuilder.RenameIndex(
                name: "IX_Posts_FileId",
                table: "Posts",
                newName: "IX_Posts_ImageId");

            migrationBuilder.RenameColumn(
                name: "UserId2",
                table: "Friendships",
                newName: "RequesterId");

            migrationBuilder.RenameColumn(
                name: "UserId1",
                table: "Friendships",
                newName: "RecipientId");

            migrationBuilder.AlterColumn<string>(
                name: "Title",
                table: "Posts",
                type: "nvarchar(max)",
                nullable: true,
                oldClrType: typeof(string),
                oldType: "nvarchar(255)",
                oldMaxLength: 255,
                oldNullable: true);

            migrationBuilder.AddColumn<bool>(
                name: "IsEdited",
                table: "Posts",
                type: "bit",
                nullable: false,
                defaultValue: false);

            migrationBuilder.AddColumn<DateTime>(
                name: "LastInteractionDate",
                table: "Posts",
                type: "datetime2",
                nullable: true);

            migrationBuilder.AddColumn<bool>(
                name: "IsEdited",
                table: "Comments",
                type: "bit",
                nullable: false,
                defaultValue: false);

            migrationBuilder.AlterColumn<DateTime>(
                name: "BirthDate",
                table: "AspNetUsers",
                type: "datetime2",
                nullable: true,
                oldClrType: typeof(DateTime),
                oldType: "datetime2");

            migrationBuilder.AddColumn<int>(
                name: "AvatarId",
                table: "AspNetUsers",
                type: "int",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.AddColumn<int>(
                name: "Reputation",
                table: "AspNetUsers",
                type: "int",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.CreateTable(
                name: "Bans",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    Reason = table.Column<int>(type: "int", nullable: false),
                    StartDate = table.Column<DateTimeOffset>(type: "datetimeoffset", nullable: false),
                    EndDate = table.Column<DateTimeOffset>(type: "datetimeoffset", nullable: true),
                    UserId = table.Column<string>(type: "nvarchar(450)", nullable: true),
                    ModeratorId = table.Column<string>(type: "nvarchar(450)", nullable: true),
                    DateCreated = table.Column<DateTime>(type: "datetime2", nullable: false),
                    DateUpdated = table.Column<DateTime>(type: "datetime2", nullable: false),
                    DeletedAt = table.Column<DateTime>(type: "datetime2", nullable: true),
                    CreatedBy = table.Column<int>(type: "int", nullable: false),
                    UpdatedBy = table.Column<int>(type: "int", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Bans", x => x.Id);
                    table.CheckConstraint("CK_Ban_UserId_And_ModeratorId_CantBeEqual", "UserId != ModeratorId");
                    table.ForeignKey(
                        name: "FK_Bans_AspNetUsers_ModeratorId",
                        column: x => x.ModeratorId,
                        principalTable: "AspNetUsers",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_Bans_AspNetUsers_UserId",
                        column: x => x.UserId,
                        principalTable: "AspNetUsers",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "Reactions",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    Type = table.Column<int>(type: "int", nullable: true),
                    UserId = table.Column<string>(type: "nvarchar(450)", nullable: true),
                    CommentId = table.Column<int>(type: "int", nullable: false),
                    DateCreated = table.Column<DateTime>(type: "datetime2", nullable: false),
                    DateUpdated = table.Column<DateTime>(type: "datetime2", nullable: false),
                    DeletedAt = table.Column<DateTime>(type: "datetime2", nullable: true),
                    CreatedBy = table.Column<int>(type: "int", nullable: false),
                    UpdatedBy = table.Column<int>(type: "int", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Reactions", x => x.Id);
                    table.ForeignKey(
                        name: "FK_Reactions_AspNetUsers_UserId",
                        column: x => x.UserId,
                        principalTable: "AspNetUsers",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_Reactions_Comments_CommentId",
                        column: x => x.CommentId,
                        principalTable: "Comments",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.AddCheckConstraint(
                name: "CK_Warning_UserId_And_ModeratorId_CantBeEqual",
                table: "Warnings",
                sql: "UserId != ModeratorId");

            migrationBuilder.CreateIndex(
                name: "IX_Friendships_RecipientId",
                table: "Friendships",
                column: "RecipientId");

            migrationBuilder.CreateIndex(
                name: "IX_Friendships_RequesterId_RecipientId",
                table: "Friendships",
                columns: new[] { "RequesterId", "RecipientId" },
                unique: true,
                filter: "[RequesterId] IS NOT NULL AND [RecipientId] IS NOT NULL");

            migrationBuilder.AddCheckConstraint(
                name: "CK_Friendship_RequesterId_And_RecipientId_CantBeEqual",
                table: "Friendships",
                sql: "RequesterId != RecipientId");

            migrationBuilder.CreateIndex(
                name: "IX_AspNetUsers_AvatarId",
                table: "AspNetUsers",
                column: "AvatarId");

            migrationBuilder.CreateIndex(
                name: "IX_Bans_ModeratorId",
                table: "Bans",
                column: "ModeratorId");

            migrationBuilder.CreateIndex(
                name: "IX_Bans_UserId_ModeratorId",
                table: "Bans",
                columns: new[] { "UserId", "ModeratorId" },
                unique: true,
                filter: "[UserId] IS NOT NULL AND [ModeratorId] IS NOT NULL");

            migrationBuilder.CreateIndex(
                name: "IX_Reactions_CommentId",
                table: "Reactions",
                column: "CommentId");

            migrationBuilder.CreateIndex(
                name: "IX_Reactions_UserId_CommentId",
                table: "Reactions",
                columns: new[] { "UserId", "CommentId" },
                unique: true,
                filter: "[UserId] IS NOT NULL");

            migrationBuilder.AddForeignKey(
                name: "FK_AspNetUsers_Attachments_AvatarId",
                table: "AspNetUsers",
                column: "AvatarId",
                principalTable: "Attachments",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_Attachments_AspNetUsers_UserId",
                table: "Attachments",
                column: "UserId",
                principalTable: "AspNetUsers",
                principalColumn: "Id");

            migrationBuilder.AddForeignKey(
                name: "FK_Friendships_AspNetUsers_RecipientId",
                table: "Friendships",
                column: "RecipientId",
                principalTable: "AspNetUsers",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);

            migrationBuilder.AddForeignKey(
                name: "FK_Friendships_AspNetUsers_RequesterId",
                table: "Friendships",
                column: "RequesterId",
                principalTable: "AspNetUsers",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);

            migrationBuilder.AddForeignKey(
                name: "FK_Posts_Attachments_ImageId",
                table: "Posts",
                column: "ImageId",
                principalTable: "Attachments",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            #region Custom

            migrationBuilder.DropIndex(
                name: "IX_Friendships_UserId1_UserId2",
                table: "Friendships");

            #endregion

            migrationBuilder.DropForeignKey(
                name: "FK_AspNetUsers_Attachments_AvatarId",
                table: "AspNetUsers");

            migrationBuilder.DropForeignKey(
                name: "FK_Attachments_AspNetUsers_UserId",
                table: "Attachments");

            migrationBuilder.DropForeignKey(
                name: "FK_Friendships_AspNetUsers_RecipientId",
                table: "Friendships");

            migrationBuilder.DropForeignKey(
                name: "FK_Friendships_AspNetUsers_RequesterId",
                table: "Friendships");

            migrationBuilder.DropForeignKey(
                name: "FK_Posts_Attachments_ImageId",
                table: "Posts");

            migrationBuilder.DropTable(
                name: "Bans");

            migrationBuilder.DropTable(
                name: "Reactions");

            migrationBuilder.DropCheckConstraint(
                name: "CK_Warning_UserId_And_ModeratorId_CantBeEqual",
                table: "Warnings");

            migrationBuilder.DropIndex(
                name: "IX_Friendships_RecipientId",
                table: "Friendships");

            migrationBuilder.DropIndex(
                name: "IX_Friendships_RequesterId_RecipientId",
                table: "Friendships");

            migrationBuilder.DropCheckConstraint(
                name: "CK_Friendship_RequesterId_And_RecipientId_CantBeEqual",
                table: "Friendships");

            migrationBuilder.DropIndex(
                name: "IX_AspNetUsers_AvatarId",
                table: "AspNetUsers");

            migrationBuilder.DropColumn(
                name: "IsEdited",
                table: "Posts");

            migrationBuilder.DropColumn(
                name: "LastInteractionDate",
                table: "Posts");

            migrationBuilder.DropColumn(
                name: "IsEdited",
                table: "Comments");

            migrationBuilder.DropColumn(
                name: "AvatarId",
                table: "AspNetUsers");

            migrationBuilder.DropColumn(
                name: "Reputation",
                table: "AspNetUsers");

            migrationBuilder.RenameColumn(
                name: "ImageId",
                table: "Posts",
                newName: "FileId");

            migrationBuilder.RenameIndex(
                name: "IX_Posts_ImageId",
                table: "Posts",
                newName: "IX_Posts_FileId");

            migrationBuilder.RenameColumn(
                name: "RequesterId",
                table: "Friendships",
                newName: "UserId2");

            migrationBuilder.RenameColumn(
                name: "RecipientId",
                table: "Friendships",
                newName: "UserId1");

            migrationBuilder.AlterColumn<string>(
                name: "Title",
                table: "Posts",
                type: "nvarchar(255)",
                maxLength: 255,
                nullable: true,
                oldClrType: typeof(string),
                oldType: "nvarchar(max)",
                oldNullable: true);

            migrationBuilder.AddColumn<int>(
                name: "PostId",
                table: "Attachments",
                type: "int",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.AlterColumn<DateTime>(
                name: "BirthDate",
                table: "AspNetUsers",
                type: "datetime2",
                nullable: false,
                defaultValue: new DateTime(1, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified),
                oldClrType: typeof(DateTime),
                oldType: "datetime2",
                oldNullable: true);

            migrationBuilder.AddColumn<string>(
                name: "Avatar",
                table: "AspNetUsers",
                type: "nvarchar(max)",
                nullable: true);

            migrationBuilder.AddColumn<bool>(
                name: "Banned",
                table: "AspNetUsers",
                type: "bit",
                nullable: false,
                defaultValue: false);

            migrationBuilder.CreateTable(
                name: "Dislikes",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    CommentId = table.Column<int>(type: "int", nullable: false),
                    CreatedBy = table.Column<int>(type: "int", nullable: false),
                    DateCreated = table.Column<DateTime>(type: "datetime2", nullable: false),
                    DateUpdated = table.Column<DateTime>(type: "datetime2", nullable: false),
                    DeletedAt = table.Column<DateTime>(type: "datetime2", nullable: true),
                    UpdatedBy = table.Column<int>(type: "int", nullable: false),
                    UserId = table.Column<string>(type: "nvarchar(450)", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Dislikes", x => x.Id);
                    table.ForeignKey(
                        name: "FK_Dislikes_AspNetUsers_UserId",
                        column: x => x.UserId,
                        principalTable: "AspNetUsers",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_Dislikes_Comments_CommentId",
                        column: x => x.CommentId,
                        principalTable: "Comments",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "Likes",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    CommentId = table.Column<int>(type: "int", nullable: false),
                    CreatedBy = table.Column<int>(type: "int", nullable: false),
                    DateCreated = table.Column<DateTime>(type: "datetime2", nullable: false),
                    DateUpdated = table.Column<DateTime>(type: "datetime2", nullable: false),
                    DeletedAt = table.Column<DateTime>(type: "datetime2", nullable: true),
                    UpdatedBy = table.Column<int>(type: "int", nullable: false),
                    UserId = table.Column<string>(type: "nvarchar(450)", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Likes", x => x.Id);
                    table.ForeignKey(
                        name: "FK_Likes_AspNetUsers_UserId",
                        column: x => x.UserId,
                        principalTable: "AspNetUsers",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_Likes_Comments_CommentId",
                        column: x => x.CommentId,
                        principalTable: "Comments",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateIndex(
                name: "IX_Friendships_UserId1_UserId2",
                table: "Friendships",
                columns: new[] { "UserId1", "UserId2" },
                unique: true,
                filter: "[UserId1] IS NOT NULL AND [UserId2] IS NOT NULL");

            migrationBuilder.CreateIndex(
                name: "IX_Friendships_UserId2",
                table: "Friendships",
                column: "UserId2");

            migrationBuilder.AddCheckConstraint(
                name: "Friendship_UserId1_And_UserId2_CantBeEqual",
                table: "Friendships",
                sql: "UserId1 != UserId2");

            migrationBuilder.CreateIndex(
                name: "IX_Attachments_PostId",
                table: "Attachments",
                column: "PostId");

            migrationBuilder.CreateIndex(
                name: "IX_Dislikes_CommentId_UserId",
                table: "Dislikes",
                columns: new[] { "CommentId", "UserId" },
                unique: true,
                filter: "[UserId] IS NOT NULL");

            migrationBuilder.CreateIndex(
                name: "IX_Dislikes_UserId",
                table: "Dislikes",
                column: "UserId");

            migrationBuilder.CreateIndex(
                name: "IX_Likes_CommentId_UserId",
                table: "Likes",
                columns: new[] { "CommentId", "UserId" },
                unique: true,
                filter: "[UserId] IS NOT NULL");

            migrationBuilder.CreateIndex(
                name: "IX_Likes_UserId",
                table: "Likes",
                column: "UserId");

            migrationBuilder.AddForeignKey(
                name: "FK_Attachments_AspNetUsers_UserId",
                table: "Attachments",
                column: "UserId",
                principalTable: "AspNetUsers",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_Attachments_Posts_PostId",
                table: "Attachments",
                column: "PostId",
                principalTable: "Posts",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_Friendships_AspNetUsers_UserId1",
                table: "Friendships",
                column: "UserId1",
                principalTable: "AspNetUsers",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);

            migrationBuilder.AddForeignKey(
                name: "FK_Friendships_AspNetUsers_UserId2",
                table: "Friendships",
                column: "UserId2",
                principalTable: "AspNetUsers",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);

            migrationBuilder.AddForeignKey(
                name: "FK_Posts_Attachments_FileId",
                table: "Posts",
                column: "FileId",
                principalTable: "Attachments",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);
        }
    }
}
