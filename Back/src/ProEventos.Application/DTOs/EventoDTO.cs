using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace ProEventos.Application.DTOs
{
    public class EventoDTO
    {
        public int Id { get; set; }

        [Required]
        public string Local { get; set; }

        [Required]
        public string DataEvento { get; set; }

        [Required]
        [StringLength(50, MinimumLength = 3)]
        public string Tema { get; set; }

        [Required]
        [Range(1, 120000)]
        public int QtdPessoas { get; set; }

        [RegularExpression(@".*\.(gif|jpe?g|bmp|png)$", ErrorMessage = "The field {0} must match (gif, jpg, jpeg, bmp, png).")]
        public string ImagemURL { get; set; }

        [Required]
        [Phone]
        public string Telefone { get; set; }

        [Required]
        [EmailAddress]
        public string Email { get; set; }

        public int UserId { get; set; }
        public UserDTO User { get; set; }

        public IEnumerable<LoteDTO> Lotes { get; set; }
        public IEnumerable<RedeSocialDTO> RedesSociais { get; set; }
        public IEnumerable<PalestranteDTO> Palestrantes { get; set; }
    }
}