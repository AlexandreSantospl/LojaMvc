using System.ComponentModel.DataAnnotations.Schema;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel;

namespace SetorDeCompras.Models
{
    public class AuthValidationModel
    {

        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public int Id { get; set; }

        public required string Name { get; set; }

        [Required(ErrorMessage = "O campo Email é obrigatório")]
        [EmailAddress(ErrorMessage = "Formato de Email inválido")]
        public required string Email { get; set; }

        [DefaultValue(0)]
        public required int Code { get; set; }
    }
}
