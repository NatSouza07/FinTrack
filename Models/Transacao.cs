using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace FinTrack.Models
{
    public class Transacao
    {
        public int Id { get; set; }

        [Required(ErrorMessage = "O tipo da transação é obrigatório.")]
        public TipoTransacao Tipo { get; set; }

        [Required(ErrorMessage = "O valor da transação é obrigatório.")]
        [Range(0.01, 9999999, ErrorMessage = "O valor deve ser maior que zero.")]
        [Column(TypeName = "decimal(18,2)")]
        public decimal Valor { get; set; }

        [Required]
        [DataType(DataType.Date)]
        public DateTime Data { get; set; } = DateTime.Now;

        [Required]
        public int ContaId { get; set; }
        public Conta Conta { get; set; } = null!;

        [Required]
        public int CategoriaId { get; set; }
        public Categoria Categoria { get; set; } = null!;

        [Required]
        public int TipoPagamentoId { get; set; }
        public TipoPagamento TipoPagamento { get; set; } = null!;

        [Required]
        public string UsuarioId { get; set; } = string.Empty;
        public Usuario Usuario { get; set; } = null!;
        public enum TipoTransacao
        {
            Entrada = 1,
            Saida = 2
        }

    }
}
