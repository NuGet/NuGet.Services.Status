// Copyright (c) .NET Foundation. All rights reserved. 
// Licensed under the Apache License, Version 2.0. See License.txt in the project root for license information. 

using System;
using System.Net;
using System.Web.Mvc;
using NuGet.Services.Status;

namespace NuGet.Status.Controllers
{
    public class HomeController : AppController
    {
        [HttpGet]
        public ActionResult Index()
        {
            return View(CachedServiceStatus?.ServiceStatus);
        }

        [HttpGet]
        public ActionResult Json()
        {
            var json = CachedServiceStatus?.Json;
            if (string.IsNullOrEmpty(json))
            {
                return new HttpStatusCodeResult(
                    HttpStatusCode.ServiceUnavailable,
                    "The service status JSON could not be loaded.");
            }

            return Content(json, "application/json");
        }

        /// <summary>
        /// If the service status is more outdated than this threshold, the <see cref="Health"/> endpoint should return unhealthy.
        /// </summary>
        private static readonly TimeSpan OutdatedThreshold = TimeSpan.FromMinutes(15);

        /// <remarks>
        /// The status page is unhealthy if:
        /// 1) It failed to load the service status JSON, which powers the page.
        /// 2) The service status JSON is outdated.
        /// </remarks>
        [HttpGet]
        public ActionResult Health()
        {
            var status = CachedServiceStatus?.ServiceStatus;
            if (status == null)
            {
                return new HttpStatusCodeResult(
                    HttpStatusCode.ServiceUnavailable, 
                    "The service status JSON could not be loaded.");
            }

            var currentTime = DateTime.UtcNow;

            /// We only check if <see cref="ServiceStatus.LastUpdated"/> is recent because <see cref="ServiceStatus.LastBuilt"/> is implied by <see cref="ServiceStatus.LastUpdated"/>.
            if (currentTime - status.LastUpdated > OutdatedThreshold)
            {
                return new HttpStatusCodeResult(
                    HttpStatusCode.ServiceUnavailable,
                    "The service status JSON was loaded, but is too outdated. " +
                    GetStatusHealthDiagnosticsString(status, currentTime));
            }
            
            return new HttpStatusCodeResult(HttpStatusCode.OK, 
                "The service status JSON is recent. " +
                GetStatusHealthDiagnosticsString(status, currentTime));
        }

        private static string GetStatusHealthDiagnosticsString(ServiceStatus status, DateTime currentTime)
        {
            return $"It was last built at {status.LastBuilt.ToString("o")}. " +
                $"It was last updated at {status.LastUpdated.ToString("o")}. " +
                $"The current time is {currentTime.ToString("o")}. ";
        }
    }
}