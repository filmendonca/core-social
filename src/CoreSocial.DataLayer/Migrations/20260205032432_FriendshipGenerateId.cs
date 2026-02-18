using Microsoft.EntityFrameworkCore.Migrations;

namespace DataLayer.Migrations
{
    public partial class FriendshipGenerateId : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            //Drop PK constraint
            migrationBuilder.Sql(@"
                ALTER TABLE [Friendships] DROP CONSTRAINT [PK_Friendships];
            ");

            //Drop old Id col
            migrationBuilder.Sql(@"
                ALTER TABLE [Friendships] DROP COLUMN [Id];
            ");

            //Redo Id as identity
            migrationBuilder.Sql(@"
                ALTER TABLE [Friendships] 
                ADD [Id] INT IDENTITY(1,1) NOT NULL PRIMARY KEY;
            ");
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            //Drop identity col
            migrationBuilder.Sql(@"
                ALTER TABLE [Friendships] DROP CONSTRAINT [PK_Friendships];
                ALTER TABLE [Friendships] DROP COLUMN [Id];
            ");

            //Redo Id
            migrationBuilder.Sql(@"
                ALTER TABLE [Friendships] 
                ADD [Id] INT NOT NULL PRIMARY KEY;
            ");
        }
    }
}
