using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Text;

namespace UlasBlog.Entity
{
    public class Iplist
    {
        public int Id { get; set; }
        [Required]
        [StringLength(17, ErrorMessage = "İp Adresi 17 Karakterden Uzun Olamaz.")]
        public string Ip { get; set; }        
        public DateTime Date { get; set; }
        public bool Block { get; set; }
        public string Note { get; set; }
    }
}
