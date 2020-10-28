// Copyright (c) .NET Foundation. All rights reserved. 
// Licensed under the Apache License, Version 2.0. See License.txt in the project root for license information. 

using System;
using System.Linq;
using System.Net.Http;
using System.Web;
using System.Web.Mvc;

namespace NuGet.Status.Helpers
{
    /// <summary>
    /// A workaround attribute that checks an accesslist of admin users for authentication. 
    /// Repairing RBAC in this application will render this check pointless.
    /// </summary>
    public class AdminAuthorizeAttribute : AuthorizeAttribute
    {
        private readonly string[] adminIdentities;

        public AdminAuthorizeAttribute()
        {
            adminIdentities = MvcApplication.StatusConfiguration?.AdminIdentities?.Split(';') ??
                throw new HttpRequestException("Authorized user configuration failure.");
        }

        protected override bool AuthorizeCore(HttpContextBase httpContext)
        {
            if (!(adminIdentities.Any(x => x.Equals(httpContext.User.Identity.Name, StringComparison.OrdinalIgnoreCase))))
            {
                return false;
            }

            return base.AuthorizeCore(httpContext);
        }
    }
}