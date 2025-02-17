USE [RecipeBook]
GO

IF NOT EXISTS (SELECT * FROM sys.types WHERE name = 'IngredientList' AND schema_id = SCHEMA_ID('dbo'))
BEGIN
	CREATE TYPE [dbo].[IngredientList] AS TABLE(
		[name] [nvarchar](100) NULL,
		[description] [nvarchar](255) NULL,
		[quantity] [decimal](10, 2) NULL,
		[unit] [nvarchar](50) NULL
	)
END
GO

SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
-- =============================================
-- Author:		Gabriel
-- Create date: 17/02/2025
-- Description:	Adds a new dish recipe
-- =============================================
CREATE OR ALTER PROCEDURE [dbo].[AddNewDish]
	@name nvarchar(100), 
	@description nvarchar(255),
	@servings int,
	@prep_time int,
	@cook_time int,
	@recipe nvarchar(MAX),
	@IngredientsList dbo.IngredientList READONLY,
	@InsertedDish INT OUTPUT
AS
BEGIN
	-- SET NOCOUNT ON added to prevent extra result sets from
	-- interfering with SELECT statements.
	SET NOCOUNT ON;

    DECLARE @InsertedIngredients TABLE (id INT, name NVARCHAR(100));
	DECLARE @InsertedDishes TABLE (id INT, name NVARCHAR(100));
	
    BEGIN TRY
        BEGIN TRANSACTION;
 
		MERGE INTO [dbo].[Ingredients] AS target
		USING @IngredientsList AS source
		ON target.name = source.name
		WHEN MATCHED THEN
			UPDATE
			SET description = COALESCE(source.description, target.description)
		WHEN NOT MATCHED THEN
			INSERT ([name], [description])  -- Only insert name and description
			VALUES (source.name, source.description)
		OUTPUT 
			inserted.id, inserted.name INTO @InsertedIngredients (id, name);

		MERGE INTO [dbo].[Dishes] AS target
		USING (VALUES
			(@name, @description, @prep_time, @cook_time, @servings, @recipe)
		) AS source (name, description, prep_time, cook_time, servings, recipe)
		ON target.name = source.name
		WHEN MATCHED THEN
			UPDATE
			SET description = source.description,
				prep_time = source.prep_time, 
				cook_time = source.cook_time, 
				servings = source.servings, 
				recipe = source.recipe
		WHEN NOT MATCHED THEN
			INSERT ([name], [description], [prep_time], [cook_time], [servings], [recipe])
			VALUES (source.name, source.description, source.prep_time, source.cook_time, source.servings, source.recipe)
		OUTPUT inserted.id, inserted.name INTO @InsertedDishes;

		SELECT @InsertedDish = id
		FROM @InsertedDishes;

		MERGE INTO [dbo].[DishesIngredients] AS target
		USING (
			SELECT @InsertedDish, i.id AS ingredientId, v.quantity, v.unit
			FROM @IngredientsList AS v
			JOIN @InsertedIngredients i ON i.name = v.name
		) AS source (dishId, ingredientId, quantity, unit)
		ON target.dishId = source.dishId AND target.ingredientId = source.ingredientId
		WHEN MATCHED THEN
			UPDATE
			SET quantity = source.quantity,
				unit = source.unit
		WHEN NOT MATCHED THEN
			INSERT (dishId, ingredientId, quantity, unit)
			VALUES (source.dishId, source.ingredientId, source.quantity, source.unit);

		COMMIT TRANSACTION;

		RETURN;
	END TRY
    BEGIN CATCH
        ROLLBACK TRANSACTION;

        THROW;
    END CATCH;
END
GO

IF NOT EXISTS (SELECT * FROM sys.types WHERE name = 'DishList' AND schema_id = SCHEMA_ID('dbo'))
BEGIN
	CREATE TYPE [dbo].[DishList] AS TABLE(
		[id] INT NULL,
		[servings] INT NULL
	)
END
GO

SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
-- =============================================
-- Author:		Gabriel
-- Create date: 17/02/2025
-- Description:	Adds a new dish recipe
-- =============================================
CREATE OR ALTER PROCEDURE [dbo].[AddNewMenu]
	@name nvarchar(100), 
	@description nvarchar(255),
	@DishList dbo.DishList READONLY,
	@InsertedMenu INT OUTPUT
AS
BEGIN
	-- SET NOCOUNT ON added to prevent extra result sets from
	-- interfering with SELECT statements.
	SET NOCOUNT ON;

    DECLARE @InsertedMenus TABLE (id INT);
	
    BEGIN TRY
        BEGIN TRANSACTION;

		MERGE INTO [dbo].[Menus] AS target
		USING (VALUES
			(@name, @description)
		) AS source (name, description)
		ON target.name = source.name
		WHEN MATCHED THEN
			UPDATE
			SET description = source.description
		WHEN NOT MATCHED THEN
			INSERT ([name], [description])
			VALUES (source.name, source.description)
		OUTPUT inserted.id INTO @InsertedMenus;

		SELECT @InsertedMenu = id
		FROM @InsertedMenus;

		MERGE INTO [dbo].[MenuDishes] AS target
		USING (
			SELECT @InsertedMenu as menuId, d.id AS dishId, d.servings
			FROM @DishList AS d
		) AS source (menuId, dishId, servings)
		ON target.dishId = source.dishId AND target.menuId = source.menuId
		WHEN MATCHED THEN
			UPDATE
			SET servings = source.servings
		WHEN NOT MATCHED THEN
			INSERT (menuId, dishId, servings)
			VALUES (source.menuId, source.dishId, source.servings);

		COMMIT TRANSACTION;
	END TRY
    BEGIN CATCH
        ROLLBACK TRANSACTION;

        THROW;
    END CATCH;
END
GO