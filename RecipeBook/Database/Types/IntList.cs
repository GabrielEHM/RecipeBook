using Microsoft.Data.SqlClient.Server;

namespace RecipeBook.Database.Types
{
    class IntListRow : SqlDataRecord
    {
        public int Id { get; set; }
    }
    class IntList : List<IntListRow>
    {
        public IntList(int[] ids)
        {
            foreach (int id in ids)
            {
                this.Add(new IntListRow { Id = id });
            }
        }
    }
}
