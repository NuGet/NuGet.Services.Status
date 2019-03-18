// Copyright (c) .NET Foundation. All rights reserved. 
// Licensed under the Apache License, Version 2.0. See License.txt in the project root for license information. 

using System;
using System.Collections.Generic;
using System.Security.Cryptography.X509Certificates;
using NuGet.Services.KeyVault;

namespace NuGet.Status.Configuration
{
    public class SecretReaderFactory : ISecretReaderFactory
    {
        public const string VaultNameKey = "KeyVault:VaultName";
        public const string ClientIdKey = "KeyVault:ClientId";
        public const string CertificateThumbprintKey = "KeyVault:CertificateThumbprint";
        public const string StoreNameKey = "KeyVault:StoreName";
        public const string StoreLocationKey = "KeyVault:StoreLocation";
        public const string ValidateCertificateKey = "KeyVault:ValidateCertificate";
        public const string CacheRefreshIntervalKey = "KeyVault:CacheRefreshInterval";

        private readonly IDictionary<string, string> _configurationDictionary;

        public SecretReaderFactory(IDictionary<string, string> configurationDictionary)
        {
            _configurationDictionary = configurationDictionary;
        }

        public ISecretReader CreateSecretReader()
        {
            var vaultName = _configurationDictionary[VaultNameKey];
            ISecretReader secretReader;

            // Is key vault configured?
            if (string.IsNullOrEmpty(vaultName))
            {
                secretReader = new EmptySecretReader();
            }
            else
            {
                var clientId = _configurationDictionary[ClientIdKey];
                var certificateThumbprint = _configurationDictionary[CertificateThumbprintKey];
                var storeLocation = _configurationDictionary[StoreLocationKey];
                var storeName = _configurationDictionary[StoreNameKey];
                var validateCertificate = _configurationDictionary[ValidateCertificateKey];

                int refreshIntervalSec;
                if (!_configurationDictionary.ContainsKey(CacheRefreshIntervalKey) ||
                    !int.TryParse(_configurationDictionary[CacheRefreshIntervalKey], out refreshIntervalSec))
                {
                    refreshIntervalSec = CachingSecretReader.DefaultRefreshIntervalSec;
                }

                secretReader = new KeyVaultReader(
                    new KeyVaultConfiguration(
                        vaultName,
                        clientId,
                        certificateThumbprint,
                        (StoreName)Enum.Parse(typeof(StoreName), storeName),
                        (StoreLocation)Enum.Parse(typeof(StoreLocation), storeLocation),
                        bool.Parse(validateCertificate)));

                secretReader = new CachingSecretReader(secretReader, refreshIntervalSec);
            }

            return secretReader;
        }

        public ISecretInjector CreateSecretInjector(ISecretReader secretReader)
        {
            return new SecretInjector(secretReader);
        }
    }
}