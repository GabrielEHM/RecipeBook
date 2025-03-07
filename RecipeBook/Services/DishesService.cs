﻿using RecipeBook.Database.Repositories;
using RecipeBook.Models;

namespace RecipeBook.Services
{
    class DishesService : Service<Dish>
    {
        private readonly DishesRepository _repository;

        public DishesService(DishesRepository dishesRepository)
            : base()
        {
            _repository = dishesRepository;
        }

        public override bool ListAll(int page = 1, int pageSize = 10)
        {
            bool repeat = true;
            while (repeat)
            {
                repeat = ConsoleMenuService.ListEntities(_repository.GetPage(page, pageSize), this);
            }
            return repeat;
        }

        public override bool GetById(string id)
        {
            throw new NotImplementedException();
        }

        public override bool Add(string? id = null)
        {
            throw new NotImplementedException();
        }

        public override bool Delete(string[] ids)
        {
            throw new NotImplementedException();
        }
    }
}
