// Copyright (c) .NET Foundation. All rights reserved. 
// Licensed under the Apache License, Version 2.0. See License.txt in the project root for license information. 

using System;
using System.Collections.Generic;
using System.Linq;
using Azure.Data.Tables;
using Azure.Identity;
using Azure.Storage.Blobs;

namespace NuGet.Status.Helpers
{
    public class StorageService
    {
        private readonly Func<string> _getStorageConnectionString;
        private readonly bool _useManagedIdentity;
        private readonly string _managedIdentityClientId;

        private BlobClient _blobClient;
        private TableClient _tableClient;

        public string Name { get; }

        public StorageService(
            string name,
            Func<string> getStorageConnectionString)
        {
            Name = name;
            _useManagedIdentity = MvcApplication.StatusConfiguration.UseManagedIdentity;

            if (!string.IsNullOrWhiteSpace(MvcApplication.StatusConfiguration.ManagedIdentityClientId))
            {
                _managedIdentityClientId = MvcApplication.StatusConfiguration.ManagedIdentityClientId;
            }

            _getStorageConnectionString = getStorageConnectionString;
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
                    return new BlobServiceClient(GetBlobServiceEndpoint(), new DefaultAzureCredential());
                }
                else
                {
                    // 2. Using MSI with ClientId
                    return new BlobServiceClient(GetBlobServiceEndpoint(), new ManagedIdentityCredential(_managedIdentityClientId));
                }
            }

            // 3. Using SAS token
            return new BlobServiceClient(_getStorageConnectionString());
        }

        private TableServiceClient GetTableServiceClient()
        {
            if (_useManagedIdentity)
            {
                if (string.IsNullOrWhiteSpace(_managedIdentityClientId))
                {
                    // 1. Using MSI with DefaultAzureCredential
                    return new TableServiceClient(GetTableServiceEndpoint(), new DefaultAzureCredential());
                }
                else
                {
                    // 2. Using MSI with ClientId
                    return new TableServiceClient(GetTableServiceEndpoint(), new ManagedIdentityCredential(_managedIdentityClientId));
                }
            }

            // 3. Using SAS token
            return new TableServiceClient(_getStorageConnectionString());
        }

        private Uri GetBlobServiceEndpoint()
        {
            var temp = new BlobServiceClient(_getStorageConnectionString());
            return temp.Uri;
        }

        /// For <see cref="BlobServiceClient"/>, we can pass the connection string to the default ctor (even without any auth),
        /// and it parses out the BlobEndpoint URI for us.
        /// For <see cref="TableServiceClient"/>, this ctor requires a SAS token to be passed in the connection string,
        /// so we cannot use the same method. We need to extract the TableEndpoint URI from the connection string manually.
        private Uri GetTableServiceEndpoint()
        {
            var connectionString = _getStorageConnectionString();
            var keyValuePairs = connectionString.Split(';')
                .Select(part => part.Split('='))
                .Where(parts => parts.Length == 2)
                .ToDictionary(parts => parts[0], parts => parts[1]);

            if (keyValuePairs.TryGetValue("TableEndpoint", out var tableEndpoint))
            {
                return new Uri(tableEndpoint);
            }
            else
            {
                throw new ArgumentException("TableEndpoint not found in connection string.");
            }
        }
    }
}