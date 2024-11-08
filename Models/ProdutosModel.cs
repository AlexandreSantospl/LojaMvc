using System.ComponentModel.DataAnnotations.Schema;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel;

namespace SetorDeCompras.Models
{
    public class ProdutosModel
    {

        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public int Id { get; set; }

        [Required(ErrorMessage = "O campo Nome é obrigatório")]
        [StringLength(100, ErrorMessage = "O Nome não pode exceder 100 caracteres")]
        public required string Name { get; set; }

        [DefaultValue(0)]
        public required float Preco { get; set; }

        [DefaultValue(0)]
        public required int Quantidade { get; set; } = 0;

        [DefaultValue("")]
        public string? Imagem { get; set; } = "";

    }
}

