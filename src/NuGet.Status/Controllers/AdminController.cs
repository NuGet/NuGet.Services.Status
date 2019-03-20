// Copyright (c) .NET Foundation. All rights reserved. 
// Licensed under the Apache License, Version 2.0. See License.txt in the project root for license information. 

using System;
using System.Collections.Generic;
using System.Runtime.CompilerServices;
using System.Threading.Tasks;
using System.Web.Mvc;
using Microsoft.WindowsAzure.Storage.Table;
using NuGet.Services.Status.Table.Manual;
using NuGet.Status.Helpers;
using NuGet.Status.Models;
using NuGet.Status.Utilities;

namespace NuGet.Status.Controllers
{
    [Authorize(Roles = "Admins")]
    public class AdminController : AppController
    {
        public const string UpdatedTempDataKey = "Updated";

        [HttpGet]
        public ActionResult Index()
        {
            return View(nameof(Index), CachedServiceStatus?.ServiceStatus);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public Task<ActionResult> CreateNewEvent(CreateStatusEvent model)
        {
            var entity = new AddStatusEventManualChangeEntity(model.AffectedComponentPath, model.AffectedComponentStatus, model.Message, model.IsActive);
            return RunUpdateStatusTask(entity);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public Task<ActionResult> EditEvent(EditStatusEvent model)
        {
            var startTime = ParseModelDateTime(model.StartTime);

            ManualStatusChangeEntity entity;
            if (model.Delete)
            {
                entity = new DeleteStatusEventManualChangeEntity(
                    model.AffectedComponentPath,
                    startTime);
            }
            else
            {
                entity = new EditStatusEventManualChangeEntity(
                    model.AffectedComponentPath,
                    model.AffectedComponentStatus,
                    startTime,
                    model.IsActive);
            }

            return RunUpdateStatusTask(entity);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public Task<ActionResult> AddMessage(AddStatusEventMessage model)
        {
            var entity = new AddStatusMessageManualChangeEntity(model.AffectedComponentPath, ParseModelDateTime(model.StartTime), model.Message, !model.ShouldDeactivate);
            return RunUpdateStatusTask(entity);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public Task<ActionResult> EditMessage(EditStatusEventMessage model)
        {
            var startTime = ParseModelDateTime(model.StartTime);
            var timestamp = ParseModelDateTime(model.Timestamp);

            ManualStatusChangeEntity entity;
            if (model.Delete)
            {
                entity = new DeleteStatusMessageManualChangeEntity(
                    model.AffectedComponentPath,
                    startTime,
                    timestamp);
            }
            else
            {
                entity = new EditStatusMessageManualChangeEntity(
                    model.AffectedComponentPath,
                    startTime,
                    timestamp,
                    model.EditMessage);
            }

            return RunUpdateStatusTask(entity);
        }

        private const string ManuallyUpdatedStatusEvent = "ManuallyUpdatedStatus";

        private async Task<ActionResult> RunUpdateStatusTask(ManualStatusChangeEntity entity, [CallerMemberName] string memberName = "")
        {
            bool success = false;

            // Enumerate through the configured storages, trying to insert the entity.
            foreach ( var storageService in new[] { StorageHelper.PrimaryStorage, StorageHelper.SecondaryStorage })
            {
                if (success = await InsertEntityToStorage(storageService, entity))
                {
                    break;
                }
            }

            QuietLog.Event(ManuallyUpdatedStatusEvent, new Dictionary<string, string> { { "success", success.ToString() } });

            TempData[UpdatedTempDataKey] = success;
            return RedirectToAction(nameof(Index));
        }

        private async Task<bool> InsertEntityToStorage(StorageService storageService, ManualStatusChangeEntity entity)
        {
            try
            {
                var table = storageService.GetCloudTable();
                var operation = TableOperation.Insert(entity);
                await table.ExecuteAsync(operation);
                return true;
            }
            catch (Exception e)
            {
                QuietLog.Log($"{nameof(AdminController)}.{nameof(InsertEntityToStorage)}", $"Failed to insert entity to table in {storageService.Name} storage!", e);
            }

            return false;
        }

        private static DateTime ParseModelDateTime(string dateTime)
        {
            return DateTime.Parse(dateTime).ToUniversalTime();
        }
    }
}