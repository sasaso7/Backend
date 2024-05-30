using System.ComponentModel.DataAnnotations.Schema;
using System.ComponentModel.DataAnnotations;


namespace BankBackend.Database.Models
{
    public class Activity
    {
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public string? Id { get; set; }
        [Required]
        public string AccountId { get; set; } // Foreign key property
        [Required]
        public Account Account { get; set; } // Navigation property
        [Required]
        public string Name { get; set; }
        public string? Description { get; set; }
        public DateTime Created { get; set; }
    }

    public class CreateActivity
    {
        public required string AccountID { get; set; }
        public required string Name { get; set; }
        public string? Description { get; set; }
    }
}
