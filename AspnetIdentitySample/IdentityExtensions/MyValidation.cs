using AspnetIdentitySample.Models;
using Microsoft.AspNet.Identity;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.Web;

namespace AspnetIdentitySample.IdentityExtensions
{
    public class MyUserValidation : IIdentityValidator<ApplicationUser>
    {
        public System.Threading.Tasks.Task<IdentityResult> ValidateAsync(ApplicationUser item)
        {
            if (item.UserName.ToLower().Contains("bad"))
                return Task.FromResult(IdentityResult.Failed("UserName cannot contain bad"));
            else if (item.HomeTown.ToLower().Contains("unknown"))
                return Task.FromResult(IdentityResult.Failed("HomeTown cannot contain unknown city"));
            else
                return Task.FromResult(IdentityResult.Success);
        }
    }

    public class MyPasswordValidation : IIdentityValidator<string>
    {
        public System.Threading.Tasks.Task<IdentityResult> ValidateAsync(string item)
        {
            if (item.ToLower().Contains("111111"))
                return Task.FromResult(IdentityResult.Failed("Password Cannot contain 6 consecutive digits"));
            else
                return Task.FromResult(IdentityResult.Success);
        }
    }

    // This allows you to Hash a given password using your own Hashing system
    // Note be very careful when plugging in your own Hasher as there are no gurantees that it will be safe.
    // The reason this hook is here so you can migrate easily from earlier membership systems which
    // might be using a different Hashing mechanism
    public class MyPasswordHasher : IPasswordHasher
    {
        // If input password is foo then in the database it will be footestpranav
        public string HashPassword(string password)
        {
            return password + "testpranav";
        }

        public PasswordVerificationResult VerifyHashedPassword(string hashedPassword, string providedPassword)
        {
            if (hashedPassword == providedPassword + "testpranav")
                return PasswordVerificationResult.Success;
            else
                return PasswordVerificationResult.Failed;
        }
    }


}