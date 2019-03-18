// Copyright (c) .NET Foundation. All rights reserved. 
// Licensed under the Apache License, Version 2.0. See License.txt in the project root for license information. 

namespace NuGet.Status.Helpers
{
    public static class StorageHelper
    {
        private const string PrimaryConnectionStringKey = "Storage:ConnectionString";
        private const string SecondaryConnectionStringKey = "Storage:SecondaryConnectionString";

        public static StorageService PrimaryStorage => new StorageService(PrimaryConnectionStringKey);
        public static StorageService SecondaryStorage => new StorageService(SecondaryConnectionStringKey);
    }
}