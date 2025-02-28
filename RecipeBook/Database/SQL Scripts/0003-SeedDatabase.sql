USE [RecipeBook]
GO

DECLARE @Ingredients dbo.IngredientList;
DECLARE @InsertedDish INT;
DECLARE @InsertedMenu INT;
DECLARE @InsertedDishes dbo.DishList;

-- Insert the ingredient for first dish
INSERT INTO @Ingredients (name, quantity, unit)
VALUES 
    ('Harina de trigo', 200, 'gr'),
    ('Mantequilla', 100, 'gr'),
    ('Huevos', 4, 'ud'),
    ('Sal', 5, 'gr'),
    ('Nata Liquida', 200, 'ml'),
    ('Queso Rallado', 150, 'gr'),
    ('Champiñones', 100, 'gr'),
    ('Baicon', 120, 'gr'),
    ('Pimienta Negra Molida', 2, 'gr'),
    ('Nuez Moscada Molida', 1, 'gr');


-- create first dish
EXEC [dbo].[Dishes_Add] 
    @IngredientsList = @Ingredients,
    @name = 'Quiche lorraine',
    @description = 'Una tarta salada en base a huevo y crema.',
    @servings = 6,
    @prep_time = 30,
    @cook_time = 40,
    @recipe = 'Empezamos preparando la masa quebrada. Hacemos un volcán con la harina, añadimos la mantequilla cortada en dados y vamos uniendo todo hasta conseguir una masa homogénea. No conviene que amasemos demasiado para que no nos quede una masa demasiado dura. Hacemos una bola, la envolvemos con papel film y la guardamos media hora en el frigorífico. Pasado este tiempo, extendemos la masa con el rodillo y cubrimos con ella un molde acanalado de base desmoldable. Pasamos el rodillo por el borde y retiramos el sobrante de masa. Pinchamos varias veces con un tenedor y horneamos la base durante 15 minutos en el horno precalentado a 180ºC. Mientras, rallamos el queso, cortamos el jamón en daditos, fileteamos los champiñones y los doramos ligeramente en una sartén para que pierdan el agua. En un bol grande batimos los huevos y los mezclamos con la nata y la leche, la sal, la pimienta y una pizca de nuez moscada. Sobre la base de la tarta ponemos el jamón en crudo o el beicon o la panceta pasados por la sartén, el queso y los champiñones. Sobre ellos echamos la mezcla de nata y huevo, dejando que se empape bien todo. Por último, horneamos la quiche a 180º durante 30 o 40 minutos o hasta que esté el relleno cuajado y la masa dorada. Servir caliente.',
	@InsertedDish = @InsertedDish OUTPUT;

INSERT INTO @InsertedDishes(id, servings) VALUES (@InsertedDish, 6);

DELETE FROM @Ingredients;

-- Insert the ingredient for the second dish
INSERT INTO @Ingredients (name, quantity, unit)
VALUES 
    ('Pollo entero', 1200, 'gr'),
    ('Limon', 1, 'ud'),
    ('Cebolla', 1, 'ud'),
    ('Sal', 15, 'gr'),
    ('Albahaca', 5, 'gr'),
    ('Tomillo', 2, 'gr'),
    ('Romero', 3, 'gr'),
    ('Oregano', 5, 'gr'),
    ('Pimienta Negra Molida', 4, 'gr'),
    ('Manteca de cerdo', 10, 'gr'),
    ('Patata de guarnicion', 3, 'ud');

-- Create second dish
EXEC [dbo].[Dishes_Add] 
    @IngredientsList = @Ingredients,
    @name = 'Pollo al horno con finas hierbas',
    @description = 'Delicioso pollo al horno con finas hierbas.',
    @servings = 4,
    @prep_time = 20,
    @cook_time = 90,
    @recipe = 'Una vez bien limpio el pollo y retiradas las plumillas que pueda tener, salpimentamos por dentro y metemos en el interior media cebolla y medio limón, y una buena rama de cada una de las hierbas frescas, para que perfume el pollo mientras se va asando.\nPor otra parte, preparamos una "cama" con patatas cortadas en rodajas gruesas y cebolla cortada en juliana, dispuestas en una bandeja de horno. Sobre las patatas repartimos también algunas hierbas aromáticas para aumentar su presencia.\nSobre esa cama ponemos el pollo, untando su exterior con manteca de cerdo para conseguir un sabor y un color espectacular. Encima ponemos algunas hojitas de tomillo y de romero y lo metemos en el horno previamente precalentado.\nHorneamos durante 80 minutos, a una temperatura de 190º, dando la vuelta al pollo cada 20 minutos para que se dore por todas partes. Al final, gratinamos durante 5 minutos para dar ese bonito color final al plato. Al girar el pollo, removemos las patatas para que también se hagan por todas partes',
	@InsertedDish = @InsertedDish OUTPUT;

INSERT INTO @InsertedDishes(id, servings) VALUES (@InsertedDish, 4);

DELETE FROM @Ingredients

-- Insert the ingredient for the second dish
INSERT INTO @Ingredients (name, quantity, unit)
VALUES 
    ('Mantequilla sin sal', 125, 'gr'),
    ('Azúcar moreno', 150, 'gr'),
    ('Huevos', 1, 'ud'),
    ('Sal', 2, 'gr'),
    ('Esencia de vainilla', 5, 'ml'),
    ('Bicarbonato sódico', 5, 'gr'),
    ('Levadura química', 2, 'gr'),
    ('Harina de repostería', 210, 'gr'),
    ('Chips de chocolate', 60, 'gr'),
    ('Flor de sal', 3, 'gr');

-- Create second dish
EXEC [dbo].[Dishes_Add] 
    @IngredientsList = @Ingredients,
    @name = 'Galletas con chips de chocolate y sal',
    @description = 'Estas galletas de mantequilla con chips de chocolate y flor de sal harán las delicias de cualquiera.',
    @servings = 20,
    @prep_time = 20,
    @cook_time = 10,
    @recipe = 'Colocar la mantequilla en una sartén y llevar al fuego. Calentar a temperatura media hasta que se derrita, bajar la potencia y dejar que llegue a ebullición. Continuar calentando durante unos 5-8 minutos, remover con suavidad y mantener la cocción hasta que adquiera un tono tostado y huela a nueces y caramelo. Retirar y llevar a un recipiente resistente al calor.\nDisponer en un cuenco mediano la harina con la sal el bicarbonato y la levadura. Mezclar con unas varillas y formar un hueco en el centro. Cuando se haya enfriado un poco la mantequilla, agregar el azúcar y batir con las varillas a mano. Agregar el huevo y la vainilla, y batir. Combinar las dos masas, mezclar y echar los chips de chocolate, incorporándolos bien. Tapar con plástico film y llevar a la nevera 30 minutos.\nPrecalentar el horno a 175ºC y preparar un par de bandejas. Tomar porciones pequeñas de masa, formar bolitas con las manos y colocarlas separadas entre sí. Aplastar ligeramente y decorar con algunos chips extra. Añadir una pizca de flor de sal por encima y hornear una bandeja cada vez durante unos 10 minutos, hasta que se empiecen a dorar. Dejar enfriar sobre una rejilla.',
	@InsertedDish = @InsertedDish OUTPUT;

INSERT INTO @InsertedDishes(id, servings) VALUES (@InsertedDish, 40);

EXEC [dbo].[Menus_Add]
	@name = 'Domingo', 
	@description = 'Plan de comida para los Domingos con tiempo libre.',
	@DishList = @InsertedDishes,
	@InsertedMenu = @InsertedMenu OUTPUT 