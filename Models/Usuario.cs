using Microsoft.AspNetCore.Identity;
using System.ComponentModel.DataAnnotations;

public class Usuario : IdentityUser
{
    [Required]
    [StringLength(150, MinimumLength = 2, ErrorMessage = "O nome completo deve ter entre 2 e 150 caracteres.")]
    public string NomeCompleto { get; set; } = string.Empty;

    [Required(ErrorMessage = "O nickname é obrigatório.")]
    [StringLength(50, MinimumLength = 2, ErrorMessage = "O nickname deve ter entre 2 e 50 caracteres.")]
    public string Nickname { get; set; } = string.Empty;

    [StringLength(200)]
    public string FotoPerfil { get; set; }
}