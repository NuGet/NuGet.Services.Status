// Copyright (c) .NET Foundation. All rights reserved. 
// Licensed under the Apache License, Version 2.0. See License.txt in the project root for license information. 

using System.Web.Mvc;
using System.Web.Routing;

namespace NuGet.Status
{
    public class RouteConfig
    {
        public static void RegisterRoutes(RouteCollection routes)
        {
            routes.MapRoute(
                name: "Json",
                url: "json",
                defaults: new { controller = "Home", action = "Json" });

            routes.MapRoute(
                name: "Health",
                url: "health",
                defaults: new { controller = "Home", action = "Health" });

            routes.MapRoute(
                name: "Default",
                namespaces: new string[] { "NuGet.Status.Controllers" },
                url: "{controller}/{action}",
                defaults: new { controller = "Home", action = "Index" },
                constraints: new { controller = "Home|Admin|Errors" });
        }
    }
}
