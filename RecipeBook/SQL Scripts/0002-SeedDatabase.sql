-- Create the database if it does not exist
IF NOT EXISTS (SELECT * FROM sys.databases WHERE name = 'RecipeBook')
BEGIN
    CREATE DATABASE RecipeBook;
END
GO

USE RecipeBook;
GO

-- Create DISHES table
IF NOT EXISTS (SELECT * FROM sys.tables WHERE name = 'DISHES')
BEGIN
    CREATE TABLE DISHES (
        id INT PRIMARY KEY IDENTITY(1,1),
        name NVARCHAR(100) NOT NULL,
        description NVARCHAR(255),
        servings INT,
        prep_time INT,
        cook_time INT,
        recipe NVARCHAR(MAX)
    );
    CREATE INDEX IX_DISHES_name ON DISHES(name);
END
GO

-- Create INGREDIENTS table
IF NOT EXISTS (SELECT * FROM sys.tables WHERE name = 'INGREDIENTS')
BEGIN
    CREATE TABLE INGREDIENTS (
        id INT PRIMARY KEY IDENTITY(1,1),
        name NVARCHAR(100) NOT NULL,
        description NVARCHAR(255)
    );
    CREATE INDEX IX_INGREDIENTS_name ON INGREDIENTS(name);
END
GO

-- Create DISHES_INGREDIENTS table
IF NOT EXISTS (SELECT * FROM sys.tables WHERE name = 'DISHES_INGREDIENTS')
BEGIN
    CREATE TABLE DISHES_INGREDIENTS (
        id INT PRIMARY KEY IDENTITY(1,1),
        dishId INT NOT NULL,
        ingredientId INT NOT NULL,
        quantity DECIMAL(10, 2),
        unit NVARCHAR(50),
        FOREIGN KEY (dishId) REFERENCES DISHES(id),
        FOREIGN KEY (ingredientId) REFERENCES INGREDIENTS(id)
    );
    CREATE INDEX IX_DISHES_INGREDIENTS_dishId ON DISHES_INGREDIENTS(dishId);
    CREATE INDEX IX_DISHES_INGREDIENTS_ingredientId ON DISHES_INGREDIENTS(ingredientId);
END
GO

-- Create MENUS table
IF NOT EXISTS (SELECT * FROM sys.tables WHERE name = 'MENUS')
BEGIN
    CREATE TABLE MENUS (
        id INT PRIMARY KEY IDENTITY(1,1),
        name NVARCHAR(100) NOT NULL,
        description NVARCHAR(255)
    );
    CREATE INDEX IX_MENUS_name ON MENUS(name);
END
GO

-- Create MENU_DISHES table
IF NOT EXISTS (SELECT * FROM sys.tables WHERE name = 'MENU_DISHES')
BEGIN
    CREATE TABLE MENU_DISHES (
        id INT PRIMARY KEY IDENTITY(1,1),
        menuId INT NOT NULL,
        dishId INT NOT NULL,
        servings INT,
        FOREIGN KEY (menuId) REFERENCES MENUS(id),
        FOREIGN KEY (dishId) REFERENCES DISHES(id)
    );
    CREATE INDEX IX_MENU_DISHES_menuId ON MENU_DISHES(menuId);
    CREATE INDEX IX_MENU_DISHES_dishId ON MENU_DISHES(dishId);
END
GO