// Copyright (c) .NET Foundation. All rights reserved. 
// Licensed under the Apache License, Version 2.0. See License.txt in the project root for license information. 

using Newtonsoft.Json;
using NuGet.Services.Status;

namespace NuGet.Status.Helpers
{
    public class CachedServiceStatus
    {
        public string Json { get; set; }
        public ServiceStatus ServiceStatus { get; set; }

        public CachedServiceStatus(string json)
        {
            Json = json;
            ServiceStatus = JsonConvert.DeserializeObject<ServiceStatus>(json);
        }
    }
}