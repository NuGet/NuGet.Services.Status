// Copyright (c) .NET Foundation. All rights reserved. 
// Licensed under the Apache License, Version 2.0. See License.txt in the project root for license information. 

using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using NuGet.Services.Configuration;

namespace NuGet.Status.Configuration
{
    public class StatusConfiguration : Services.Configuration.Configuration
    {
        public const string StoragePrefix = "Storage:";

        public string ApplicationInsightsKey { get; set; }

        [DefaultValue("https://www.nuget.org/")]
        public string NuGetBaseUrl { get; set; }

        public string ExternalBrandingMessage { get; set; }
        public string ExternalBrandingUrl { get; set; }
        public string ExternalAboutUrl { get; set; }
        public string ExternalPrivacyPolicyUrl { get; set; }
        public string ExternalTermsOfUseUrl { get; set; }
        public string TrademarksUrl { get; set; }

        [ConfigurationKeyPrefix(IdaConfiguration.IdaPrefix)]
        public bool AdminEnabled { get; set; }

        [ConfigurationKeyPrefix(StoragePrefix)]
        [Required]
        public string ConnectionString { get; set; }

        [ConfigurationKeyPrefix(StoragePrefix)]
        [Required]
        public string SecondaryConnectionString { get; set; }

        [ConfigurationKeyPrefix(StoragePrefix)]
        [Required]
        public string BlobName { get; set; }

        [ConfigurationKeyPrefix(StoragePrefix)]
        [Required]
        public string ContainerName { get; set; }

        [ConfigurationKeyPrefix(StoragePrefix)]
        [Required]
        public string TableName { get; set; }
    }
}