// Copyright (c) .NET Foundation. All rights reserved. 
// Licensed under the Apache License, Version 2.0. See License.txt in the project root for license information. 

using System.Web.Mvc;
using System.Web.Routing;

namespace NuGet.Status.Controllers
{
    public class ErrorsController : AppController
    {
        protected override void Initialize(RequestContext requestContext)
        {
            base.Initialize(requestContext);

            Response.TrySkipIisCustomErrors = true;
        }

        [AcceptVerbs(HttpVerbs.Get | HttpVerbs.Head)]
        public virtual ActionResult BadRequest()
        {
            return View();
        }

        [AcceptVerbs(HttpVerbs.Get | HttpVerbs.Head)]
        public virtual ActionResult NotFound()
        {
            return View();
        }

        [AcceptVerbs(HttpVerbs.Get | HttpVerbs.Head)]
        public virtual ActionResult InternalError()
        {
            return View();
        }
    }
}