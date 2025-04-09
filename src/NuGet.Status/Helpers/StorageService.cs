// Copyright (c) .NET Foundation. All rights reserved. 
// Licensed under the Apache License, Version 2.0. See License.txt in the project root for license information. 

using Microsoft.WindowsAzure.Storage;
using Microsoft.WindowsAzure.Storage.Blob;
using Microsoft.WindowsAzure.Storage.Table;
using System;
using Azure.Data.Tables;
using Azure.Identity;
using Azure.Storage.Blobs;

namespace NuGet.Status.Helpers
{
    public class StorageService
    {
        private readonly Func<string> _getConnectionString; // OLD

        private readonly bool _useManagedIdentity;
        private readonly string _managedIdentityClientId;
        private readonly Func<string> _getBlobConnectionString;
        private readonly Func<string> _getTableConnectionString;

        public string Name { get; }

        public StorageService(string name, Func<string> getConnectionString) // OLD
        {
            Name = name;
            _getConnectionString = getConnectionString;
        }

        public CloudBlockBlob GetCloudBlockBlob() // OLD
        {
            var container = GetCloudBlobContainer();
            return container.GetBlockBlobReference(MvcApplication.StatusConfiguration.BlobName);
        }

        public CloudBlobContainer GetCloudBlobContainer() // OLD
        {
            var storageAccount = GetCloudStorageAccount();

            return storageAccount
                .CreateCloudBlobClient()
                .GetContainerReference(MvcApplication.StatusConfiguration.ContainerName);
        }

        public CloudTable GetCloudTable() // OLD
        {
            var storageAccount = GetCloudStorageAccount();

            return storageAccount
                .CreateCloudTableClient()
                .GetTableReference(MvcApplication.StatusConfiguration.TableName);
        }

        public CloudStorageAccount GetCloudStorageAccount() // OLD
        {
            return CloudStorageAccount.Parse(_getConnectionString());
        }

        // Do we need lazy loading for connection strings? We're not using KV secret injection for SAS tokens any more, does that change things?
        // Do we need to pass config values in the ctor at all? Looks like we can just use the config values directly in the methods (MvcApplication...).
        public StorageService(
            string name,
            string useManagedIdentityStr,
            string managedIdentityClientId,
            Func<string> getBlobConnectionString,
            Func<string> getTableConnectionString)
        {
            _useManagedIdentity = false;
            if (!bool.TryParse(useManagedIdentityStr, out _useManagedIdentity))
            {
                throw new ArgumentException("Invalid value for Storage:UseManagedIdentity. Expected 'true' or 'false'.", nameof(useManagedIdentityStr));
            }

            if (!string.IsNullOrWhiteSpace(managedIdentityClientId))
            {
                _managedIdentityClientId = managedIdentityClientId;
            }

            _getBlobConnectionString = getBlobConnectionString;
            _getTableConnectionString = getTableConnectionString;
        }

        public BlobClient GetBlobClient()
        {
            BlobContainerClient containerClient = GetBlobContainerClient();
            return containerClient.GetBlobClient(MvcApplication.StatusConfiguration.BlobName);
        }

        public BlobContainerClient GetBlobContainerClient()
        {
            BlobServiceClient blobServiceClient = GetBlobServiceClient();
            return blobServiceClient.GetBlobContainerClient(MvcApplication.StatusConfiguration.ContainerName);
        }

        public BlobServiceClient GetBlobServiceClient()
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

        public TableClient GetTableClient()
        {
            TableServiceClient tableServiceClient = GetTableServiceClient();
            return tableServiceClient.GetTableClient(MvcApplication.StatusConfiguration.TableName);
        }

        public TableServiceClient GetTableServiceClient()
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