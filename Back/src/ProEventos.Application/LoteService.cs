using System;
using System.Linq;
using System.Threading.Tasks;
using AutoMapper;
using ProEventos.Application.Contratos;
using ProEventos.Application.DTOs;
using ProEventos.Domain;
using ProEventos.Persistence.Contratos;

namespace ProEventos.Application
{
    public class LoteService : ILoteService
    {
        private readonly IGeralPersist _geralPersist;
        private readonly ILotePersist _lotePersist;
        private readonly IMapper _mapper;
        public LoteService(IGeralPersist geralPersist, ILotePersist lotePersist, IMapper mapper)
        {
            _lotePersist = lotePersist;
            _geralPersist = geralPersist;
            _mapper = mapper;
        }

        #region SaveLotes
        /// <summary>
        /// SaveLotes
        /// </summary>
        /// <param name="eventoId"></param>
        /// <param name="models"></param>
        /// <returns></returns>
        public async Task<LoteDTO[]> SaveLotes(int eventoId, LoteDTO[] models)
        {
            try
            {
                var lotes = await _lotePersist.GetLotesByEventoIdAsync(eventoId);
                if (lotes == null) return null;

                foreach (var item in models)
                {
                    if (item.Id == 0)
                    {
                        await AddLote(eventoId, item);
                    }
                    else
                    {
                        await UpdateLote(eventoId, lotes.FirstOrDefault(lote => lote.Id == item.Id), item);
                    }
                }

                var lotesRetorno = await _lotePersist.GetLotesByEventoIdAsync(eventoId);

                return _mapper.Map<LoteDTO[]>(lotesRetorno);
            }
            catch (Exception ex)
            {
                throw new Exception(ex.Message);

            }
        }

        #region AddLote
        /// <summary>
        /// AddLote
        /// </summary>
        /// <param name="eventoId"></param>
        /// <param name="model"></param>
        /// <returns></returns>
        public async Task AddLote(int eventoId, LoteDTO model)
        {
            try
            {
                var lote = _mapper.Map<Lote>(model);
                lote.EventoId = eventoId;

                _geralPersist.Add<Lote>(lote);

                await _geralPersist.SaveChangesAsync();
            }
            catch (Exception ex)
            {
                throw new Exception(ex.Message);
            }
        }
        #endregion

        #region UpdateLote
        /// <summary>
        /// UpdateLote
        /// </summary>
        /// <param name="loteAtual"></param>
        /// <param name="loteAtualizado"></param>
        /// <returns></returns>
        public async Task UpdateLote(int eventoId, Lote loteAtual, LoteDTO loteAtualizado)
        {
            try
            {
                _mapper.Map(loteAtualizado, loteAtual);
                loteAtual.EventoId = eventoId;

                _geralPersist.Update<Lote>(loteAtual);

                await _geralPersist.SaveChangesAsync();
            }
            catch (Exception ex)
            {
                throw new Exception(ex.Message);
            }
        }
        #endregion

        #endregion

        #region DeleteLote
        /// <summary>
        /// DeleteLote
        /// </summary>
        /// <param name="eventoId"></param>
        /// <param name="loteId"></param>
        /// <returns></returns>
        public async Task<bool> DeleteLote(int eventoId, int loteId)
        {
            try
            {
                var lote = await _lotePersist.GetLoteByIdsAsync(eventoId, loteId);
                if (lote == null) throw new Exception("Lote para delete não encontrado.");

                _geralPersist.Delete<Lote>(lote);
                return await _geralPersist.SaveChangesAsync();
            }
            catch (System.Exception)
            {

                throw;
            }
        }
        #endregion

        #region GetLotesByEventoIdAsync
        /// <summary>
        /// GetLotesByEventoIdAsync
        /// </summary>
        /// <param name="eventoId"></param>
        /// <returns></returns>
        public async Task<LoteDTO[]> GetLotesByEventoIdAsync(int eventoId)
        {
            try
            {
                var lotes = await _lotePersist.GetLotesByEventoIdAsync(eventoId);
                if (lotes == null) return null;

                return _mapper.Map<LoteDTO[]>(lotes);
            }
            catch (Exception ex)
            {
                throw new Exception(ex.Message);
            }
        }
        #endregion

        #region GetLoteByIdsAsync
        /// <summary>
        /// GetLoteByIdsAsync
        /// </summary>
        /// <param name="eventoId"></param>
        /// <param name="loteId"></param>
        /// <returns></returns>
        public async Task<LoteDTO> GetLoteByIdsAsync(int eventoId, int loteId)
        {
            try
            {
                var lote = await _lotePersist.GetLoteByIdsAsync(eventoId, loteId);
                if (lote == null) return null;

                return _mapper.Map<LoteDTO>(lote);
            }
            catch (Exception ex)
            {
                throw new Exception(ex.Message);
            }
        }
        #endregion
    }
}