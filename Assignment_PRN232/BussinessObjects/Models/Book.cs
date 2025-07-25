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
        public DateTime? PublishDate { get; set; }
        [StringLength(20)]
        public string ISBN { get; set; }
        public string Description { get; set; }
        public int Quantity { get; set; }
        public int Available { get; set; }
        [StringLength(100)]
        public string Location { get; set; }
        public bool Status { get; set; } // true: Active, false: Inactive
        [StringLength(255)]
        public string? ImageUrl { get; set; }
        [JsonIgnore]
        public Author? Author { get; set; }
        [JsonIgnore]
        public Category? Category { get; set; }
        [JsonIgnore]
        public Publisher? Publisher { get; set; }
        [JsonIgnore]
        public ICollection<BorrowDetail>? BorrowDetails { get; set; }
    }
} 