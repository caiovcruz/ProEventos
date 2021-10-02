using System.Threading.Tasks;
using ProEventos.Persistence.Contextos;
using ProEventos.Persistence.Contratos;

namespace ProEventos.Persistence
{
    public class GeralPersist : IGeralPersist
    {
        private readonly ProEventosContext _context;
        public GeralPersist(ProEventosContext context)
        {
            _context = context;

        }

        #region Add
        /// <summary>
        /// Add
        /// </summary>
        /// <typeparam name="T"></typeparam>
        public void Add<T>(T entity) where T : class
        {
            _context.Add(entity);
        }
        #endregion

        #region Update
        /// <summary>
        /// Update
        /// </summary>
        /// <typeparam name="T"></typeparam>
        public void Update<T>(T entity) where T : class
        {
            _context.Update(entity);
        }
        #endregion

        #region Delete
        /// <summary>
        /// Delete
        /// </summary>
        /// <typeparam name="T"></typeparam>
        public void Delete<T>(T entity) where T : class
        {
            _context.Remove(entity);
        }
        #endregion

        #region DeleteRange
        /// <summary>
        /// DeleteRange
        /// </summary>
        /// <typeparam name="T"></typeparam>
        public void DeleteRange<T>(T[] entityArray) where T : class
        {
            _context.RemoveRange(entityArray);
        }
        #endregion

        #region SaveChangesAsync
        /// <summary>
        /// SaveChangesAsync
        /// </summary>
        /// <returns></returns>
        public async Task<bool> SaveChangesAsync()
        {
            return (await _context.SaveChangesAsync()) > 0;
        }
        #endregion
    }
}
