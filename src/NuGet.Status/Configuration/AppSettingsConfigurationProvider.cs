// Copyright (c) .NET Foundation. All rights reserved. 
// Licensed under the Apache License, Version 2.0. See License.txt in the project root for license information. 

using System;
using System.Configuration;
using System.Threading.Tasks;
using NuGet.Services.Configuration;
using NuGet.Services.KeyVault;

namespace NuGet.Status.Configuration
{
    public class AppSettingsConfigurationProvider : ConfigurationProvider
    {
        private readonly ISecretInjector _secretInjector;

        public AppSettingsConfigurationProvider(ISecretInjector secretInjector)
        {
            _secretInjector = secretInjector ?? throw new ArgumentNullException(nameof(secretInjector));
        }

        protected override Task<string> Get(string key)
        {
            var setting = ConfigurationManager.AppSettings[key];
            return _secretInjector.InjectAsync(setting);
        }
    }
}