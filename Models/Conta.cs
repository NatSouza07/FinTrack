using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace FinTrack.Models
{
    public class Conta
    {
        public int Id { get; set; }

        [Required(ErrorMessage = "O nome da conta é obrigatório.")]
        [StringLength(100, MinimumLength = 2)]
        public string Nome { get; set; } = string.Empty;

        [Required(ErrorMessage = "O saldo inicial é obrigatório.")]
        [Range(0, 9999999)]
        [Column(TypeName = "decimal(18,2)")]
        [Display(Name = "Saldo Inicial")]
        public decimal SaldoInicial { get; set; }

        [Required(ErrorMessage = "O usuário é obrigatório.")]
        public string UsuarioId { get; set; } = string.Empty;

        public Usuario Usuario { get; set; }

        public ICollection<Transacao> Transacoes { get; set; } = new List<Transacao>();

    }
}
