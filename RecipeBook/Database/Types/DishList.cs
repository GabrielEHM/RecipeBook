using Microsoft.Data.SqlClient.Server;
using RecipeBook.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace RecipeBook.Database.Types
{
    class DishListRow : SqlDataRecord
    {
        public int Id { get; set; }
        public int Servings { get; set; }
    }
    class DishList : List<DishListRow>
    {
        public DishList(List<Dish> dishes)
        {
            foreach (var dish in dishes)
            {
                var row = new DishListRow { Id = dish.Id, Servings = dish.Servings };
                this.Add(row);
            }
        }
    }
}
