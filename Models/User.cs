using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Models
{
    public class User
    {
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public int Id { get; set; } 

        [Required(ErrorMessage = "O campo Nome é obrigatório")]
        [StringLength(100, ErrorMessage = "O Nome não pode exceder 100 caracteres")]
        public required string Name { get; set; }

        [Required(ErrorMessage = "O campo Email é obrigatório")]
        [EmailAddress(ErrorMessage = "Formato de Email inválido")]
        public required string Email { get; set; }

        [Required(ErrorMessage = "O campo Idade é obrigatório")]
        [Range(18, 100, ErrorMessage = "A idade deve estar entre 18 e 100 anos")]
        public int Age { get; set; }

        public PurchaseList? Purchases { get; set; }
        public List<PurchaseList> PurchasesList { get; set; } = new List<PurchaseList>();
    }
}
