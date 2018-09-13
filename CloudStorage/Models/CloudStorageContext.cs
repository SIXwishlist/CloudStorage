using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Data.Entity;

namespace CloudStorage.Models
{
    public class CloudStorageContext : DbContext
    {
        public DbSet<User> Users { get; set; }
        public DbSet<FtpServer> FtpServers { get; set; }
        
        public DbSet<FileProcessingResult> FilesProcessingResults { get; set; }
        

        public CloudStorageContext()
        {
            
        }
    }
}