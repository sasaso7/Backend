using System.ComponentModel.DataAnnotations.Schema;
using System.ComponentModel.DataAnnotations;


namespace BankBackend.Database.Models
{
    public class Activity
    {
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public string? Id { get; set; }
        public required Account Account { get; set;}
        public required string Name { get; set;}
        public string? Description { get; set;}
        public DateTime Created { get; set; }
    }

    public class CreateActivity
    {
        public required string AccountID { get; set; }
        public required string Name { get; set; }
        public string? Description { get; set; }
        public DateTime Created { get; set; }
    }
}
