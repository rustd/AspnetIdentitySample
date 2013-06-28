using Microsoft.AspNet.Identity.EntityFramework;
using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;
using System.Web;

namespace AspnetIdentitySample.Models
{
    public class MyUser : User
    {
        //public DateTime BirthDate { get; set; }
    }

    public class CustomUserContext : IdentityStoreContext
    {
        public CustomUserContext(DbContext db)
            : base(db)
        {
            Users = new UserStore<MyUser>(db);
        }
    }

    public class MyDbContext : IdentityDbContext<MyUser, UserClaim, UserSecret, UserLogin, Role, UserRole>
    {
    }
}