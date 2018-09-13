using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;
using System.Web;


namespace OneDriveFinal.Models
{
    public class WebAppContext : DbContext
    {
        public DbSet<PerWebUserCache> PerWebUserCaches { get; set; }


        public WebAppContext()
        {

        }
    }
}
    