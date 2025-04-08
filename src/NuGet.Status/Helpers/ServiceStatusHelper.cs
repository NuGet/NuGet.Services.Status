// Copyright (c) .NET Foundation. All rights reserved. 
// Licensed under the Apache License, Version 2.0. See License.txt in the project root for license information. 

using System;
using System.Collections.Generic;
using System.IO;
using System.Threading;
using System.Threading.Tasks;
using Azure.Storage.Blobs;
using NuGet.Status.Utilities;

namespace NuGet.Status.Helpers
{
    public static class ServiceStatusHelper
    {
        private const string ReloadedStatusEvent = "ReloadedStatus";
        private const string ReloadedStatusEventSuccessProperty = "Success";

        /// <summary>
        /// The frequency to reload the service status at.
        /// </summary>
        private static readonly TimeSpan ReloadEvery = TimeSpan.FromSeconds(30);
        
        public static CachedServiceStatus CachedServiceStatus { get; private set; }

        public static async Task ReloadServiceStatusForever(CancellationToken token)
        {
            while (!token.IsCancellationRequested)
            {
                await ReloadServiceStatus(token);
                await Task.Delay(ReloadEvery, token);
            }
        }

        private static async Task ReloadServiceStatus(CancellationToken token)
        {
            var success = false;

            try
            {
                CachedServiceStatus = await GetServiceStatusAsync(token);
                success = true;
            }
            catch (Exception e)
            {
                QuietLog.Log($"{nameof(ServiceStatusHelper)}.{nameof(ReloadServiceStatus)}", "Failed to reload!", e);
            }
            finally
            {
                QuietLog.Event(
                    ReloadedStatusEvent,
                    new Dictionary<string, string> { { ReloadedStatusEventSuccessProperty, success.ToString() } });
            }
        }

        private static async Task<CachedServiceStatus> GetServiceStatusAsync(CancellationToken token)
        {
            var blob = StorageHelper.PrimaryStorage.GetCloudBlockBlob();
            var json = await blob.DownloadTextAsync(token);
            return new CachedServiceStatus(json);

            /*
            BlobClient blobClient = StorageHelper.PrimaryStorage.GetBlobClient();
            var blobResponse = await blobClient.DownloadAsync(token);

            using (var streamReader = new StreamReader(blobResponse.Value.Content))
            {
                var json = await streamReader.ReadToEndAsync();
                return new CachedServiceStatus(json);
            }
            */
        }
    }
}