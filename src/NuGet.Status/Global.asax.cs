// Copyright (c) .NET Foundation. All rights reserved. 
// Licensed under the Apache License, Version 2.0. See License.txt in the project root for license information. 

using System;
using System.Configuration;
using System.Linq;
using System.Security.Claims;
using System.Threading;
using System.Threading.Tasks;
using System.Web.Helpers;
using System.Web.Hosting;
using System.Web.Mvc;
using System.Web.Optimization;
using System.Web.Routing;
using Microsoft.ApplicationInsights.Extensibility;
using NuGet.Services.Configuration;
using NuGet.Status.Configuration;
using NuGet.Status.Helpers;
using NuGet.Status.Utilities;

namespace NuGet.Status
{
    public class MvcApplication : System.Web.HttpApplication
    {
        private static TimeSpan ConfigurationRefreshPeriod = new TimeSpan(days: 1, hours: 0, minutes: 0, seconds: 0);
        private static readonly IConfigurationFactory _configurationFactory;

        public static StatusConfiguration StatusConfiguration { get; private set; }

        /// <remarks>
        /// The IDA configuration is only refreshed once.
        /// It is only used on application startup in <see cref="Startup.Init"/>.
        /// </remarks>
        private static Lazy<IdaConfiguration> _idaConfiguration = new Lazy<IdaConfiguration>(
            () => _configurationFactory.Get<IdaConfiguration>().Result);
        public static IdaConfiguration IdaConfiguration => _idaConfiguration.Value;

        static MvcApplication()
        {
            var configurationDictionary = ConfigurationManager.AppSettings.AllKeys.ToDictionary(key => key, key => ConfigurationManager.AppSettings[key]);
            var secretReaderFactory = new SecretReaderFactory(configurationDictionary);
            var secretReader = secretReaderFactory.CreateSecretReader();
            var secretInjector = secretReaderFactory.CreateSecretInjector(secretReader);

            var configurationProvider = new AppSettingsConfigurationProvider(secretInjector);
            _configurationFactory = new ConfigurationFactory(configurationProvider);

            StatusConfiguration = _configurationFactory.Get<StatusConfiguration>().Result;
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

            UrlExtensions.BaseUrl = StatusConfiguration.NuGetBaseUrl.TrimEnd('/');

            var instrumentationKey = StatusConfiguration.ApplicationInsightsKey;
            if (!string.IsNullOrWhiteSpace(instrumentationKey))
            {
                TelemetryConfiguration.Active.InstrumentationKey = instrumentationKey;
            }

            HostingEnvironment.QueueBackgroundWorkItem(ServiceStatusHelper.ReloadServiceStatusForever);
            HostingEnvironment.QueueBackgroundWorkItem(RefreshConfigurationForever);
        }

        protected void Application_Error(object sender, EventArgs e)
        {
            QuietLog.Log(
                nameof(Application_Error), 
                "Exception thrown by request!", 
                Server.GetLastError());
        }

        private async Task RefreshConfigurationForever(CancellationToken token)
        {
            await Task.Delay(ConfigurationRefreshPeriod);
            StatusConfiguration = await _configurationFactory.Get<StatusConfiguration>();
            await RefreshConfigurationForever(token);
        }
    }
}
