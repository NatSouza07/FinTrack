using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace FinTrack.Models
{
    public class Meta
    {
        public int Id { get; set; }

        [Required(ErrorMessage = "O valor da meta é obrigatório.")]
        [Range(0.01, 9999999, ErrorMessage = "O valor da meta deve ser maior que zero.")]
        [Column(TypeName = "decimal(18,2)")]
        public decimal ValorMeta { get; set; }

        [Required(ErrorMessage = "A mês da meta é obrigatória.")]
        [Range(1, 12, ErrorMessage = "O mês da meta deve estar entre 1 e 12.")]
        public int Mes { get; set; }

        [Required(ErrorMessage = "O ano da meta é obrigatório.")]
        [Range(2000, 2100, ErrorMessage = "O ano da meta deve estar entre 2000 e 2100.")]
        public int Ano { get; set; }

        [Required]
        public string UsuarioId { get; set; } = string.Empty;
        public Usuario Usuario { get; set; } = null!;

        [NotMapped]
        public decimal ProgressoCalculado { get; set; }

        [NotMapped]
        public int Porcentagem { get; set; }

    }
}
