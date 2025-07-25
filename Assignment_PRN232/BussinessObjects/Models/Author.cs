using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Text.Json.Serialization;

namespace BussinessObjects.Models
{
    public class Author
    {
        [Key, DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public int AuthorId { get; set; }
        [Required]
        [StringLength(100)]
        public string AuthorName { get; set; } = string.Empty;
        public string? Description { get; set; }
        [JsonIgnore]
        public ICollection<Book>? Books { get; set; }
    }
} 