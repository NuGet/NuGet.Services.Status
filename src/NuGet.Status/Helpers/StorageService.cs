// Copyright (c) .NET Foundation. All rights reserved. 
// Licensed under the Apache License, Version 2.0. See License.txt in the project root for license information. 

using Microsoft.WindowsAzure.Storage;
using Microsoft.WindowsAzure.Storage.Blob;
using Microsoft.WindowsAzure.Storage.Table;
using NuGet.Status.Utilities;
using System;
using System.Collections.Generic;
using System.Security.Cryptography;
using System.Text;

namespace NuGet.Status.Helpers
{
    public class StorageService
    {
        private readonly Func<string> _getConnectionString;

        public string Name { get; }

        public StorageService(string name, Func<string> getConnectionString)
        {
            Name = name;
            _getConnectionString = getConnectionString;
        }

        public CloudBlockBlob GetCloudBlockBlob()
        {
            var container = GetCloudBlobContainer();
            return container.GetBlockBlobReference(MvcApplication.StatusConfiguration.BlobName);
        }

        public CloudBlobContainer GetCloudBlobContainer()
        {
            var storageAccount = GetCloudStorageAccount();

            return storageAccount
                .CreateCloudBlobClient()
                .GetContainerReference(MvcApplication.StatusConfiguration.ContainerName);
        }

        public CloudTable GetCloudTable()
        {
            var storageAccount = GetCloudStorageAccount();

            return storageAccount
                .CreateCloudTableClient()
                .GetTableReference(MvcApplication.StatusConfiguration.TableName);
        }

        public CloudStorageAccount GetCloudStorageAccount()
        {
            var connectionString = _getConnectionString();
            QuietLog.Event("ConnectionString", new Dictionary<string, string> { { "hash", GetHash(connectionString) } });
            return CloudStorageAccount.Parse(connectionString);
        }

        private static string GetHash(string str)
        {
            using (var hasher = SHA256.Create())
            {
                var hash = hasher.ComputeHash(Encoding.UTF8.GetBytes(str));
                return BitConverter.ToString(hash).Replace("-", "");
            }
        }
    }
}