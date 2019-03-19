// Copyright (c) .NET Foundation. All rights reserved.
// Licensed under the Apache License, Version 2.0. See License.txt in the project root for license information.

using System.Diagnostics.CodeAnalysis;

[module: SuppressMessage("Microsoft.Security.Web.Configuration", "CA3103:EnableFormsRequireSSL", Justification = "Forms authentication is not enabled.")]
[module: SuppressMessage("Microsoft.Security.Web.Configuration", "CA3119:EnableHttpCookiesRequireSsl", Justification = "Enabled in root web.config, which is inherited by other configurations.")]
[module: SuppressMessage("Microsoft.Security.Web.Configuration", "CA3135:EnableRoleManagerCookieRequireSsl", Justification = "Only used by a test project in the submodule that is not a part of this project.")]