// Copyright (c) .NET Foundation. All rights reserved. 
// Licensed under the Apache License, Version 2.0. See License.txt in the project root for license information. 

using NuGet.Services.KeyVault;

namespace NuGet.Status.Configuration
{
    public class EmptySecretReaderFactory : ISecretReaderFactory
    {
        public ISecretReader CreateSecretReader()
        {
            return new EmptySecretReader();
        }

        public ISecretInjector CreateSecretInjector(ISecretReader secretReader)
        {
            return new SecretInjector(secretReader);
        }
    }
}