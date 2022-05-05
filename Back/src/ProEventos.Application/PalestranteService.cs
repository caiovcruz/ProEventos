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
    public class PalestranteService : IPalestranteService
    {
        private readonly IPalestrantePersist _palestrantePersist;
        private readonly IMapper _mapper;
        public PalestranteService(IPalestrantePersist palestrantePersist, IMapper mapper)
        {
            _palestrantePersist = palestrantePersist;
            _mapper = mapper;
        }

        #region AddPalestrante
        /// <summary>
        /// AddPalestrante
        /// </summary>
        /// <param name="model"></param>
        /// <returns></returns>
        public async Task<PalestranteDTO> AddPalestrante(int userId, PalestranteAddDTO model)
        {
            try
            {
                var palestrante = _mapper.Map<Palestrante>(model);
                palestrante.UserId = userId;

                _palestrantePersist.Add<Palestrante>(palestrante);

                if (await _palestrantePersist.SaveChangesAsync())
                {
                    var palestranteRetorno = await _palestrantePersist.GetPalestranteByUserIdAsync(userId, false);

                    return _mapper.Map<PalestranteDTO>(palestranteRetorno);
                }

                return null;
            }
            catch (Exception ex)
            {
                throw new Exception(ex.Message);
            }
        }
        #endregion

        #region UpdatePalestrante
        /// <summary>
        /// UpdatePalestrante
        /// </summary>
        /// <param name="palestranteId"></param>
        /// <param name="model"></param>
        /// <returns></returns>
        public async Task<PalestranteDTO> UpdatePalestrante(int userId, PalestranteUpdateDTO model)
        {
            try
            {
                var palestrante = await _palestrantePersist.GetPalestranteByUserIdAsync(userId, false);
                if (palestrante == null) return null;

                model.Id = palestrante.Id;
                model.UserId = userId;

                _mapper.Map(model, palestrante);

                _palestrantePersist.Update<Palestrante>(palestrante);

                if (await _palestrantePersist.SaveChangesAsync())
                {
                    var palestranteRetorno = await _palestrantePersist.GetPalestranteByUserIdAsync(userId, false);

                    return _mapper.Map<PalestranteDTO>(palestranteRetorno);
                }

                return null;
            }
            catch (Exception ex)
            {
                throw new Exception(ex.Message);

            }
        }
        #endregion

        #region GetAllPalestrantesAsync
        /// <summary>
        /// GetAllPalestrantesAsync
        /// </summary>
        /// <param name="pageParams"></param>
        /// <param name="includeEventos"></param>
        /// <returns></returns>
        public async Task<PageList<PalestranteDTO>> GetAllPalestrantesAsync(PageParams pageParams, bool includeEventos = false)
        {
            try
            {
                var palestrantes = await _palestrantePersist.GetAllPalestrantesAsync(pageParams, includeEventos);
                if (palestrantes == null) return null;

                var palestrantesMapped = _mapper.Map<PageList<PalestranteDTO>>(palestrantes);

                palestrantesMapped.CurrentPage = palestrantes.CurrentPage;
                palestrantesMapped.TotalPages = palestrantes.TotalPages;
                palestrantesMapped.PageSize = palestrantes.PageSize;
                palestrantesMapped.TotalCount = palestrantes.TotalCount;

                return palestrantesMapped;
            }
            catch (Exception ex)
            {
                throw new Exception(ex.Message);
            }
        }
        #endregion

        #region GetPalestranteByUserIdAsync
        /// <summary>
        /// GetPalestranteByIdAsync
        /// </summary>
        /// <param name="palestranteId"></param>
        /// <param name="includeEventos"></param>
        /// <returns></returns>
        public async Task<PalestranteDTO> GetPalestranteByUserIdAsync(int userId, bool includeEventos = false)
        {
            try
            {
                var palestrante = await _palestrantePersist.GetPalestranteByUserIdAsync(userId, includeEventos);
                if (palestrante == null) return null;

                return _mapper.Map<PalestranteDTO>(palestrante);
            }
            catch (Exception ex)
            {
                throw new Exception(ex.Message);
            }
        }
        #endregion
    }
}