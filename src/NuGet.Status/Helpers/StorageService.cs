// Copyright (c) .NET Foundation. All rights reserved. 
// Licensed under the Apache License, Version 2.0. See License.txt in the project root for license information. 

using Microsoft.WindowsAzure.Storage;
using Microsoft.WindowsAzure.Storage.Blob;
using Microsoft.WindowsAzure.Storage.Table;
using System.Threading.Tasks;

namespace NuGet.Status.Helpers
{
    public class StorageService
    {
        private const string TableNameKey = "Storage:TableName";
        private const string ContainerNameKey = "Storage:ContainerName";
        private const string BlobNameKey = "Storage:BlobName";

        public string ConnectionStringKey { get; }

        public StorageService(string connectionStringKey)
        {
            ConnectionStringKey = connectionStringKey;
        }

        public async Task<CloudBlockBlob> GetCloudBlockBlobAsync()
        {
            var blobName = await MvcApplication
                .ConfigurationProvider
                .GetOrThrow<string>(BlobNameKey);

            var container = await GetCloudBlobContainerAsync();

            return container.GetBlockBlobReference(blobName);
        }

        public async Task<CloudBlobContainer> GetCloudBlobContainerAsync()
        {
            var containerName = await MvcApplication
                .ConfigurationProvider
                .GetOrThrow<string>(ContainerNameKey);

            var storageAccount = await GetCloudStorageAccountAsync();

            return storageAccount
                .CreateCloudBlobClient()
                .GetContainerReference(containerName);
        }

        public async Task<CloudTable> GetCloudTableAsync()
        {
            var tableName = await MvcApplication
                .ConfigurationProvider
                .GetOrThrow<string>(TableNameKey);

            var storageAccount = await GetCloudStorageAccountAsync();

            return storageAccount
                .CreateCloudTableClient()
                .GetTableReference(tableName);
        }

        public async Task<CloudStorageAccount> GetCloudStorageAccountAsync()
        {
            var connectionString = await MvcApplication
                .ConfigurationProvider
                .GetOrThrow<string>(ConnectionStringKey);

            return CloudStorageAccount.Parse(connectionString);
        }
    }
}