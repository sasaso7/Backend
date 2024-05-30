
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace BankBackend.Database.Models
{
    public class Account
    {
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public string? Id { get; set; }
        public string? Picture { get; set; }
        public string? Name { get; set; }
        public string UserId { get; set; }  // Foreign key property
    }

    public class CreateAccountRequest
    {
        public required string Name { get; set; }
        public required string UserId { get; set; }
    }
}
