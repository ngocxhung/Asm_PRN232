using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace BussinessObjects.Models
{
    public class Fine
    {
        [Key, DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public int FineId { get; set; }
        [ForeignKey("BorrowRecord")]
        public int BorrowId { get; set; }
        public decimal Amount { get; set; }
        public string Reason { get; set; }
        public bool Paid { get; set; }
        public DateTime? PaidDate { get; set; }
        public BorrowRecord BorrowRecord { get; set; }
    }
} 