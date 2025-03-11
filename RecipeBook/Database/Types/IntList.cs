using Microsoft.Data.SqlClient.Server;
using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace RecipeBook.Database.Types
{
    class IntListRow : SqlDataRecord
    {
        public int Id { get; set; }
    }
    class IntList : List<IntListRow>
    {
        public IntList(int[] ids) {
            foreach (int id in ids)
            {
                this.Add(new IntListRow { Id = id });
            }
        }
    }
}
