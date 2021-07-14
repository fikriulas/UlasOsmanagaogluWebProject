using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;

namespace UlasBlog.WebUI.Models
{
    public class RoleViewModel
    {
        public int Id { get; set; }
        [Display(Name="Rol Adı")]
        [Required(ErrorMessage ="Rol Adı Gereklidir")]
        [MaxLength(20,ErrorMessage ="Rol Adı Maksimum 20 Karakterden Oluşabilir")]
        public string Name { get; set; }
    }
}
