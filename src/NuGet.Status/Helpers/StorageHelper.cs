// Copyright (c) .NET Foundation. All rights reserved. 
// Licensed under the Apache License, Version 2.0. See License.txt in the project root for license information. 

namespace NuGet.Status.Helpers
{
    public static class StorageHelper
    {
        public static StorageService PrimaryStorage => new StorageService(
            "primary",
            () => MvcApplication.StatusConfiguration.ConnectionString);
        public static StorageService SecondaryStorage => new StorageService(
            "secondary",
            () => MvcApplication.StatusConfiguration.SecondaryConnectionString);
    }
}