// Copyright (c) .NET Foundation. All rights reserved. 
// Licensed under the Apache License, Version 2.0. See License.txt in the project root for license information. 

using System.Web.Mvc;

namespace NuGet.Status.Controllers
{
    [Authorize(Roles = "Admins")]
    public class AdminController : AppController
    {
        [HttpGet]
        public ActionResult Index()
        {
            // stub to force authentication
            return null;
        }
    }
}