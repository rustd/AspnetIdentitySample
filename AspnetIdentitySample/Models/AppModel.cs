using Microsoft.AspNet.Identity;
using Microsoft.AspNet.Identity.EntityFramework;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Data.Entity;
using System.Linq;
using System.Web;

namespace AspnetIdentitySample.Models
{
    public class MyUser : IdentityUser
    {
        public string HomeTown { get; set; }
    }
    public class MyDbContext : IdentityDbContext<MyUser>
    {
        public MyDbContext()
            : base("DefaultConnection")
        {
        }
    }


}