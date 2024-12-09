using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using EntityFrameworkCore.EncryptColumn.Attribute;

namespace AccountMgmt.Models
{
    public class Transaction
    {
        [Key]
        public int TransactionId { get; set; }

        [Column(TypeName = "nvarchar(12)")]
        [DisplayName("Source Account")]
        [Required(ErrorMessage = "This field is required.")]
        [MaxLength(12, ErrorMessage = "Maximum 12 characters only.")]
        public string SourceAccountNumber { get; set; }

        [Column(TypeName = "nvarchar(100)")]
        [DisplayName("Destination Account")]
        [Required(ErrorMessage = "This field is required.")]
        public string DestinationAccountNumber { get; set; }


        [Required(ErrorMessage = "This field is required.")]
        [EncryptColumn]
        public double Amount { get; set; }

        [Column(TypeName = "nvarchar(500)")]
        [DisplayName("Narration")]
        [Required(ErrorMessage = "This field is required.")]
        [MaxLength(500, ErrorMessage = "Maximum 500 characters only.")]
        public string Narration { get; set; }



        [DisplayFormat(DataFormatString = "{0:MMM-dd-yy}")]
        public DateTime Date { get; set; }
    }
}
