-- Create the database if it does not exist
IF NOT EXISTS (SELECT * FROM sys.databases WHERE name = 'RecipeBook')
BEGIN
    CREATE DATABASE RecipeBook;
END
GO

USE RecipeBook;
GO

-- Create Dishes table
IF NOT EXISTS (SELECT * FROM sys.tables WHERE name = 'Dishes')
BEGIN
    CREATE TABLE Dishes (
        id INT PRIMARY KEY IDENTITY(1,1),
        name NVARCHAR(100) NOT NULL,
        description NVARCHAR(255),
        servings INT,
        prep_time INT,
        cook_time INT,
        recipe NVARCHAR(MAX)
    );
    CREATE INDEX IX_Dishes_name ON Dishes(name);
END
GO

-- Create Ingredients table
IF NOT EXISTS (SELECT * FROM sys.tables WHERE name = 'Ingredients')
BEGIN
    CREATE TABLE Ingredients (
        id INT PRIMARY KEY IDENTITY(1,1),
        name NVARCHAR(100) NOT NULL UNIQUE,
        description NVARCHAR(255)
    );
END
GO

-- Create DishesIngredients table
IF NOT EXISTS (SELECT * FROM sys.tables WHERE name = 'DishesIngredients')
BEGIN
    CREATE TABLE DishesIngredients (
        id INT PRIMARY KEY IDENTITY(1,1),
        dishId INT NOT NULL,
        ingredientId INT NOT NULL,
        quantity DECIMAL(10, 2),
        unit NVARCHAR(50),
        FOREIGN KEY (dishId) REFERENCES Dishes(id) ON DELETE CASCADE,
        FOREIGN KEY (ingredientId) REFERENCES Ingredients(id)
    );
    CREATE INDEX IX_DishesIngredients_dishId ON DishesIngredients(dishId);
    CREATE INDEX IX_DishesIngredients_ingredientId ON DishesIngredients(ingredientId);
END
GO

-- Create Menus table
IF NOT EXISTS (SELECT * FROM sys.tables WHERE name = 'Menus')
BEGIN
    CREATE TABLE Menus (
        id INT PRIMARY KEY IDENTITY(1,1),
        name NVARCHAR(100) NOT NULL UNIQUE,
        description NVARCHAR(255)
    );
END
GO

-- Create MenuDishes table
IF NOT EXISTS (SELECT * FROM sys.tables WHERE name = 'MenuDishes')
BEGIN
    CREATE TABLE MenuDishes (
        id INT PRIMARY KEY IDENTITY(1,1),
        menuId INT NOT NULL,
        dishId INT NOT NULL,
        servings INT,
        FOREIGN KEY (menuId) REFERENCES Menus(id) ON DELETE CASCADE,
        FOREIGN KEY (dishId) REFERENCES Dishes(id) ON DELETE CASCADE
    );
    CREATE INDEX IX_MenuDishes_menuId ON MenuDishes(menuId);
    CREATE INDEX IX_MenuDishes_dishId ON MenuDishes(dishId);
END
GO