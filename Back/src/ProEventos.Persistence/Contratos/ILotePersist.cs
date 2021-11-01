using System.Threading.Tasks;
using ProEventos.Domain;

namespace ProEventos.Persistence.Contratos
{
    public interface ILotePersist
    {
        #region GetLotesByEventoIdAsync
        /// <summary>
        /// Método que retornará um array de lotes por evento
        /// </summary>
        /// <param name="eventoId">Código do evento</param>
        /// <returns>Array de lotes</returns>
        Task<Lote[]> GetLotesByEventoIdAsync(int eventoId);
        #endregion

        #region GetLoteByIdsAsync
        /// <summary>
        /// Método que retornará apenas 1 lote
        /// </summary>
        /// <param name="eventoId">Código do evento</param>
        /// <param name="id">Código do lote</param>
        /// <returns>Apenas 1 lote</returns>
        Task<Lote> GetLoteByIdsAsync(int eventoId, int loteId);
        #endregion
    }
}