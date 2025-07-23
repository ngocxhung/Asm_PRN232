using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace BussinessObjects.Models
{
    public class Publisher
    {
        [Key, DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public int PublisherId { get; set; }
        [Required]
        [StringLength(100)]
        public string PublisherName { get; set; }
        public string ContactInfo { get; set; }
        public ICollection<Book> Books { get; set; }
    }
} 