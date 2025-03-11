﻿using Dapper;

namespace RecipeBook.Models
{
    abstract class Storable
    {
        public int Id { get; set; }
        public string Name { get; set; } = string.Empty;
        public string? Description { get; set; }
        public virtual DynamicParameters ToDynamicParameters()
        {
            var parameters = new DynamicParameters();
            parameters.Add("Name", Name);
            parameters.Add("Description", Description);
            return parameters;
        }
        protected abstract void FromReader(SqlMapper.GridReader reader);
        public static T? FromReader<T>(SqlMapper.GridReader reader) where T : Storable
        {
            var entity = reader.ReadSingleOrDefault<T>();
            if (entity is null) return null;
            entity.FromReader(reader);
            return entity;
        }
    }
}
