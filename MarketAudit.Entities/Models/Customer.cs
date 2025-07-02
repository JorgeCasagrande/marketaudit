using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Text;

namespace MarketAudit.Entities.Models
{
    public class Customer : Entity
    {
        [Required]
        [StringLength(50,ErrorMessage ="El nombre debe tener hasta 50 caracteres")]
        public string Name { get; set; }
        [Required]
        [StringLength(255, ErrorMessage = "La descripción debe tener hasta 255 caracteres")]
        public string Description { get; set; }
        public string Image { get; set; }
        public bool Enable { get; set; }
    }
}
