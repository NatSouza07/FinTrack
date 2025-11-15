using Microsoft.AspNetCore.Identity;
using System.ComponentModel.DataAnnotations;

namespace FinTrack.Models
{
    public class Usuario : IdentityUser
    {
        [Required]
        [StringLength(150)]

        public string NomeCompleto { get; set; } = string.Empty;

        public string? FotoPerfil { get; set; }
    }
}
