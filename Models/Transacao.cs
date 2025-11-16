using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace FinTrack.Models
{
    public class Transacao
    {
        public int Id { get; set; }

        [Required(ErrorMessage = "O tipo da transação é obrigatório.")]
        public TipoPagamento Tipo { get; set; }

        [Required(ErrorMessage = "O valor da transação é obrigatório.")]
        [Column(TypeName = "decimal(18,2)")]
        public decimal Valor { get; set; }

        [Required(ErrorMessage = "A data da transação é obrigatória.")]
        public DateTime Data { get; set; } = DateTime.Now;

        [Required]
        public int ContaId { get; set; }
        public Conta Conta { get; set; }

        [Required]
        public int CategoriaId { get; set; }
        public Categoria Categoria { get; set; }

        [Required]
        public int TipoPagamentoId { get; set; }
        public TipoPagamento TipoPagamento { get; set; }

        [Required]
        public int UsuarioId { get; set; } = string.Empty;
        public Usuario Usuario { get; set; }

        public enum TipoTransacao
        {
            Entrada = 1,
            Saída = 2
        }

    }
}
