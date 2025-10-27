using Ignis.Data;

namespace Ignis.Models.Commons
{
    public class Util
    {
        private AppDbContext _db;

        public Util(AppDbContext db)
        {
            _db = db;
        }

        /// <summary>
        /// Adds or updates an entity in the database asynchronously.
        /// 
        /// This generic method accepts an entity of type <typeparamref name="T"/> (where T is a class)
        /// that contains a property named either "ID" or "Id" of type int. It uses reflection to inspect
        /// the entity's identifier. If the identifier is 0, the entity is considered new and is added to
        /// the DbContext; otherwise, it is treated as an existing entity and is updated. After processing,
        /// the changes are persisted to the database.
        /// 
        /// The type of the entity. Must be a class with an "ID" or "Id" property of type int.      
        /// Exceptions:
        /// An exception is thrown if the entity does not contain an "ID" or "Id" property.
        /// </summary>

        /// <returns>The added or updated entity.</returns>
        public async Task<T> AddUpdate<T>(T data) where T : class
        {
            // get the property info for "ID" or "Id"
            var type = typeof(T);
            var idProperty = type.GetProperty("ID") ?? type.GetProperty("Id");
            if (idProperty == null)
            {
                throw new ArgumentException("Entity must have a property named 'ID' or 'Id'.");
            }

            // assuming the Id property is of type int.
            int idValue = (int)idProperty.GetValue(data);

            // get the correct DbSet for the type T
            var dbSet = _db.Set<T>();

            if (idValue == 0)
            {
                // For new entities, add it to the DbContext.
                await dbSet.AddAsync(data);
            }
            else
            {
                // For existing entities, update it in the DbContext.
                dbSet.Update(data);
            }

            await _db.SaveChangesAsync();
            return data;
        }
    }
}
