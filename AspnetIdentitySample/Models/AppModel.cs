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
        public virtual ICollection<ToDo> ToDoes { get; set; }
    }

    public class ToDo
    {
        public int Id { get; set; }
        public string Description { get; set; }
        public bool IsDone { get; set; }
        public virtual MyUser User { get; set; }
    }
    public class MyDbContext : IdentityDbContext<MyUser>
    {
        public MyDbContext()
            : base("DefaultConnection")
        {
        }

        protected override void OnModelCreating(DbModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);
            modelBuilder.Entity<IdentityUser>()
                .ToTable("Users");
            modelBuilder.Entity<MyUser>()
                .ToTable("Users");
        }

        public System.Data.Entity.DbSet<AspnetIdentitySample.Models.ToDo> ToDoes { get; set; }
    }


}