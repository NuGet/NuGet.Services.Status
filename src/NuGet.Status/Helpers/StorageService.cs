// Copyright (c) .NET Foundation. All rights reserved. 
// Licensed under the Apache License, Version 2.0. See License.txt in the project root for license information. 

using System;
using Azure.Data.Tables;
using Azure.Identity;
using Azure.Storage.Blobs;

namespace NuGet.Status.Helpers
{
    public class StorageService
    {
        private readonly string _blobConnectionString;
        private readonly string _tableConnectionString;
        private readonly bool _useManagedIdentity;
        private readonly string _managedIdentityClientId;

        public string Name { get; }

        // Do we need to use lazy loading for connection strings? We're not using KV secret injection for SAS tokens any more, does that change things?
        // Do we need to pass config values in the ctor at all? Looks like we can just access the App config values directly in the methods (MvcApplication.Configuration..).
        public StorageService(
            string name,
            string blobConnectionString,
            string tableConnectionString)
        {
            Name = name;
            _useManagedIdentity = MvcApplication.StatusConfiguration.UseManagedIdentity;

            if (!string.IsNullOrWhiteSpace(MvcApplication.StatusConfiguration.ManagedIdentityClientId))
            {
                _managedIdentityClientId = MvcApplication.StatusConfiguration.ManagedIdentityClientId;
            }

            _blobConnectionString = blobConnectionString;
            _tableConnectionString = tableConnectionString;
        }

        public BlobClient GetBlobClient()
        {
            BlobContainerClient containerClient = GetBlobContainerClient();
            return containerClient.GetBlobClient(MvcApplication.StatusConfiguration.BlobName);
        }

        private BlobContainerClient GetBlobContainerClient()
        {
            BlobServiceClient blobServiceClient = GetBlobServiceClient();
            return blobServiceClient.GetBlobContainerClient(MvcApplication.StatusConfiguration.ContainerName);
        }

        private BlobServiceClient GetBlobServiceClient()
        {
            if (_useManagedIdentity)
            {
                if (string.IsNullOrWhiteSpace(_managedIdentityClientId))
                {
                    // 1. Using MSI with DefaultAzureCredential
                    return new BlobServiceClient(new Uri(_blobConnectionString), new DefaultAzureCredential());
                }
                else
                {
                    // 2. Using MSI with ClientId
                    return new BlobServiceClient(new Uri(_blobConnectionString), new ManagedIdentityCredential(_managedIdentityClientId));
                }
            }

            // 3. Using SAS token
            return new BlobServiceClient(_blobConnectionString);
        }

        public TableClient GetTableClient()
        {
            TableServiceClient tableServiceClient = GetTableServiceClient();
            return tableServiceClient.GetTableClient(MvcApplication.StatusConfiguration.TableName);
        }

        private TableServiceClient GetTableServiceClient()
        {
            if (_useManagedIdentity)
            {
                if (string.IsNullOrWhiteSpace(_managedIdentityClientId))
                {
                    // 1. Using MSI with DefaultAzureCredential
                    return new TableServiceClient(new Uri(_tableConnectionString), new DefaultAzureCredential());
                }
                else
                {
                    // 2. Using MSI with ClientId
                    return new TableServiceClient(new Uri(_tableConnectionString), new ManagedIdentityCredential(_managedIdentityClientId));
                }
            }

            // 3. Using SAS token
            return new TableServiceClient(_tableConnectionString);
        }
    }
}