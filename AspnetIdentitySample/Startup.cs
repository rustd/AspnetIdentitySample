﻿using Microsoft.Owin;
using Owin;

[assembly: OwinStartupAttribute(typeof(AspnetIdentitySample.Startup))]
namespace AspnetIdentitySample
{
    public partial class Startup
    {
        public void Configuration(IAppBuilder app)
        {
            ConfigureAuth(app);
        }
    }
}
