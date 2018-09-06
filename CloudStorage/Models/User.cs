using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace CloudStorage.Models
{
    public class User
    {
        public int Id { get; set; }
        public string Username { get; set; }
        public string Password { get; set; }
        public bool CloudServiceRegistered { get; set; }
        public byte CloudServiceType { get; set; }

        public enum ServiceType { GoogleDrive = 1, OneDrive = 2, FtpServer = 9 };

   
    }
}