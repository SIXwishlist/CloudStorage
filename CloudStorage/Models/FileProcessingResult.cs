using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.ComponentModel.DataAnnotations;

namespace CloudStorage.Models
{
    public class FileProcessingResult
    {
        public int Id { get; set; }
        public int UserId { get; set; }
        [StringLength(200)]
        public string FileName { get; set; }
        public byte CloudStorageType { get; set; }
        public DateTime? TimeStamp { get; set; }
        public bool IsSuccessful { get; set; }
    }
}