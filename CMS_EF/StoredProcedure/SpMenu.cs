// using System.Data.Entity.Migrations;

using Microsoft.EntityFrameworkCore.Migrations;
using System.Diagnostics.CodeAnalysis;

namespace CMS_EF.StoredProcedure
{
    public partial class SpMenu 
    {

        public static void CreateSp(MigrationBuilder migrationBuilder)
        {
            Sp_Menu_RepairNested(migrationBuilder);
            Sp_BE_MenuManager_MoveUp(migrationBuilder);
            Sp_BE_MenuManager_MoveDown(migrationBuilder);
        }
        private static void Sp_Menu_RepairNested(MigrationBuilder migrationBuilder)
        {
			migrationBuilder.Sql(@"CREATE OR ALTER PROCEDURE Menu_RepairNested
                                    AS
                                    BEGIN
                                        DECLARE @Id int
										DECLARE @PId int
										DECLARE @RowCount int
										DECLARE @Lft int
										DECLARE @Lvl int
										SELECT @RowCount = COUNT(*) FROM [Menu] WHERE [Id] > -1
										UPDATE [Menu] SET [Lvl] = 0, [Lft] = 0, [Rgt] = 1 WHERE [PId] = 0
										UPDATE [Menu] SET [Lvl] = NULL, [Lft] = NULL, [Rgt] = NULL WHERE [PId] != 0
										WHILE EXISTS (SELECT * FROM [Menu] WHERE [Lft] IS NULL) AND @RowCount > 0
										BEGIN
											SELECT @Id = MAX([Nc].[Id]) FROM [Menu] [Nc]
												INNER JOIN [Menu] [Nc2] ON [Nc2].[Id] = [Nc].[PId]
												WHERE [Nc].[Lft] IS NULL AND [Nc2].[Lft] IS NOT NULL
											SELECT @PId = [PId] FROM [Menu] WHERE [Id] = @Id
											SELECT @Lft = [Lft], @Lvl = [Lvl] FROM [Menu] WHERE [Id] = @PId
											UPDATE [Menu] SET [Rgt] = [Rgt] + 2 WHERE [Rgt] > @Lft;
											UPDATE [Menu] SET [Lft] = [Lft] + 2 WHERE [Lft] > @Lft;
											UPDATE [Menu] SET [Lft] = @Lft + 1, [Rgt] = @Lft + 2, [Lvl] = @Lvl + 1 WHERE [Id] = @Id
											SET @RowCount = @RowCount - 1
										END
                                    END");
		}

		private static void Sp_BE_MenuManager_MoveDown(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.Sql(@"CREATE OR ALTER PROCEDURE BE_MenuManager_MoveDown(
                                        @Id int, 
                                        @Last_Modified_At datetime, 
                                        @Last_Modified_By int )
                                    AS
                                    BEGIN
										SET NOCOUNT ON;
										DECLARE @PId int
										DECLARE @Lft int
										DECLARE @Rgt int
										DECLARE @Lvl int
										DECLARE @BroId int

										SELECT @PId = [PId], @Lvl = [Lvl], @Lft = [Lft], @Rgt = [Rgt] FROM [Menu] WHERE [Id] = @Id
										SELECT TOP(1) @BroId = [Id] FROM [Menu] WHERE [Lft] > @Lft AND [PId] = @PId ORDER BY [Lft]
										IF (@BroId IS NOT NULL)
											BEGIN
												DECLARE @WidthNode int
												DECLARE @BroRgt int
												DECLARE @PLvl int
												DECLARE @NewLvl int
												DECLARE @NewLft int
												DECLARE @NewRgt int
		
												SET XACT_ABORT ON
												BEGIN TRAN
												BEGIN TRY			
													SET @WidthNode = @Rgt - @Lft + 1
			
													UPDATE [Menu] SET [Rgt] = [Rgt] - @Rgt, [Lft] = [Lft] - @Lft WHERE [Lft] BETWEEN @Lft AND @Rgt
													UPDATE [Menu] SET [Rgt] = [Rgt] - @WidthNode WHERE [Rgt] > @Rgt
													UPDATE [Menu] SET [Lft] = [Lft] - @WidthNode WHERE [Lft] > @Rgt
													SELECT @BroRgt = [Rgt] FROM [Menu] WHERE [Id] = @BroId
													UPDATE [Menu] SET [Lft] = [Lft] + @WidthNode WHERE [Lft] > @BroRgt AND [Rgt] > 0
													UPDATE [Menu] SET [Rgt] = [Rgt] + @WidthNode WHERE [Rgt] > @BroRgt
													SELECT @PLvl = [Lvl] FROM [Menu] WHERE [Id] = @PId
													SET @NewLvl = @PLvl + 1
													UPDATE [Menu] SET [Lvl] = [Lvl] - @Lvl + @NewLvl WHERE [Rgt] <= 0
													SET @NewLft = @BroRgt + 1
													SET @NewRgt = @BroRgt + @WidthNode;
													UPDATE [Menu] SET [PId] = @PId, [Lft] = @NewLft, [Rgt] = @NewRgt WHERE [Id] = @Id
													UPDATE [Menu] SET [Rgt] = [Rgt] + @NewRgt, [Lft] = [Lft] + @NewLft WHERE [Rgt] < 0
													UPDATE [Menu] SET [LastModifiedAt] = @Last_Modified_At, [LastModifiedBy] = @Last_Modified_By WHERE [Id] = @Id
													COMMIT
												END TRY
												BEGIN CATCH
													ROLLBACK
													DECLARE @ErrorMessage varchar(MAX)
													SELECT @ErrorMessage = ERROR_MESSAGE()
													RAISERROR(@ErrorMessage, 16, 1)
												END CATCH
											END
										SET NOCOUNT OFF;
                                    END");
        }

		private static void Sp_BE_MenuManager_MoveUp(MigrationBuilder migrationBuilder)
		{
			migrationBuilder.Sql(@"CREATE OR ALTER PROCEDURE BE_MenuManager_MoveUp(
                                        @Id int, 
                                        @Last_Modified_At datetime, 
                                        @Last_Modified_By int )
                                    AS
                                    BEGIN
										SET NOCOUNT ON;
										DECLARE @PId int
										DECLARE @Lft int
										DECLARE @Rgt int
										DECLARE @Lvl int
										DECLARE @BroId int

										SELECT @PId = [PId], @Lvl = [Lvl], @Lft = [Lft], @Rgt = [Rgt] FROM [Menu] WHERE [Id] = @Id
										SELECT TOP(1) @BroId = [Id] FROM [Menu] WHERE [Lft] < @Lft AND [PId] = @PId ORDER BY [Lft] DESC
										IF (@BroId IS NOT NULL)
											BEGIN
												DECLARE @WidthNode int
												DECLARE @BroLft int
												DECLARE @PLvl int
												DECLARE @NewLvl int
												DECLARE @NewRgt int
		
												SET XACT_ABORT ON
												BEGIN TRAN
												BEGIN TRY			
													SET @WidthNode = @Rgt - @Lft + 1
													UPDATE [Menu] SET [Rgt] = [Rgt] -  @Rgt, [Lft] = [Lft] - @Lft WHERE [Lft] BETWEEN @Lft AND @Rgt
													UPDATE [Menu] SET [Rgt] = [Rgt] - @WidthNode WHERE [Rgt] > @Rgt
													UPDATE [Menu] SET [Lft] = [Lft] - @WidthNode WHERE [Lft] > @Rgt
													SELECT @BroLft = [Lft] FROM [Menu] WHERE [Id] = @BroId
													UPDATE [Menu] SET [Lft] = [Lft] + @WidthNode WHERE [Lft] >= @BroLft AND [Rgt] > 0
													UPDATE [Menu] SET [Rgt] = [Rgt] + @WidthNode WHERE [Rgt] >= @BroLft
													SELECT @PLvl = [Lvl] FROM [Menu] WHERE [Id] = @PId
													SET @NewLvl = @PLvl + 1
													UPDATE [Menu] SET [Lvl] = [Lvl] - @Lvl + @NewLvl WHERE [Rgt] <= 0
													SET @NewRgt = @BroLft + @WidthNode - 1;
													UPDATE [Menu] SET [PId] = @PId, [Lft] = @BroLft, [Rgt] = @NewRgt WHERE [Id] = @Id
													UPDATE [Menu] SET [Rgt] = [Rgt] + @NewRgt, [Lft] = [Lft] + @BroLft WHERE [Rgt] < 0
													UPDATE [Menu] SET [LastModifiedAt] = @Last_Modified_At, [LastModifiedBy] = @Last_Modified_By WHERE [Id] = @Id
													
													COMMIT
												END TRY
												BEGIN CATCH
													ROLLBACK
													DECLARE @ErrorMessage varchar(MAX)
													SELECT @ErrorMessage = ERROR_MESSAGE()
													RAISERROR(@ErrorMessage, 16, 1)
												END CATCH
											END
										SET NOCOUNT OFF;
                                    END");
		}

        public static void Drop_Sp(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.Sql(@"DROP PROCEDURE Menu_RepairNested");
            migrationBuilder.Sql(@"DROP PROCEDURE BE_MenuManager_MoveDown");
            migrationBuilder.Sql(@"DROP PROCEDURE BE_MenuManager_MoveUp");
		}
    }
}
