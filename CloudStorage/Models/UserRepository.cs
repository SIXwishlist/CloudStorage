using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Web;

namespace CloudStorage.Models
{
    public class UserRepository
    {
        // Context declaration
        private static CloudStorageContext _context = new CloudStorageContext();

        public static int Register(User user)
        {
            // User id
            int userId = 0;

            try
            {
                _context.Users.Add(user);
                _context.SaveChanges();
                userId = user.Id;
            } 
            catch (Exception ex)
            {
                Debug.WriteLine(ex.Message);
            }
            

            return userId;
        }

        public static User Login(User user)
        {
            // Get user from DB according to login info
            User userInDB = _context.Users.First(u => u.Username.Equals(user.Username));
            
            if (userInDB != null && userInDB.Password.Equals(user.Password))
            {
                return userInDB;
            }

            return null;
        }

        /**
         *Update cloud service status in User Table 
         * 
         */
        public static bool UpdateCloudServiceStatus(int userId, bool useService, byte serviceType)
        {
            // Get user from DB 
            try {
                User userInDB = _context.Users.First(u => u.Id == userId);

                userInDB.CloudServiceRegistered = useService;
                userInDB.CloudServiceType = serviceType;

                _context.SaveChanges();
                return true;
            }
            catch (Exception ex)
            {
                Debug.WriteLine(ex.Message);
                return false;
            }

            
        }

        public static bool UpdateFtpServer(FtpServer ftpServer)
        {
            // Get ftp server in db
            FtpServer ftpInDB = _context.FtpServers.FirstOrDefault(f => f.UserId == ftpServer.UserId);

            try
            {
                // Add a new record into DB when does not exist
                if (ftpInDB == null)
                {
                    _context.FtpServers.Add(ftpServer);
                    _context.SaveChanges();
                }
                // Update existing record
                else
                {
                    ftpInDB.Host = ftpServer.Host;
                    ftpInDB.Username = ftpServer.Username;
                    ftpInDB.Password = ftpServer.Password;
                    _context.SaveChanges();
                }
                return true;
            } 
            catch (Exception ex)
            {
                Debug.WriteLine(ex.Message);
                return false;
            }
            
        }
    }
}