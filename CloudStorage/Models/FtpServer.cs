using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;

namespace CloudStorage.Models
{
    public class FtpServer
    {
        public int Id { get; set; }
        public int UserId { get; set; }
        [Required]
        [Display(Name = "FTP Server Address")]
        public string Host { get; set; }
        [Required]
        public string Username { get; set; }
        [Required]
        public string Password { get; set; }

        
    }
}