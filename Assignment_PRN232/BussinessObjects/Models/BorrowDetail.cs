using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace BussinessObjects.Models
{
    public class BorrowDetail
    {
        [Key, DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public int BorrowDetailId { get; set; }
        [ForeignKey("BorrowRecord")]
        public int BorrowId { get; set; }
        [ForeignKey("Book")]
        public int BookId { get; set; }
        public int Quantity { get; set; }
        public BorrowRecord BorrowRecord { get; set; }
        public Book Book { get; set; }
    }
} 