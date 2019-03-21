// Copyright (c) .NET Foundation. All rights reserved. 
// Licensed under the Apache License, Version 2.0. See License.txt in the project root for license information. 

using System.ComponentModel.DataAnnotations;
using NuGet.Services.Configuration;

namespace NuGet.Status.Configuration
{
    public class IdaConfiguration : Services.Configuration.Configuration
    {
        public const string IdaPrefix = "Ida:";

        [ConfigurationKeyPrefix(IdaPrefix)]
        [Required]
        public string ClientId { get; set; }

        [ConfigurationKeyPrefix(IdaPrefix)]
        [Required]
        public string AADInstance { get; set; }

        [ConfigurationKeyPrefix(IdaPrefix)]
        [Required]
        public string Tenant { get; set; }

        [ConfigurationKeyPrefix(IdaPrefix)]
        [Required]
        public string RootUri { get; set; }
    }
}