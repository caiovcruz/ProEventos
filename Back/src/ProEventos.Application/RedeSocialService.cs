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
    public class RedeSocialService : IRedeSocialService
    {
        private readonly IRedeSocialPersist _redeSocialPersist;
        private readonly IMapper _mapper;
        public RedeSocialService(IRedeSocialPersist redeSocialPersist, IMapper mapper)
        {
            _redeSocialPersist = redeSocialPersist;
            _mapper = mapper;
        }

        #region SaveBy
        /// <summary>
        /// SaveByEvento
        /// </summary>
        /// <param name="eventoId"></param>
        /// <param name="models"></param>
        /// <returns></returns>
        public async Task<RedeSocialDTO[]> SaveByEvento(int eventoId, RedeSocialDTO[] models)
        {
            try
            {
                var redesSociais = await _redeSocialPersist.GetAllByEventoIdAsync(eventoId);
                if (redesSociais == null) return null;

                foreach (var item in models)
                {
                    if (item.Id == 0)
                    {
                        await AddRedeSocial(eventoId, item, true);
                    }
                    else
                    {
                        await UpdateRedeSocial(eventoId, redesSociais.FirstOrDefault(redeSocial => redeSocial.Id == item.Id), item, true);
                    }
                }

                var redesSociaisRetorno = await _redeSocialPersist.GetAllByEventoIdAsync(eventoId);

                return _mapper.Map<RedeSocialDTO[]>(redesSociaisRetorno);
            }
            catch (Exception ex)
            {
                throw new Exception(ex.Message);

            }
        }

        /// <summary>
        /// SaveByPalestrante
        /// </summary>
        /// <param name="palestranteId"></param>
        /// <param name="models"></param>
        /// <returns></returns>
        public async Task<RedeSocialDTO[]> SaveByPalestrante(int palestranteId, RedeSocialDTO[] models)
        {
            try
            {
                var redesSociais = await _redeSocialPersist.GetAllByPalestranteIdAsync(palestranteId);
                if (redesSociais == null) return null;

                foreach (var item in models)
                {
                    if (item.Id == 0)
                    {
                        await AddRedeSocial(palestranteId, item, false);
                    }
                    else
                    {
                        await UpdateRedeSocial(palestranteId, redesSociais.FirstOrDefault(redeSocial => redeSocial.Id == item.Id), item, false);
                    }
                }

                var redesSociaisRetorno = await _redeSocialPersist.GetAllByPalestranteIdAsync(palestranteId);

                return _mapper.Map<RedeSocialDTO[]>(redesSociaisRetorno);
            }
            catch (Exception ex)
            {
                throw new Exception(ex.Message);

            }
        }

        /// <summary>
        /// AddRedeSocial
        /// </summary>
        /// <param name="id"></param>
        /// <param name="model"></param>
        /// <param name="isEvento"></param>
        /// <returns></returns>
        public async Task AddRedeSocial(int id, RedeSocialDTO model, bool isEvento)
        {
            try
            {
                var redeSocial = _mapper.Map<RedeSocial>(model);

                redeSocial.EventoId = isEvento ? id : null;
                redeSocial.PalestranteId = !isEvento ? id : null;

                _redeSocialPersist.Add<RedeSocial>(redeSocial);

                await _redeSocialPersist.SaveChangesAsync();
            }
            catch (Exception ex)
            {
                throw new Exception(ex.Message);
            }
        }

        /// <summary>
        /// UpdateRedeSocial
        /// </summary>
        /// <param name="id"></param>
        /// <param name="redeSocialAtual"></param>
        /// <param name="redeSocialAtualizado"></param>
        /// <param name="isEvento"></param>
        /// <returns></returns>
        public async Task UpdateRedeSocial(int id, RedeSocial redeSocialAtual, RedeSocialDTO redeSocialAtualizado, bool isEvento)
        {
            try
            {
                _mapper.Map(redeSocialAtualizado, redeSocialAtual);

                redeSocialAtual.EventoId = isEvento ? id : null;
                redeSocialAtual.PalestranteId = !isEvento ? id : null;

                _redeSocialPersist.Update<RedeSocial>(redeSocialAtual);

                await _redeSocialPersist.SaveChangesAsync();
            }
            catch (Exception ex)
            {
                throw new Exception(ex.Message);
            }
        }

        #endregion

        #region DeleteBy
        /// <summary>
        /// DeleteByEvento
        /// </summary>
        /// <param name="eventoId"></param>
        /// <param name="redeSocialId"></param>
        /// <returns></returns>
        public async Task<bool> DeleteByEvento(int eventoId, int redeSocialId)
        {
            try
            {
                var redeSocial = await _redeSocialPersist.GetRedeSocialEventoByIdsAsync(eventoId, redeSocialId);
                if (redeSocial == null) throw new Exception("Rede Social para delete não encontrado.");

                _redeSocialPersist.Delete<RedeSocial>(redeSocial);
                return await _redeSocialPersist.SaveChangesAsync();
            }
            catch (System.Exception)
            {

                throw;
            }
        }
        
        /// <summary>
        /// DeleteByPalestrante
        /// </summary>
        /// <param name="palestranteId"></param>
        /// <param name="redeSocialId"></param>
        /// <returns></returns>
        public async Task<bool> DeleteByPalestrante(int palestranteId, int redeSocialId)
        {
            try
            {
                var redeSocial = await _redeSocialPersist.GetRedeSocialPalestranteByIdsAsync(palestranteId, redeSocialId);
                if (redeSocial == null) throw new Exception("Rede Social para delete não encontrado.");

                _redeSocialPersist.Delete<RedeSocial>(redeSocial);
                return await _redeSocialPersist.SaveChangesAsync();
            }
            catch (System.Exception)
            {

                throw;
            }
        }
        #endregion

        #region GetAllById
        /// <summary>
        /// GetAllByEventoIdAsync
        /// </summary>
        /// <param name="eventoId"></param>
        /// <returns></returns>
        public async Task<RedeSocialDTO[]> GetAllByEventoIdAsync(int eventoId)
        {
            try
            {
                var redesSociais = await _redeSocialPersist.GetAllByEventoIdAsync(eventoId);
                if (redesSociais == null) return null;

                return _mapper.Map<RedeSocialDTO[]>(redesSociais);
            }
            catch (Exception ex)
            {
                throw new Exception(ex.Message);
            }
        }
        
        /// <summary>
        /// GetAllByPalestranteIdAsync
        /// </summary>
        /// <param name="palestranteId"></param>
        /// <returns></returns>
        public async Task<RedeSocialDTO[]> GetAllByPalestranteIdAsync(int palestranteId)
        {
            try
            {
                var redesSociais = await _redeSocialPersist.GetAllByPalestranteIdAsync(palestranteId);
                if (redesSociais == null) return null;

                return _mapper.Map<RedeSocialDTO[]>(redesSociais);
            }
            catch (Exception ex)
            {
                throw new Exception(ex.Message);
            }
        }
        #endregion

        #region GetByIds
        /// <summary>
        /// GetRedeSocialEventoByIdsAsync
        /// </summary>
        /// <param name="eventoId"></param>
        /// <param name="redeSocialId"></param>
        /// <returns></returns>
        public async Task<RedeSocialDTO> GetRedeSocialEventoByIdsAsync(int eventoId, int redeSocialId)
        {
            try
            {
                var redeSocial = await _redeSocialPersist.GetRedeSocialEventoByIdsAsync(eventoId, redeSocialId);
                if (redeSocial == null) return null;

                return _mapper.Map<RedeSocialDTO>(redeSocial);
            }
            catch (Exception ex)
            {
                throw new Exception(ex.Message);
            }
        }

        
        /// <summary>
        /// GetRedeSocialEventoByIdsAsync
        /// </summary>
        /// <param name="palestranteId"></param>
        /// <param name="redeSocialId"></param>
        /// <returns></returns>
        public async Task<RedeSocialDTO> GetRedeSocialPalestranteByIdsAsync(int palestranteId, int redeSocialId)
        {
            try
            {
                var redeSocial = await _redeSocialPersist.GetRedeSocialPalestranteByIdsAsync(palestranteId, redeSocialId);
                if (redeSocial == null) return null;

                return _mapper.Map<RedeSocialDTO>(redeSocial);
            }
            catch (Exception ex)
            {
                throw new Exception(ex.Message);
            }
        }
        #endregion
    }
}