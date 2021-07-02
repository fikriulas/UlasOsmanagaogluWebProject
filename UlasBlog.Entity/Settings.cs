using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Text;

namespace UlasBlog.Entity
{
    public class Settings
    {
        public int Id { get; set; }
        [Required(ErrorMessage ="Site Adı Alanı Gereklidir.")]
        [Display(Name = "Site Adı")]
        [StringLength(45, ErrorMessage = "Site Adı Alanı 45 Karakterden Uzun Olamaz.")]
        public string SiteName { get; set; }
        [Required(ErrorMessage = "Site Açıklaması Alanı Gereklidir.")]
        [Display(Name = "Site Açıklaması")]        
        [StringLength(250, ErrorMessage = "Site Açıklaması Alanı 250 Karakterden Uzun Olamaz.")]
        public string SiteDescription { get; set; }
        [Required(ErrorMessage = "Smtp Address Alanı Gereklidir.")]
        [Display(Name = "Smtp Address")]
        [StringLength(45, ErrorMessage = "Smtp Address 45 Karakterden Uzun Olamaz.")]
        public string SmtpAddress { get; set; }
        [Display(Name = "Tarih")]
        public DateTime UpdateDate { get; set; }
        [Display(Name ="Port")]
        [Required(ErrorMessage = "Port Alanı Gereklidir.")]        
        [StringLength(4, ErrorMessage = "Port Alanı 4 Karakterden Uzun Olamaz.")]
        public string Port { get; set; }
        [Display(Name = "Mail Kullanıcı Adı")]
        [EmailAddress]
        [Required(ErrorMessage = "Mail Kullanıcı Adı Alanı Gereklidir.")]
        [StringLength(45, ErrorMessage = "Mail Kullanıcı Adı Alanı 45 Karakterden Uzun Olamaz.")]
        public string MailUserName { get; set; }
        [Display(Name = "Mail Şifresi")]
        [Required(ErrorMessage = "Mail Şifresi Gereklidir.")]
        [DataType(DataType.Password)]
        [StringLength(35, ErrorMessage = "Mail Şifresi Alanı 35 Karakterden Uzun Olamaz.")]
        public string MailPassword { get; set; }
    }
}
