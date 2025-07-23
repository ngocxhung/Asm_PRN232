using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace BussinessObjects.Models
{
    public class BorrowRecord
    {
        [Key, DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public int BorrowId { get; set; }
        [ForeignKey("User")]
        public int UserId { get; set; }
        public DateTime BorrowDate { get; set; }
        public DateTime DueDate { get; set; }
        public DateTime? ReturnDate { get; set; }
        [StringLength(20)]
        public string Status { get; set; } // Borrowed/Returned/Overdue
        public string Note { get; set; }
        public User User { get; set; }
        public ICollection<BorrowDetail> BorrowDetails { get; set; }
        public Fine Fine { get; set; }
    }
} 