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
        private readonly Func<string> _getBlobConnectionString;
        private readonly Func<string> _getTableConnectionString;
        private readonly bool _useManagedIdentity;
        private readonly string _managedIdentityClientId;

        private BlobClient _blobClient;
        private TableClient _tableClient;

        public string Name { get; }

        public StorageService(
            string name,
            Func<string> getBlobConnectionString,
            Func<string> getTableConnectionString)
        {
            Name = name;
            _useManagedIdentity = MvcApplication.StatusConfiguration.UseManagedIdentity;

            if (!string.IsNullOrWhiteSpace(MvcApplication.StatusConfiguration.ManagedIdentityClientId))
            {
                _managedIdentityClientId = MvcApplication.StatusConfiguration.ManagedIdentityClientId;
            }

            _getBlobConnectionString = getBlobConnectionString;
            _getTableConnectionString = getTableConnectionString;
        }

        public BlobClient GetBlobClient()
        {
            if (_blobClient is null || _useManagedIdentity is false)
            {
                BlobContainerClient containerClient = GetBlobContainerClient();
                _blobClient = containerClient.GetBlobClient(MvcApplication.StatusConfiguration.BlobName);
            }

            return _blobClient;
        }

        public TableClient GetTableClient()
        {
            if (_tableClient is null || _useManagedIdentity is false)
            {
                TableServiceClient tableServiceClient = GetTableServiceClient();
                _tableClient = tableServiceClient.GetTableClient(MvcApplication.StatusConfiguration.TableName);
            }

            return _tableClient;
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
                    return new BlobServiceClient(new Uri(_getBlobConnectionString()), new DefaultAzureCredential());
                }
                else
                {
                    // 2. Using MSI with ClientId
                    return new BlobServiceClient(new Uri(_getBlobConnectionString()), new ManagedIdentityCredential(_managedIdentityClientId));
                }
            }

            // 3. Using SAS token
            return new BlobServiceClient(_getBlobConnectionString());
        }

        private TableServiceClient GetTableServiceClient()
        {
            if (_useManagedIdentity)
            {
                if (string.IsNullOrWhiteSpace(_managedIdentityClientId))
                {
                    // 1. Using MSI with DefaultAzureCredential
                    return new TableServiceClient(new Uri(_getTableConnectionString()), new DefaultAzureCredential());
                }
                else
                {
                    // 2. Using MSI with ClientId
                    return new TableServiceClient(new Uri(_getTableConnectionString()), new ManagedIdentityCredential(_managedIdentityClientId));
                }
            }

            // 3. Using SAS token
            return new TableServiceClient(_getTableConnectionString());
        }
    }
}