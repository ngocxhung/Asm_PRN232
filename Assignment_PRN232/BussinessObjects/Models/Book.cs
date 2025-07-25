using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Text.Json.Serialization;

namespace BussinessObjects.Models
{
    public class Book
    {
        [Key, DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public int BookId { get; set; }
        [Required]
        [StringLength(200)]
        public string Title { get; set; }
        [ForeignKey("Author")]
        public int AuthorId { get; set; }
        [ForeignKey("Category")]
        public int CategoryId { get; set; }
        [ForeignKey("Publisher")]
        public int? PublisherId { get; set; }
        public int? PublishYear { get; set; }
        [StringLength(20)]
        public string ISBN { get; set; }
        public string Description { get; set; }
        public int Quantity { get; set; }
        public int Available { get; set; }
        [StringLength(100)]
        public string Location { get; set; }
        [StringLength(20)]
        public string Status { get; set; } // Available/Unavailable
        [StringLength(255)]
        public string ImageUrl { get; set; }
     
        public Author? Author { get; set; }
        public Category? Category { get; set; }
        public Publisher? Publisher { get; set; }
   
        public ICollection<BorrowDetail>? BorrowDetails { get; set; }
    }
} 