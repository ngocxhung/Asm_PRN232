using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Text.Json.Serialization;

namespace BussinessObjects.Models
{
    public class Publisher
    {
        [Key, DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public int PublisherId { get; set; }
        [Required]
        [StringLength(100)]
        public string PublisherName { get; set; } = string.Empty;
        public string? ContactInfo { get; set; }
        [JsonIgnore]
        public ICollection<Book>? Books { get; set; }
    }
} 