using System;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using ProEventos.Application.Contratos;
using Microsoft.AspNetCore.Http;
using ProEventos.Application.DTOs;
using Microsoft.AspNetCore.Authorization;
using ProEventos.API.Extensions;

namespace ProEventos.API.Controllers
{
    [Authorize]
    [ApiController]
    [Route("api/[controller]")]
    public class RedesSociaisController : ControllerBase
    {
        private readonly IRedeSocialService _redeSocialService;
        private readonly IEventoService _eventoService;
        private readonly IPalestranteService _palestranteService;
        public RedesSociaisController(IRedeSocialService redeSocialService,
                                      IEventoService eventoService,
                                      IPalestranteService palestranteService)
        {
            _redeSocialService = redeSocialService;
            _eventoService = eventoService;
            _palestranteService = palestranteService;
        }

        #region GetByEvento
        /// <summary>
        /// GetByEvento
        /// </summary>
        /// <param name="eventoId"></param>
        /// <returns></returns>
        [HttpGet("evento/{eventoId}")]
        public async Task<IActionResult> GetByEvento(int eventoId)
        {
            try
            {
                if (!(await AutorEvento(eventoId))) return Unauthorized();

                var redesSociais = await _redeSocialService.GetAllByEventoIdAsync(eventoId);
                if (redesSociais == null) return NoContent();

                return Ok(redesSociais);
            }
            catch (Exception ex)
            {
                return this.StatusCode(StatusCodes.Status500InternalServerError,
                $"Erro ao tentar recuperar redes sociais. Erro: {ex.Message}");
            }
        }

        /// <summary>
        /// AutorEvento
        /// /// </summary>
        /// <param name="eventoId"></param>
        /// <returns></returns>
        [NonAction]
        private async Task<bool> AutorEvento(int eventoId)
        {
            var evento = await _eventoService.GetEventoByIdAsync(User.GetUserId(), eventoId, false);
            return evento == null ? false : true;
        }
        #endregion

        #region GetByPalestrante
        /// <summary>
        /// GetByPalestrante
        /// </summary>
        /// <returns></returns>
        [HttpGet("palestrante")]
        public async Task<IActionResult> GetByPalestrante()
        {
            try
            {
                var palestrante = await _palestranteService.GetPalestranteByUserIdAsync(User.GetUserId(), false);
                if (palestrante == null) return Unauthorized();

                var redesSociais = await _redeSocialService.GetAllByPalestranteIdAsync(palestrante.Id);
                if (redesSociais == null) return NoContent();

                return Ok(redesSociais);
            }
            catch (Exception ex)
            {
                return this.StatusCode(StatusCodes.Status500InternalServerError,
                $"Erro ao tentar recuperar redes sociais. Erro: {ex.Message}");
            }
        }
        #endregion

        #region SaveByEvento
        /// <summary>
        /// SaveByEvento
        /// </summary>
        /// <param name="eventoId"></param>
        /// <param name="models"></param>
        /// <returns></returns>
        [HttpPut("evento/{eventoId}")]
        public async Task<IActionResult> SaveByEvento(int eventoId, RedeSocialDTO[] models)
        {
            try
            {
                if (!(await AutorEvento(eventoId))) return Unauthorized();

                var redesSociais = await _redeSocialService.SaveByEvento(eventoId, models);
                if (redesSociais == null) return NoContent();

                return Ok(redesSociais);
            }
            catch (Exception ex)
            {
                return this.StatusCode(StatusCodes.Status500InternalServerError,
                $"Erro ao tentar salvar redes sociais. Erro: {ex.Message}");
            }
        }
        #endregion

        #region SaveByPalestrante
        /// <summary>
        /// SaveByPalestrante
        /// </summary>
        /// <param name="models"></param>
        /// <returns></returns>
        [HttpPut("palestrante")]
        public async Task<IActionResult> SaveByPalestrante(RedeSocialDTO[] models)
        {
            try
            {
                var palestrante = await _palestranteService.GetPalestranteByUserIdAsync(User.GetUserId(), false);
                if (palestrante == null) return Unauthorized();

                var redesSociais = await _redeSocialService.SaveByPalestrante(palestrante.Id, models);
                if (redesSociais == null) return NoContent();

                return Ok(redesSociais);
            }
            catch (Exception ex)
            {
                return this.StatusCode(StatusCodes.Status500InternalServerError,
                $"Erro ao tentar salvar redes sociais. Erro: {ex.Message}");
            }
        }
        #endregion

        #region DeleteByEvento
        /// <summary>
        /// DeleteByEvento
        /// </summary>
        /// <param name="eventoId"></param>
        /// <param name="redeSocialId"></param>
        /// <returns></returns>
        [HttpDelete("evento/{eventoId}/{redeSocialId}")]
        public async Task<IActionResult> DeleteByEvento(int eventoId, int redeSocialId)
        {
            try
            {
                if (!(await AutorEvento(eventoId))) return Unauthorized();

                var redeSocial = await _redeSocialService.GetRedeSocialEventoByIdsAsync(eventoId, redeSocialId);
                if (redeSocial == null) return NoContent();

                return await _redeSocialService.DeleteByEvento(eventoId, redeSocial.Id)
                    ? Ok(new { message = "Deletado" })
                    : throw new Exception("Ocorreu um problema não específico ao tentar deletar rede social.");
            }
            catch (Exception ex)
            {
                return this.StatusCode(StatusCodes.Status500InternalServerError,
                $"Erro ao tentar deletar rede social. Erro: {ex.Message}");
            }
        }
        #endregion

        #region DeleteByPalestrante
        /// <summary>
        /// DeleteByPalestrante
        /// </summary>
        /// <param name="redeSocialId"></param>
        /// <returns></returns>
        [HttpDelete("palestrante/{redeSocialId}")]
        public async Task<IActionResult> DeleteByPalestrante(int redeSocialId)
        {
            try
            {
                var palestrante = await _palestranteService.GetPalestranteByUserIdAsync(User.GetUserId(), false);
                if (palestrante == null) return Unauthorized();

                var redeSocial = await _redeSocialService.GetRedeSocialPalestranteByIdsAsync(palestrante.Id, redeSocialId);
                if (redeSocial == null) return NoContent();

                return await _redeSocialService.DeleteByPalestrante(palestrante.Id, redeSocial.Id)
                    ? Ok(new { message = "Deletado" })
                    : throw new Exception("Ocorreu um problema não específico ao tentar deletar rede social.");
            }
            catch (Exception ex)
            {
                return this.StatusCode(StatusCodes.Status500InternalServerError,
                $"Erro ao tentar deletar rede social. Erro: {ex.Message}");
            }
        }
        #endregion
    }
}
