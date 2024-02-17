﻿using System.ComponentModel;
using System.ComponentModel.DataAnnotations;

namespace BulkyWeb.Models
{
    public class Category
    {
        [Key]
        public int id { get; set; }
        [Required]
        [MaxLength(30)]
        [DisplayName("Name")]
        public string name { get; set; }
        [Range(1, 100, ErrorMessage = "The display order must be between 1 and 100")]
        [DisplayName("Display order")]
        public int display_order { get; set; }
    }
}
