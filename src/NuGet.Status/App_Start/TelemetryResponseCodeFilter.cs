// Copyright (c) .NET Foundation. All rights reserved. 
// Licensed under the Apache License, Version 2.0. See License.txt in the project root for license information. 

using System.Collections.Generic;
using Microsoft.ApplicationInsights.Channel;
using Microsoft.ApplicationInsights.DataContracts;
using Microsoft.ApplicationInsights.Extensibility;

namespace NuGet.Status
{
    public class TelemetryResponseCodeFilter : ITelemetryProcessor
    {
        /// <summary>
        /// This is a set of HTTP response (status) codes that are considered successful. This overrides the default of
        /// the <see cref="ResponseTelemetry.Success" /> property. If a response code is not in this set, the success
        /// boolean is left as is, meaning the Application Insights default is used.
        /// </summary>
        private static readonly HashSet<int> SuccessResponseCodes = new HashSet<int>
        {
            400,
            404
        };

        private readonly ITelemetryProcessor _next;

        public TelemetryResponseCodeFilter(ITelemetryProcessor next)
        {
            _next = next;
        }

        public void Process(ITelemetry item)
        {
            var request = item as RequestTelemetry;
            int responseCode;
            if (request != null && int.TryParse(request.ResponseCode, out responseCode))
            {
                if (SuccessResponseCodes.Contains(responseCode))
                {
                    request.Success = true;
                }
            }

            _next.Process(item);
        }
    }
}