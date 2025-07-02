using MarketAudit.Entities.Models.Generic;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.Design.Serialization;
using System.Text;

namespace MarketAudit.Entities.Models
{
    public class User : Entity
    {
        [Required(ErrorMessage = "Ingrese un usuario")]
        [StringLength(50,ErrorMessage ="El nombre de usuario debe tener hasta 50 caracteres")]
        public string UserName { get; set; }
        public string Password { get; set; }
        [Required(ErrorMessage = "Ingrese un nombre")]
        [StringLength(50, ErrorMessage = "El nombre debe tener hasta 50 caracteres")]
        public string Name { get; set; }
        [Required(ErrorMessage = "Ingrese un apellido")]
        [StringLength(50, ErrorMessage = "El apellido debe tener hasta 50 caracteres")]
        public string LastName { get; set; }
        [Required(ErrorMessage = "Ingrese un correo")]
        [StringLength(100, ErrorMessage = "El correo debe tener hasta 100 caracteres")]
        public string Email { get; set; }
        [Required(ErrorMessage = "Debe seleccionar un rol")]
        public long RoleId { get; set; }
        public bool Enabled { get; set; }
        public string Image { get; set; }
        public DateTime? Creation { get; set; }
        public DateTime? EndDate { get; set; }
        public IList<KeyValueDto> RoleList { get; set; }
        public bool UserTest { get; set; }
    }
}
