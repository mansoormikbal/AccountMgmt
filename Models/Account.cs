using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Diagnostics.CodeAnalysis;


namespace AccountMgmt.Models
{
    public class Account
    {
        [Key]
        public int AccountId { get; set; }

        [Column(TypeName = "nvarchar(200)")]
        [DisplayName("Account HolderName")]
        [Required(ErrorMessage = "This field is required.")]
        [MaxLength(200, ErrorMessage = "Maximum 200 characters only.")]
        public string AccountHolderName { get; set; }

        [Column(TypeName = "decimal(18,4)")]
        [DisplayName("Balance")]
        [Required(ErrorMessage = "This field is required.")]
        public double Balance { get; set; }
        [Column(TypeName = "nvarchar(12)")]
        [DisplayName("Account Number")]
        [Required(ErrorMessage = "This field is required.")]
        [MaxLength(12, ErrorMessage = "Maximum 12 characters only.")]
        public string AccountNumber { get; set; }
        public DateTime CreatedAt { get; set; }

        //[AllowNull]
        //public ICollection<Transaction> Transactions { get; set; }
    }
}
