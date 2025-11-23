using System.ComponentModel.DataAnnotations;

namespace FinTrack.Models
{
    public class Categoria
    {
        public int Id { get; set; }

        [Required(ErrorMessage = "O nome da categoria é obrigatório.")]
        [StringLength(100, MinimumLength = 2)]
        public string Nome { get; set; } = string.Empty;

        [Required(ErrorMessage = "O tipo da categoria é obrigatório.")]
        [Display(Name = "Tipo de Categoria")]
        public TipoCategoria Tipo { get; set; }
    }

    public enum TipoCategoria
    {
        Receita = 1,
        Despesa = 2
    }
}
