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
CREATE OR ALTER PROCEDURE [dbo].[Dishes_Add]
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

-- =============================================
-- Author:		Gabriel
-- Create date: 17/02/2025
-- Description:	Adds a new dish recipe
-- =============================================
CREATE OR ALTER PROCEDURE [dbo].[Menus_Add]
	@name nvarchar(100), 
	@description nvarchar(255),
	@DishList dbo.DishList READONLY,
	@InsertedMenu INT OUTPUT
AS
BEGIN
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

-- =============================================
-- Author:		Gabriel Hernández
-- Create date: 24/02/2025
-- Description:	Adds an Ingredient or updates its description if already exists
-- =============================================
CREATE OR ALTER PROCEDURE [dbo].[Ingredients_Add] 
	@name NVARCHAR(100), 
	@description NVARCHAR(255)
AS
BEGIN
	SET NOCOUNT ON;

	MERGE INTO [dbo].[Ingredients] AS target
		USING (VALUES (@name, @description)) AS source (name, description)
		ON target.name = source.name
		WHEN MATCHED THEN
			UPDATE
			SET description = COALESCE(source.description, target.description)
		WHEN NOT MATCHED THEN
			INSERT ([name], [description])  -- Only insert name and description
			VALUES (source.name, source.description)
		OUTPUT 
			inserted.id, inserted.name;
END
GO

-- =============================================
-- Author:		Gabriel Hernández
-- Create date: 24/02/2025
-- Description:	Removes an ingredient if its not used on any dishes
-- =============================================
CREATE OR ALTER PROCEDURE [dbo].[Ingredients_Remove]
	@id int
AS
BEGIN
	SET NOCOUNT ON;

    DECLARE @ErrorMessage NVARCHAR(4000);
    DECLARE @ErrorSeverity INT;
    DECLARE @ErrorState INT;

    BEGIN TRY
        DELETE FROM Ingredients WHERE Id = @id;
        RETURN 1;
    END TRY
    BEGIN CATCH
        -- Check if the error is due to foreign key constraint
        IF ERROR_NUMBER() = 547 -- FK violation error number
        BEGIN
            RETURN 0; -- Indicates FK constraint violation
        END
        ELSE
        BEGIN
            -- Re-throw other errors
            SELECT @ErrorMessage = ERROR_MESSAGE(),
                   @ErrorSeverity = ERROR_SEVERITY(),
                   @ErrorState = ERROR_STATE();
            RAISERROR(@ErrorMessage, @ErrorSeverity, @ErrorState);
        END
    END CATCH;
END
GO

-- =============================================
-- Author:		Gabriel Hernández
-- Create date: 24/02/2025
-- Description:	Gets an ingredients details and list of dishes it is being used in
-- =============================================
CREATE OR ALTER PROCEDURE [dbo].[Ingredients_GetById]
	@id INT
AS
BEGIN
	SET NOCOUNT ON;

	SELECT * 
	FROM [dbo].[Ingredients]
	WHERE id = @id

	SELECT d.*
	FROM [dbo].[DishesIngredients] di
	JOIN [dbo].[Dishes] d ON di.dishId = d.id
	WHERE di.ingredientId = @id
END
GO

-- =============================================
-- Author:		Gabriel Hernández
-- Create date: 24/02/2025
-- Description:	Returns a paginated list of ingredients
-- =============================================
CREATE OR ALTER PROCEDURE [dbo].[Ingredients_GetAll]
	@PageNumber INT = 1, 
	@PageSize INT = 10,
    @TotalCount INT OUTPUT
AS
BEGIN
	SET NOCOUNT ON;

    IF @PageNumber < 1 SET @PageNumber = 1;
    IF @PageSize < 1 SET @PageSize = 10;

    SELECT i.id, name, description, COUNT(DISTINCT di.dishId) AS 'Used in (Dishes)'
    FROM [dbo].[Ingredients] i
	LEFT JOIN [dbo].[DishesIngredients] di
		ON i.id = di.ingredientId
	GROUP BY i.id, i.name, i.description
    ORDER BY i.id
    OFFSET (@PageNumber - 1) * @PageSize ROWS
    FETCH NEXT @PageSize ROWS ONLY;

	SELECT @TotalCount = COUNT(*) FROM [dbo].[Ingredients];
END
GO

-- =============================================
-- Author:		Gabriel Hernández
-- Create date: 24/02/2025
-- Description:	Removes a dish
-- =============================================
CREATE OR ALTER PROCEDURE [dbo].[Dishes_Remove]
	@id INT
AS
BEGIN
	SET NOCOUNT ON;

    BEGIN TRY
        DELETE FROM [dbo].[Dishes] WHERE id = @id;
    END TRY
    BEGIN CATCH
        THROW;
    END CATCH;
END
GO

IF NOT EXISTS (SELECT * FROM sys.types WHERE name = 'IntList' AND schema_id = SCHEMA_ID('dbo'))
BEGIN
	CREATE TYPE [dbo].[IntList] AS TABLE(
		[id] INT PRIMARY KEY
	)
END
GO

-- =============================================
-- Author:		Gabriel Hernández
-- Create date: 24/02/2025
-- Description:	Removes dishes in bulk
-- =============================================
CREATE OR ALTER PROCEDURE [dbo].[Dishes_BulkRemove]
	@ids [dbo].[IntList] READONLY
AS
BEGIN
	SET NOCOUNT ON;

    BEGIN TRY
        DELETE FROM [dbo].[Dishes]
        WHERE id IN (SELECT id FROM @ids);
    END TRY
    BEGIN CATCH
        THROW;
    END CATCH;
END
GO

-- =============================================
-- Author:		Gabriel Hernández
-- Create date: 24/02/2025
-- Description:	Returns the dish's ingredient list adjusted by the requested serving amount
-- =============================================
CREATE OR ALTER FUNCTION [dbo].[GetDishAdjustedIngredients]
(
	@dishId INT, 
	@servings INT = NULL
)
RETURNS 
@ingredients TABLE 
(
	id INT, 
	name NVARCHAR(100) NOT NULL,
	description NVARCHAR(255) NULL,
	quantity DECIMAL(10, 2),
	unit NVARCHAR(50)
)
AS
BEGIN
	DECLARE @servingsRatio DECIMAL(10, 2) = 1;

	IF (@servings IS NOT NULL)
	BEGIN
		SELECT @servingsRatio = @servings/servings
		FROM [dbo].[Dishes]
		WHERE id = @dishId
	END

	INSERT INTO @ingredients (id, name, description, quantity, unit)
	SELECT 
		i.id, 
		i.name, 
		i.description, 
		di.quantity * @servingsRatio AS quantity, 
		di.unit
	FROM [dbo].[DishesIngredients] di
	JOIN [dbo].[Ingredients] i ON di.ingredientId = i.id
	WHERE di.dishId = @dishId
	
	RETURN;
END
GO

-- =============================================
-- Author:		Gabriel Hernández
-- Create date: 24/02/2025
-- Description:	Gets an dish's details, its ingredients and list of menus it is being used in. Ingredient amounts can be adjusted by giving the desired servings
-- =============================================
ALTER   PROCEDURE [dbo].[Dishes_GetById]
	@id INT,
	@servings INT = NULL
AS
BEGIN
	SET NOCOUNT ON;

	SELECT *
	FROM [dbo].[Dishes]
	WHERE id = @id

	SELECT * FROM dbo.GetDishAdjustedIngredients(@id, @servings)

	SELECT DISTINCT m.*
	FROM [dbo].[MenuDishes] md
	JOIN [dbo].[Menus] m ON md.menuId = m.id
	WHERE md.dishId = @id
END
GO

-- =============================================
-- Author:		Gabriel Hernández
-- Create date: 24/02/2025
-- Description:	Returns a paginated list of dishes
-- =============================================
CREATE OR ALTER PROCEDURE [dbo].[Dishes_GetAll]
	@PageNumber INT = 1, 
	@PageSize INT = 10,
    @TotalCount INT OUTPUT
AS
BEGIN
	SET NOCOUNT ON;

    IF @PageNumber < 1 SET @PageNumber = 1;
    IF @PageSize < 1 SET @PageSize = 10;

    SELECT d.id
		  ,d.name
		  ,d.description
		  ,d.servings
		  ,d.prep_time
		  ,d.cook_time
		  ,COUNT(DISTINCT mi.menuId) AS 'Used in (Menus)'
    FROM [dbo].[Dishes] d
	LEFT JOIN [dbo].[MenuDishes] mi
		ON d.id = mi.dishId
	GROUP BY d.id, d.name, d.description, d.servings, d.prep_time, d.cook_time
    ORDER BY d.id
    OFFSET (@PageNumber - 1) * @PageSize ROWS
    FETCH NEXT @PageSize ROWS ONLY;

	SELECT @TotalCount = COUNT(*) FROM [dbo].[Dishes];
END
GO

-- =============================================
-- Author:		Gabriel Hernández
-- Create date: 24/02/2025
-- Description:	Gets an menu's details and a list of the dishes in contains
-- =============================================
CREATE OR ALTER PROCEDURE [dbo].[Menus_GetById]
	@id INT
AS
BEGIN
	SET NOCOUNT ON;

	SELECT * 
	FROM [dbo].[Menus]
	WHERE id = @id

	SELECT d.*, md.servings
	FROM [dbo].[MenuDishes] md
	JOIN [dbo].[Dishes] d ON md.dishId = d.id
	WHERE md.menuId = @id
END
GO

-- =============================================
-- Author:		Gabriel Hernández
-- Create date: 24/02/2025
-- Description:	Returns a paginated list of menus
-- =============================================
CREATE OR ALTER PROCEDURE [dbo].[Menus_GetAll]
	@PageNumber INT = 1, 
	@PageSize INT = 10,
    @TotalCount INT OUTPUT
AS
BEGIN
	SET NOCOUNT ON;

    IF @PageNumber < 1 SET @PageNumber = 1;
    IF @PageSize < 1 SET @PageSize = 10;

    SELECT m.id
		  ,m.name
		  ,m.description
		  ,COUNT(mi.dishId) AS 'Dishes in menu'
    FROM [dbo].[Menus] m
	LEFT JOIN [dbo].[MenuDishes] mi
		ON m.id = mi.menuId
	GROUP BY m.id, m.name, m.description
    ORDER BY m.id
    OFFSET (@PageNumber - 1) * @PageSize ROWS
    FETCH NEXT @PageSize ROWS ONLY;

	SELECT @TotalCount = COUNT(*) FROM [dbo].[Dishes];
END
GO

-- =============================================
-- Author:		Gabriel Hernández
-- Create date: 24/02/2025
-- Description:	Creates a shopping list of ingredients based on a menu
-- =============================================
CREATE OR ALTER PROCEDURE Menus_GetShoppingList 
	@menuIds [dbo].[IntList] READONLY
AS
BEGIN
	SET NOCOUNT ON;

	DECLARE @dishes TABLE (id INT, servings INT);

	INSERT INTO @dishes (id, servings)
	SELECT dishId, servings
	FROM [dbo].[MenuDishes]
	WHERE menuId IN (SELECT id FROM @menuIds)

	SELECT i.id, i.name, SUM(i.quantity) as quantity, i.unit
	FROM @dishes d
	CROSS APPLY [dbo].[GetDishAdjustedIngredients](d.id, d.servings) i
	GROUP BY i.id, i.name, i.unit
END
GO