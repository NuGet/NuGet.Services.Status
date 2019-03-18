// Copyright (c) .NET Foundation. All rights reserved. 
// Licensed under the Apache License, Version 2.0. See License.txt in the project root for license information. 

using System;
using System.Configuration;
using System.IO;
using System.Linq;
using System.Security.Claims;
using System.Web.Helpers;
using System.Web.Hosting;
using System.Web.Mvc;
using System.Web.Optimization;
using System.Web.Routing;
using Newtonsoft.Json.Linq;
using NuGet.Services.Configuration;
using NuGet.Status.Configuration;
using NuGet.Status.Helpers;
using NuGet.Status.Utilities;

namespace NuGet.Status
{
    public class MvcApplication : System.Web.HttpApplication
    {
        public static IConfigurationProvider ConfigurationProvider;

        static MvcApplication()
        {
            var configurationDictionary = ConfigurationManager.AppSettings.AllKeys.ToDictionary(key => key, key => ConfigurationManager.AppSettings[key]);

            var filename = configurationDictionary["Config:FileName"];
            var configPath = HostingEnvironment.MapPath($@"~/App_Data/{filename}.json");
            var configJson = File.ReadAllText(configPath);
            var config = JObject.Parse(configJson);
            foreach (var property in config.Properties())
            {
                configurationDictionary[property.Name] = property.Value.ToString();
            }

            var secretReaderFactory = new SecretReaderFactory(configurationDictionary);
            var secretReader = secretReaderFactory.CreateSecretReader();
            var secretInjector = secretReaderFactory.CreateSecretInjector(secretReader);

            ConfigurationProvider = new SecretConfigurationProvider(secretInjector, configurationDictionary);
        }

        protected void Application_Start()
        {
            var galleryVirtualPathProvider = new NuGetGalleryVirtualPathProvider();
            HostingEnvironment.RegisterVirtualPathProvider(galleryVirtualPathProvider);
            BundleTable.VirtualPathProvider = HostingEnvironment.VirtualPathProvider;

            AreaRegistration.RegisterAllAreas();
            BundleConfig.RegisterBundles(BundleTable.Bundles);
            FilterConfig.RegisterGlobalFilters(GlobalFilters.Filters);
            RouteConfig.RegisterRoutes(RouteTable.Routes);
            AntiForgeryConfig.UniqueClaimTypeIdentifier = ClaimTypes.NameIdentifier;

#pragma warning disable CS0618 // Type or member is obsolete
            UrlExtensions.BaseUrl = ConfigurationProvider
                .GetOrDefaultSync("NuGetBaseUrl", "https://www.nuget.org/")
                .TrimEnd('/');
#pragma warning restore CS0618 // Type or member is obsolete

            HostingEnvironment.QueueBackgroundWorkItem(token =>
            {
                return ServiceStatusHelper.ReloadServiceStatusForever(token);
            });
        }

        protected void Application_Error(object sender, EventArgs e)
        {
            QuietLog.Log(
                nameof(Application_Error), 
                "Exception thrown by request!", 
                Server.GetLastError());
        }

        private static JObject GetConfigFile(string filename)
        {
            var configPath = HostingEnvironment.MapPath($@"~/App_Data/{filename}.json");
            var configJson = File.ReadAllText(configPath);
            return JObject.Parse(configJson);
        }
    }
}
