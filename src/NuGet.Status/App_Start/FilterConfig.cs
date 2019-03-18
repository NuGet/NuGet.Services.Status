// Copyright (c) .NET Foundation. All rights reserved. 
// Licensed under the Apache License, Version 2.0. See License.txt in the project root for license information. 

using System.Web.Mvc;

namespace NuGet.Status
{
    public class FilterConfig
    {
        public static void RegisterGlobalFilters(GlobalFilterCollection filters)
        {
#if !DEBUG
            filters.Add(new RequireHttpsAttribute());
#endif
        }
    }
}
