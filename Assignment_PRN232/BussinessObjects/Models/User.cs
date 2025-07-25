using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace BussinessObjects.Models
{
    public class User
    {
        [Key, DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public int UserId { get; set; }
        [Required]
        [StringLength(100)]
        public string Username { get; set; }
        [Required]
        public string PasswordHash { get; set; }
        [StringLength(100)]
        public string FullName { get; set; }
        [StringLength(100)]
        public string Email { get; set; }
        [StringLength(20)]
        public string Phone { get; set; }
        [StringLength(20)]
        public string Role { get; set; } // Admin, Librarian, Reader
        public bool Status { get; set; } // Active/Inactive
        public DateTime CreatedAt { get; set; }
        [StringLength(255)]
        public string ImageUrl { get; set; }
        [Microsoft.AspNetCore.Mvc.ModelBinding.BindNever]
        [System.Text.Json.Serialization.JsonIgnore]
        [Newtonsoft.Json.JsonIgnore]
        public ICollection<BorrowRecord>? BorrowRecords { get; set; }
    }
} 