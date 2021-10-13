using System;
using System.Threading.Tasks;
using AutoMapper;
using ProEventos.Application.Contratos;
using ProEventos.Application.DTOs;
using ProEventos.Domain;
using ProEventos.Persistence.Contratos;

namespace ProEventos.Application
{
    public class EventoService : IEventoService
    {
        private readonly IGeralPersist _geralPersist;
        private readonly IEventoPersist _eventoPersist;
        private readonly IMapper _mapper;
        public EventoService(IGeralPersist geralPersist, IEventoPersist eventoPersist, IMapper mapper)
        {
            _eventoPersist = eventoPersist;
            _geralPersist = geralPersist;
            _mapper = mapper;
        }

        #region AddEvento
        /// <summary>
        /// AddEvento
        /// </summary>
        /// <param name="model"></param>
        /// <returns></returns>
        public async Task<EventoDTO> AddEvento(EventoDTO model)
        {
            try
            {
                var evento = _mapper.Map<Evento>(model);

                _geralPersist.Add<Evento>(evento);

                if (await _geralPersist.SaveChangesAsync())
                {
                    var eventoRetorno = await _eventoPersist.GetEventoByIdAsync(evento.Id, false);

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
        public async Task<EventoDTO> UpdateEvento(int eventoId, EventoDTO model)
        {
            try
            {
                var evento = await _eventoPersist.GetEventoByIdAsync(eventoId, false);
                if (evento == null) return null;

                model.Id = eventoId;

                _mapper.Map(model, evento);

                _geralPersist.Update<Evento>(evento);

                if (await _geralPersist.SaveChangesAsync())
                {
                    var eventoRetorno = await _eventoPersist.GetEventoByIdAsync(evento.Id, false);

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
        public async Task<bool> DeleteEvento(int eventoId)
        {
            try
            {
                var evento = await _eventoPersist.GetEventoByIdAsync(eventoId, false);
                if (evento == null) throw new Exception("Evento para delete não encontrado.");

                _geralPersist.Delete<Evento>(evento);
                return await _geralPersist.SaveChangesAsync();
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
        /// <param name="includePalestrantes"></param>
        /// <returns></returns>
        public async Task<EventoDTO[]> GetAllEventosAsync(bool includePalestrantes = false)
        {
            try
            {
                var eventos = await _eventoPersist.GetAllEventosAsync(includePalestrantes);
                if (eventos == null) return null;

                return _mapper.Map<EventoDTO[]>(eventos);
            }
            catch (Exception ex)
            {
                throw new Exception(ex.Message);
            }
        }
        #endregion

        #region GetAllEventosByTemaAsync
        public async Task<EventoDTO[]> GetAllEventosByTemaAsync(string tema, bool includePalestrantes = false)
        {
            try
            {
                var eventos = await _eventoPersist.GetAllEventosByTemaAsync(tema, includePalestrantes);
                if (eventos == null) return null;

                return _mapper.Map<EventoDTO[]>(eventos);
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
        public async Task<EventoDTO> GetEventoByIdAsync(int eventoId, bool includePalestrantes = false)
        {
            try
            {
                var evento = await _eventoPersist.GetEventoByIdAsync(eventoId, includePalestrantes);
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