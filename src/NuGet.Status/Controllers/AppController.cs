// Copyright (c) .NET Foundation. All rights reserved. 
// Licensed under the Apache License, Version 2.0. See License.txt in the project root for license information. 

using NuGet.Status.Helpers;
using System.Web.Mvc;

namespace NuGet.Status.Controllers
{
    public class AppController : Controller
    {
        protected CachedServiceStatus CachedServiceStatus => ServiceStatusHelper.CachedServiceStatus;

        protected override void HandleUnknownAction(string actionName)
        {
            HttpNotFound().ExecuteResult(ControllerContext);
        }
    }
}