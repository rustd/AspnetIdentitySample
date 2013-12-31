using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using Microsoft.AspNet.Identity;
using AspnetIdentitySample.Models;
using System.Security.Claims;
using System.Threading.Tasks;

namespace AspnetIdentitySample.IdentityExtensions
{
    public class MyClaimsIdentityFactory<TUser> : ClaimsIdentityFactory<TUser> where TUser : IUser<string>
    {
        internal const string IdentityProviderClaimType = "http://schemas.microsoft.com/accesscontrolservice/2010/07/claims/identityprovider";
        internal const string DefaultIdentityProviderClaimValue = "ASP.NET Identity";

        public MyClaimsIdentityFactory()
        {
            UserIdClaimType = ClaimTypes.NameIdentifier;
            UserNameClaimType = ClaimsIdentity.DefaultNameClaimType;
            // This is the custom claim added to the User. This can keep track of the 
            // last login time of the User
            LastLoginTimeType = "LoginTime";
        }

        public async override Task<ClaimsIdentity> CreateAsync(UserManager<TUser,string> manager, TUser user, string authenticationType)
        {
 	        // This is a standard place where you can register your ClaimsIdentityFactory.
            // This class generates a ClaimsIdentity for the given user
            // By default it adds Roles, UserName, UserId as Claims for the User
            // You can add more standard set of claims here if you want to.
            // The Following sample shows how you can add more claims in a centralized way.
            // This sample does not add Roles as Claims

            var id = new ClaimsIdentity(authenticationType, UserNameClaimType, null);
            id.AddClaim(new Claim(UserIdClaimType, user.Id.ToString(), ClaimValueTypes.String));
            id.AddClaim(new Claim(UserNameClaimType, user.UserName, ClaimValueTypes.String));
            id.AddClaim(new Claim(IdentityProviderClaimType, DefaultIdentityProviderClaimValue, ClaimValueTypes.String));
            
            // Add the claims for the login time
            id.AddClaim(new Claim(LastLoginTimeType, DateTime.Now.ToString()));

            if (manager.SupportsUserClaim)
            {
                id.AddClaims(await manager.GetClaimsAsync(user.Id));
            }
            return id;
        }

        public string UserIdClaimType { get; set; }
        public string UserNameClaimType { get; set; }
        public string LastLoginTimeType { get; set; }
    }
}