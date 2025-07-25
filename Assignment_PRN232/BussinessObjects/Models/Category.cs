using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Text.Json.Serialization;

namespace BussinessObjects.Models
{
    public class Category
    {
        [Key, DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public int CategoryId { get; set; }
        [Required]
        [StringLength(100)]
        public string CategoryName { get; set; } = string.Empty;
        public string? Description { get; set; }
        [JsonIgnore]
        public ICollection<Book>? Books { get; set; }
    }
} 