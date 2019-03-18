// Copyright (c) .NET Foundation. All rights reserved. 
// Licensed under the Apache License, Version 2.0. See License.txt in the project root for license information. 

using System;
using System.Collections.Generic;
using Microsoft.ApplicationInsights;

namespace NuGet.Status.Utilities
{
    public static class QuietLog
    {
        public static void Event(string eventName, IDictionary<string, string> properties = null)
        {
            var telemetryClient = new TelemetryClient();
            telemetryClient.TrackEvent(eventName, properties);
        }

        public static void Log(string source, string message, Exception exception)
        {
            var telemetryClient = new TelemetryClient();
            telemetryClient.TrackException(exception, new Dictionary<string, string>
            {
                { "Source", source },
                { "Message", message }
            });
        }
    }
}