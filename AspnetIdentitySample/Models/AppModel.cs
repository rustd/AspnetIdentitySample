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
      public string HomeTown { get; set; }
    }
    public class MyDbContext : IdentityDbContextWithCustomUser<MyUser>
    {
    }
}