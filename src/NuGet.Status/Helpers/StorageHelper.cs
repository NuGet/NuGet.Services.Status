// Copyright (c) .NET Foundation. All rights reserved. 
// Licensed under the Apache License, Version 2.0. See License.txt in the project root for license information. 

namespace NuGet.Status.Helpers
{
    public static class StorageHelper
    {
        // need lazy loading for useMSI and ClientId too? Don't need lazy loading for any of them?
        public static StorageService PrimaryStorage => new StorageService(
            "primary",
            () => MvcApplication.StatusConfiguration.PrimaryStorageBlobEndpoint,
            () => MvcApplication.StatusConfiguration.PrimaryStorageTableEndpoint);
        public static StorageService SecondaryStorage => new StorageService(
            "secondary",
            () => MvcApplication.StatusConfiguration.SecondaryStorageBlobEndpoint,
            () => MvcApplication.StatusConfiguration.SecondaryStorageTableEndpoint);
    }
}