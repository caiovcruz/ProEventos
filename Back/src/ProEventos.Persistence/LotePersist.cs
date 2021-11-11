using System.Linq;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using ProEventos.Domain;
using ProEventos.Persistence.Contextos;
using ProEventos.Persistence.Contratos;

namespace ProEventos.Persistence
{
    public class LotePersist : GeralPersist, ILotePersist
    {
        private readonly ProEventosContext _context;
        public LotePersist(ProEventosContext context) : base(context)
        {
            _context = context;
            //_context.ChangeTracker.QueryTrackingBehavior = QueryTrackingBehavior.NoTracking;
        }

        #region GetLoteByIdsAsync
        /// <summary>
        /// GetLoteByIdsAsync
        /// </summary>
        /// <param name="eventoId"></param>
        /// <param name="id"></param>
        /// <returns></returns>
        public async Task<Lote> GetLoteByIdsAsync(int eventoId, int loteId)
        {
            IQueryable<Lote> query = _context.Lotes;

            query = query.AsNoTracking()
                         .Where(lote => lote.EventoId == eventoId && lote.Id == loteId);

            return await query.FirstOrDefaultAsync();
        }
        #endregion

        #region GetLotesByEventoIdAsync
        /// <summary>
        /// GetLotesByEventoIdAsync
        /// </summary>
        /// <param name="eventoId"></param>
        /// <returns></returns>
        public async Task<Lote[]> GetLotesByEventoIdAsync(int eventoId)
        {
            IQueryable<Lote> query = _context.Lotes;

            query = query.AsNoTracking()
                         .Where(lote => lote.EventoId == eventoId);

            return await query.ToArrayAsync();
        }
        #endregion
    }
}
