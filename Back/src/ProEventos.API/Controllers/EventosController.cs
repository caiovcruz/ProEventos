using System;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using ProEventos.Application.Contratos;
using Microsoft.AspNetCore.Http;
using ProEventos.Application.DTOs;
using System.IO;
using Microsoft.AspNetCore.Hosting;

namespace ProEventos.API.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class EventosController : ControllerBase
    {
        private readonly IEventoService _eventoService;
        private readonly IWebHostEnvironment _hostEnvironment;
        public EventosController(IEventoService eventoService, IWebHostEnvironment hostEnvironment)
        {
            _hostEnvironment = hostEnvironment;
            _eventoService = eventoService;
        }

        #region Get
        /// <summary>
        /// Get
        /// </summary>
        /// <param name="includePalestrantes"></param>
        /// <returns></returns>
        [HttpGet]
        public async Task<IActionResult> Get(bool includePalestrantes = false)
        {
            try
            {
                var eventos = await _eventoService.GetAllEventosAsync(includePalestrantes);
                if (eventos == null) return NoContent();

                return Ok(eventos);
            }
            catch (Exception ex)
            {
                return this.StatusCode(StatusCodes.Status500InternalServerError,
                $"Erro ao tentar recuperar eventos. Erro: {ex.Message}");
            }
        }
        #endregion

        #region GetById
        /// <summary>
        /// GetById
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        [HttpGet("{id}")]
        public async Task<IActionResult> GetById(int id, bool includePalestrantes = false)
        {
            try
            {
                var evento = await _eventoService.GetEventoByIdAsync(id, includePalestrantes);
                if (evento == null) return NoContent();

                return Ok(evento);
            }
            catch (Exception ex)
            {
                return this.StatusCode(StatusCodes.Status500InternalServerError,
                $"Erro ao tentar recuperar evento. Erro: {ex.Message}");
            }
        }
        #endregion

        #region GetByTema
        /// <summary>
        /// GetByTema
        /// </summary>
        /// <param name="tema"></param>
        /// <returns></returns>
        [HttpGet("tema/{tema}")]
        public async Task<IActionResult> GetByTema(string tema, bool includePalestrantes = false)
        {
            try
            {
                var eventos = await _eventoService.GetAllEventosByTemaAsync(tema, includePalestrantes);
                if (eventos == null) return NoContent();

                return Ok(eventos);
            }
            catch (Exception ex)
            {
                return this.StatusCode(StatusCodes.Status500InternalServerError,
                $"Erro ao tentar recuperar eventos. Erro: {ex.Message}");
            }
        }
        #endregion

        #region Post
        /// <summary>
        /// Post
        /// </summary>
        /// <returns></returns>
        [HttpPost]
        public async Task<IActionResult> Post(EventoDTO model)
        {
            try
            {
                var evento = await _eventoService.AddEvento(model);
                if (evento == null) return NoContent();

                return Ok(evento);
            }
            catch (Exception ex)
            {
                return this.StatusCode(StatusCodes.Status500InternalServerError,
                $"Erro ao tentar adicionar evento. Erro: {ex.Message}");
            }
        }
        #endregion

        #region UploadImage
        /// <summary>
        /// Post
        /// </summary>
        /// <returns></returns>
        [HttpPost("upload-image/{eventoId}")]
        public async Task<IActionResult> UploadImage(int eventoId)
        {
            try
            {
                var evento = await _eventoService.GetEventoByIdAsync(eventoId, true);
                if (evento == null) return NoContent();

                var file = Request.Form.Files[0];
                if (file.Length > 0)
                {
                    DeleteImage(evento.ImagemURL);
                    evento.ImagemURL = await SaveImage(eventoId, file);
                }

                var eventoRetorno = await _eventoService.UpdateEvento(eventoId, evento);

                return Ok(eventoRetorno);
            }
            catch (Exception ex)
            {
                return this.StatusCode(StatusCodes.Status500InternalServerError,
                $"Erro ao fazer upload da imagem. Erro: {ex.Message}");
            }
        }

        [NonAction]
        public void DeleteImage(string imageName)
        {
            var imagePath = Path.Combine(_hostEnvironment.ContentRootPath, @"Resources/Images/Eventos", imageName);
            if (System.IO.File.Exists(imagePath))
                System.IO.File.Delete(imagePath);
        }

        [NonAction]
        public async Task<string> SaveImage(int eventoId, IFormFile imageFile)
        {
            string pathImagesEvento = Path.Combine(_hostEnvironment.ContentRootPath, @"Resources/Images/Eventos");

            if (!Directory.Exists(pathImagesEvento)) Directory.CreateDirectory(pathImagesEvento);

            string imageName = $"{eventoId}{Path.GetExtension(imageFile.FileName)}";

            var imagePath = Path.Combine(pathImagesEvento, imageName);

            using (var fileStream = new FileStream(imagePath, FileMode.Create))
            {
                await imageFile.CopyToAsync(fileStream);
            }

            return imageName;
        }
        #endregion

        #region Put
        /// <summary>
        /// Put
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        [HttpPut("{id}")]
        public async Task<IActionResult> Put(int id, EventoDTO model)
        {
            try
            {
                var evento = await _eventoService.UpdateEvento(id, model);
                if (evento == null) return NoContent();

                return Ok(evento);
            }
            catch (Exception ex)
            {
                return this.StatusCode(StatusCodes.Status500InternalServerError,
                $"Erro ao tentar atualizar evento. Erro: {ex.Message}");
            }
        }

        #endregion

        #region Delete
        /// <summary>
        /// Delete
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        [HttpDelete("{id}")]
        public async Task<IActionResult> Delete(int id)
        {
            try
            {
                var evento = await _eventoService.GetEventoByIdAsync(id, true);
                if (evento == null) return NoContent();

                if (await _eventoService.DeleteEvento(id))
                {
                    DeleteImage(evento.ImagemURL);
                    return Ok(new { message = "Deletado" });
                }
                else
                {
                    throw new Exception("Ocorreu um problema não específico ao tentar deletar Evento.");
                }
            }
            catch (Exception ex)
            {
                return this.StatusCode(StatusCodes.Status500InternalServerError,
                $"Erro ao tentar deletar evento. Erro: {ex.Message}");
            }
        }
        #endregion
    }
}
