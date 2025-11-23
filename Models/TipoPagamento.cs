using System.ComponentModel.DataAnnotations;

namespace FinTrack.Models
{
    public class TipoPagamento
    {
        public int Id { get; set; }

        [Required(ErrorMessage = "O nome do tipo de pagamento é obrigatório.")]
        [StringLength(50, MinimumLength = 2)]
        public string Nome { get; set; } = string.Empty;
    }
}
