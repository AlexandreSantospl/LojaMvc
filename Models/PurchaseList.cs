using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

public class PurchaseList
{
    [Key]
    [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
    public int Id { get; set; }

    [Required(ErrorMessage = "O campo Nome é obrigatório")]
    [StringLength(100, ErrorMessage = "O Nome não pode exceder 100 caracteres")]
    public string Name { get; set; } = string.Empty;

    public float Price { get; set; }
}
