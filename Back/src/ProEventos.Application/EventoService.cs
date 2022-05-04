using System;
using System.Threading.Tasks;
using AutoMapper;
using ProEventos.Application.Contratos;
using ProEventos.Application.DTOs;
using ProEventos.Domain;
using ProEventos.Persistence.Contratos;
using ProEventos.Persistence.Models;

namespace ProEventos.Application
{
    public class EventoService : IEventoService
    {
        private readonly IEventoPersist _eventoPersist;
        private readonly IMapper _mapper;
        public EventoService(IEventoPersist eventoPersist, IMapper mapper)
        {
            _eventoPersist = eventoPersist;
            _mapper = mapper;
        }

        #region AddEvento
        /// <summary>
        /// AddEvento
        /// </summary>
        /// <param name="model"></param>
        /// <returns></returns>
        public async Task<EventoDTO> AddEvento(int userId, EventoDTO model)
        {
            try
            {
                var evento = _mapper.Map<Evento>(model);
                evento.UserId = userId;

                _eventoPersist.Add<Evento>(evento);

                if (await _eventoPersist.SaveChangesAsync())
                {
                    var eventoRetorno = await _eventoPersist.GetEventoByIdAsync(userId, evento.Id, false);

                    return _mapper.Map<EventoDTO>(eventoRetorno);
                }

                return null;
            }
            catch (Exception ex)
            {
                throw new Exception(ex.Message);
            }
        }
        #endregion

        #region UpdateEvento
        /// <summary>
        /// UpdateEvento
        /// </summary>
        /// <param name="eventoId"></param>
        /// <param name="model"></param>
        /// <returns></returns>
        public async Task<EventoDTO> UpdateEvento(int userId, int eventoId, EventoDTO model)
        {
            try
            {
                var evento = await _eventoPersist.GetEventoByIdAsync(userId, eventoId, false);
                if (evento == null) return null;

                model.Id = eventoId;
                model.UserId = userId;

                _mapper.Map(model, evento);

                _eventoPersist.Update<Evento>(evento);

                if (await _eventoPersist.SaveChangesAsync())
                {
                    var eventoRetorno = await _eventoPersist.GetEventoByIdAsync(userId, evento.Id, false);

                    return _mapper.Map<EventoDTO>(eventoRetorno);
                }

                return null;
            }
            catch (Exception ex)
            {
                throw new Exception(ex.Message);

            }
        }
        #endregion

        #region DeleteEvento
        /// <summary>
        /// DeleteEvento
        /// </summary>
        /// <param name="eventoId"></param>
        /// <returns></returns>
        public async Task<bool> DeleteEvento(int userId, int eventoId)
        {
            try
            {
                var evento = await _eventoPersist.GetEventoByIdAsync(userId, eventoId, false);
                if (evento == null) throw new Exception("Evento para delete não encontrado.");

                _eventoPersist.Delete<Evento>(evento);
                return await _eventoPersist.SaveChangesAsync();
            }
            catch (Exception ex)
            {
                throw new Exception(ex.Message);

            }
        }
        #endregion

        #region GetAllEventosAsync
        /// <summary>
        /// GetAllEventosAsync
        /// </summary>
        /// <param name="userId"></param>
        /// <param name="pageParams"></param>
        /// <param name="includePalestrantes"></param>
        /// <returns></returns>
        public async Task<PageList<EventoDTO>> GetAllEventosAsync(int userId, PageParams pageParams, bool includePalestrantes = false)
        {
            try
            {
                var eventos = await _eventoPersist.GetAllEventosAsync(userId, pageParams, includePalestrantes);
                if (eventos == null) return null;

                var eventosMapped = _mapper.Map<PageList<EventoDTO>>(eventos);

                eventosMapped.CurrentPage = eventos.CurrentPage;
                eventosMapped.TotalPages = eventos.TotalPages;
                eventosMapped.PageSize = eventos.PageSize;
                eventosMapped.TotalCount = eventos.TotalCount;

                return eventosMapped;
            }
            catch (Exception ex)
            {
                throw new Exception(ex.Message);
            }
        }
        #endregion

        #region GetEventoByIdAsync
        /// <summary>
        /// GetEventoByIdAsync
        /// </summary>
        /// <param name="eventoId"></param>
        /// <param name="includePalestrantes"></param>
        /// <returns></returns>
        public async Task<EventoDTO> GetEventoByIdAsync(int userId, int eventoId, bool includePalestrantes = false)
        {
            try
            {
                var evento = await _eventoPersist.GetEventoByIdAsync(userId, eventoId, includePalestrantes);
                if (evento == null) return null;

                return _mapper.Map<EventoDTO>(evento);
            }
            catch (Exception ex)
            {
                throw new Exception(ex.Message);
            }
        }
        #endregion
    }
}