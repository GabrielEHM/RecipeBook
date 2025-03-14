using Dapper;
using Microsoft.Data.SqlClient;
using RecipeBook.Database.Types;
using RecipeBook.Models;

namespace RecipeBook.Database
{
    class DbService(DatabaseConnection databaseConnection)
    {
        private readonly DatabaseConnection _databaseConnection = databaseConnection;

        public async IAsyncEnumerable<Paged<T>> GetAll<T>(string procedureName, int pageSize = 10, int startPage = 1) where T : IPageable
        {
            int page = startPage;
            while (true)
            {
                Paged<T> pageEntities;
                try
                {
                    using (var connection = _databaseConnection.GetConnection())
                    {
                        DynamicParameters parameters = new DynamicParameters();
                        parameters.Add("PageNumber", page);
                        parameters.Add("PageSize", pageSize);
                        parameters.Add("TotalCount", dbType: System.Data.DbType.Int32, direction: System.Data.ParameterDirection.Output);

                        var entities = (await connection.QueryAsync<T>(procedureName, parameters, commandType: System.Data.CommandType.StoredProcedure)).ToList();
                        var totalItems = parameters.Get<int>("TotalCount");
                        pageEntities = new Paged<T>(entities, page, pageSize, totalItems);
                    }
                }
                catch (SqlException sqlEx)
                {
                    Console.WriteLine($"SQL error occurred: {sqlEx.Message}");
                    throw;
                }
                catch (Exception ex)
                {
                    Console.WriteLine($"An error occurred: {ex.Message}");
                    throw;
                }
                if (pageEntities != null)
                {
                    yield return pageEntities;

                    if (pageEntities.Entities.Count < pageSize)
                    {
                        yield break;
                    }

                    page++;
                }
            }
        }

        public Paged<T> GetPage<T>(string procedureName, int page, int pageSize = 10) where T : IPageable
        {
            try
            {
                using (var connection = _databaseConnection.GetConnection())
                {
                    DynamicParameters parameters = new DynamicParameters();
                    parameters.Add("PageNumber", page);
                    parameters.Add("PageSize", pageSize);
                    parameters.Add("TotalCount", dbType: System.Data.DbType.Int32, direction: System.Data.ParameterDirection.Output);

                    var entities = connection.Query<T>(procedureName, parameters, commandType: System.Data.CommandType.StoredProcedure).ToList();
                    var totalItems = parameters.Get<int>("TotalCount");
                    var pageEntities = new Paged<T>(entities, page, pageSize, totalItems);
                    return pageEntities;
                }
            }
            catch (SqlException sqlEx)
            {
                Console.WriteLine($"SQL error occurred: {sqlEx.Message}");
                throw;
            }
            catch (Exception ex)
            {
                Console.WriteLine($"An error occurred: {ex.Message}");
                throw;
            }
        }

        public T? GetById<T>(string procedureName, int id) where T : Storable
        {
            try
            {
                using (var connection = _databaseConnection.GetConnection())
                {
                    var parameters = new DynamicParameters();
                    parameters.Add("id", id);
                    using (var results = connection.QueryMultiple(procedureName, parameters, commandType: System.Data.CommandType.StoredProcedure))
                    {
                        var entity = Storable.FromReader<T>(results);
                        return entity;
                    }
                }
            }
            catch (SqlException sqlEx)
            {
                Console.WriteLine($"SQL error occurred: {sqlEx.Message}");
                throw;
            }
            catch (Exception ex)
            {
                Console.WriteLine($"An error occurred: {ex.Message}");
                throw;
            }
        }

        public Dish? GetById(string procedureName, int id, int? servings = null)
        {
            try
            {
                using (var connection = _databaseConnection.GetConnection())
                {
                    var parameters = new DynamicParameters();
                    parameters.Add("id", id);
                    parameters.Add("servings", servings);
                    using (var results = connection.QueryMultiple(procedureName, parameters, commandType: System.Data.CommandType.StoredProcedure))
                    {
                        var entity = Storable.FromReader<Dish>(results);
                        return entity;
                    }
                }
            }
            catch (SqlException sqlEx)
            {
                Console.WriteLine($"SQL error occurred: {sqlEx.Message}");
                throw;
            }
            catch (Exception ex)
            {
                Console.WriteLine($"An error occurred: {ex.Message}");
                throw;
            }
        }

        /// <returns>
        /// The id of the created or updated entity.
        /// </returns>
        public int CreateOrUpdate<T>(string procedureName, T entity) where T : Storable
        {
            try
            {
                using (var connection = _databaseConnection.GetConnection())
                {
                    var parameters = entity.ToDynamicParameters();
                    parameters.Add("insertedId", dbType: System.Data.DbType.Int32, direction: System.Data.ParameterDirection.Output);

                    connection.Execute(procedureName, parameters, commandType: System.Data.CommandType.StoredProcedure);
                    var insertedEntityId = parameters.Get<int>("TotalCount");
                    return insertedEntityId;
                }
            }
            catch (SqlException sqlEx)
            {
                Console.WriteLine($"SQL error occurred: {sqlEx.Message}");
                throw;
            }
            catch (Exception ex)
            {
                Console.WriteLine($"An error occurred: {ex.Message}");
                throw;
            }
        }

        public bool ConstrainedRemove(string procedureName, int id)
        {
            try
            {
                using (var connection = _databaseConnection.GetConnection())
                {

                    return connection.QuerySingle<bool>(procedureName, new { id }, commandType: System.Data.CommandType.StoredProcedure);
                }
            }
            catch (SqlException sqlEx)
            {
                Console.WriteLine($"SQL error occurred: {sqlEx.Message}");
                throw;
            }
            catch (Exception ex)
            {
                Console.WriteLine($"An error occurred: {ex.Message}");
                throw;
            }
        }

        public void Remove(string procedureName, int id)
        {
            try
            {
                using (var connection = _databaseConnection.GetConnection())
                {
                    connection.Execute(procedureName, new { id }, commandType: System.Data.CommandType.StoredProcedure);
                }
            }
            catch (SqlException sqlEx)
            {
                Console.WriteLine($"SQL error occurred: {sqlEx.Message}");
                throw;
            }
            catch (Exception ex)
            {
                Console.WriteLine($"An error occurred: {ex.Message}");
                throw;
            }
        }

        public void BulkRemove(string procedureName, int[] ids)
        {
            try
            {
                using (var connection = _databaseConnection.GetConnection())
                {
                    var intList = new IntList(ids);
                    var parameters = new DynamicParameters();
                    parameters.Add("ids", intList.AsTableValuedParameter("dbo.IntList"));
                    connection.Execute(procedureName, parameters, commandType: System.Data.CommandType.StoredProcedure);
                }
            }
            catch (SqlException sqlEx)
            {
                Console.WriteLine($"SQL error occurred: {sqlEx.Message}");
                throw;
            }
            catch (Exception ex)
            {
                Console.WriteLine($"An error occurred: {ex.Message}");
                throw;
            }
        }
    }
}
