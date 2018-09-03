using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;

namespace CloudStorage.Models
{
    public class FtpServer
    {
        [Required]
        [Display(Name = "FTP Server Address")]
        public string Address { get; set; }
        [Required]
        public string Username { get; set; }
        [Required]
        public string Password { get; set; }
    }
}