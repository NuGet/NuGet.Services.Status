// Copyright (c) .NET Foundation. All rights reserved. 
// Licensed under the Apache License, Version 2.0. See License.txt in the project root for license information. 

using Azure.Data.Tables;
using Azure.Storage;
using Azure.Storage.Blobs;

using System;
using Azure.Identity;
using Microsoft.Extensions.Configuration;
using static System.Net.WebRequestMethods;

namespace NuGet.Status.Helpers
{
    public class StorageService
    {
        private readonly Func<string> _getAccountName;
        private readonly Func<string> _getManagedIdentity;

        public string Name { get; }

        public StorageService(string name, Func<string> getAccountName, Func<string> getManagedIdentity)
        {
            Name = name;
            _getAccountName = getAccountName;
            _getManagedIdentity = getManagedIdentity;
        }

        public BlobClient GetBlobClient()
        {
            var container = GetBlobContainerClient();
            return container.GetBlobClient(MvcApplication.StatusConfiguration.BlobName);
        }

        public BlobContainerClient GetBlobContainerClient()
        {
            var client = GetBlobServiceClient();
            return client.GetBlobContainerClient(MvcApplication.StatusConfiguration.ContainerName);
        }

        public TableClient GetTableClient()
        {
            var client = GetTableServiceClient();
            return client.GetTableClient(MvcApplication.StatusConfiguration.TableName);
        }

        public BlobServiceClient GetBlobServiceClient()
        {
            var uri = new Uri($"BlobEndpoint=https://{_getAccountName()}.blob.core.windows.net");
            var tokenCredential = new ManagedIdentityCredential(_getManagedIdentity());
            var options = new BlobClientOptions();
            return new BlobServiceClient(uri, tokenCredential, options);
        }

        public TableServiceClient GetTableServiceClient()
        {
            var uri = new Uri($"https://{_getAccountName()}.table.core.windows.net");
            var tokenCredential = new ManagedIdentityCredential(_getManagedIdentity());
            return new TableServiceClient(uri, tokenCredential);
        }

    }
}