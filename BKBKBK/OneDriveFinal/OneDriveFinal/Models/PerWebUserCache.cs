using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.ComponentModel.DataAnnotations;

namespace OneDriveFinal.Models
{
    public class PerWebUserCache
    {
        [Key]
        public int Id { get; set; }
        //public int UserId { get; set; }
        public string WebUserUniqueId { get; set; }
        public byte[] CacheBits { get; set; }
        //public string UserState { get; set; }
        public DateTime? LastWrite { get; set; }
    }
}