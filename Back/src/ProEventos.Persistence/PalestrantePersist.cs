using System.Linq;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using ProEventos.Domain;
using ProEventos.Domain.Enum;
using ProEventos.Persistence.Contextos;
using ProEventos.Persistence.Contratos;
using ProEventos.Persistence.Models;

namespace ProEventos.Persistence
{
    public class PalestrantePersist : GeralPersist, IPalestrantePersist
    {
        private readonly ProEventosContext _context;
        public PalestrantePersist(ProEventosContext context) : base(context)
        {
            _context = context;

        }

        #region GetAllPalestrantesAsync
        /// <summary>
        /// GetAllPalestrantesAsync
        /// </summary>
        /// <param name="includeEventos"></param>
        /// <returns></returns>
        public async Task<PageList<Palestrante>> GetAllPalestrantesAsync(PageParams pageParams, bool includeEventos = false)
        {
            IQueryable<Palestrante> query = _context.Palestrantes
                                                    .Include(p => p.User)
                                                    .Include(p => p.RedesSociais);

            if (includeEventos)
            {
                query = query
                        .Include(p => p.PalestrantesEventos)
                        .ThenInclude(pe => pe.Evento);
            }

            query = query.AsNoTracking()
                         .Where(p => (p.MiniCurriculo.ToLower().Contains(pageParams.Term.ToLower()) ||
                                      p.User.PrimeiroNome.ToLower().Contains(pageParams.Term.ToLower()) ||
                                      p.User.UltimoNome.ToLower().Contains(pageParams.Term.ToLower())) &&
                                      p.User.Funcao == Funcao.Palestrante)
                         .OrderBy(p => p.Id);

            return await PageList<Palestrante>.CreateAsync(query, pageParams.PageNumber, pageParams.PageSize);
        }
        #endregion

        #region GetPalestranteByIdAsync
        /// <summary>
        /// GetPalestranteByIdAsync
        /// </summary>
        /// <param name="PalestranteId"></param>
        /// <param name="includeEventos"></param>
        /// <returns></returns>
        public async Task<Palestrante> GetPalestranteByUserIdAsync(int userId, bool includeEventos = false)
        {
            IQueryable<Palestrante> query = _context.Palestrantes
                                                    .Include(p => p.User)
                                                    .Include(p => p.RedesSociais);

            if (includeEventos)
            {
                query = query
                        .Include(p => p.PalestrantesEventos)
                        .ThenInclude(pe => pe.Evento);
            }

            query = query.AsNoTracking()
                         .Where(p => p.UserId == userId)
                         .OrderBy(p => p.Id);

            return await query.FirstOrDefaultAsync();
        }
        #endregion
    }
}
