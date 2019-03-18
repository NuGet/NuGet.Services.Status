// Copyright (c) .NET Foundation. All rights reserved. 
// Licensed under the Apache License, Version 2.0. See License.txt in the project root for license information. 

using System;
using System.Net;
using System.Threading.Tasks;
using Microsoft.Owin;
using Microsoft.Owin.Security;
using Microsoft.Owin.Security.Cookies;
using Microsoft.Owin.Security.OpenIdConnect;
using Owin;

[assembly: OwinStartup(typeof(NuGet.Status.Startup))]
namespace NuGet.Status
{
    public partial class Startup
    {
        private static string _clientId;
        private static string _aadInstance;
        private static string _tenant;
        private static string _authority;
        private static string _rootUri;
        private static string _redirectUri;
        private static bool _postStatusEnabled;

        public void Configuration(IAppBuilder app)
        {
            Init();

            // Enable HSTS
            app.Use(async (context, next) =>
            {
                context.Response.Headers.Add("Strict-Transport-Security", new string[] { "max-age=31536000; includeSubDomains" });
                await next.Invoke();
            });

            if (_postStatusEnabled)
            {
                app.SetDefaultSignInAsAuthenticationType(CookieAuthenticationDefaults.AuthenticationType);

                // Ignore secure cookie for local testing.
                var cookieSecure = _redirectUri.StartsWith("http://localhost")
                    ? CookieSecureOption.Never
                    : CookieSecureOption.Always;

                var options = new CookieAuthenticationOptions
                {
                    CookieHttpOnly = true,
                    CookieSecure = cookieSecure,
                    ExpireTimeSpan = TimeSpan.FromMinutes(10),
                    SlidingExpiration = true
                };

                app.UseCookieAuthentication(options);

                app.UseOpenIdConnectAuthentication(
                    new OpenIdConnectAuthenticationOptions
                    {
                        ClientId = _clientId,
                        Authority = _authority,
                        RedirectUri = _redirectUri,
                        PostLogoutRedirectUri = _rootUri,
                        TokenValidationParameters = new System.IdentityModel.Tokens.TokenValidationParameters
                        {
                            ValidateIssuer = false,
                            RoleClaimType = "roles"
                        },
                        Notifications = new OpenIdConnectAuthenticationNotifications
                        {
                            AuthenticationFailed = context =>
                            {
                                context.HandleResponse();
                                context.Response.Redirect("/Errors/BadRequest");
                                return Task.FromResult(0);
                            }
                        }
                    });
            }
        }

        private void Init()
        {
            // Ensure that SSLv3 is disabled and that Tls v1.2 is enabled.
            ServicePointManager.SecurityProtocol &= ~SecurityProtocolType.Ssl3;
            ServicePointManager.SecurityProtocol |= SecurityProtocolType.Tls12;

            var configurationProvider = MvcApplication.ConfigurationProvider;

#pragma warning disable CS0618 // Type or member is obsolete
            _postStatusEnabled = configurationProvider.GetOrDefaultSync<bool>("Ida:AdminEnabled");
#pragma warning restore CS0618 // Type or member is obsolete

            if (_postStatusEnabled)
            {
                _clientId = configurationProvider.GetOrThrowSync<string>("Ida:ClientId");

                _aadInstance = configurationProvider.GetOrThrowSync<string>("Ida:AADInstance");
                _tenant = configurationProvider.GetOrThrowSync<string>("Ida:Tenant");
                _authority = new Uri(new Uri(_aadInstance), _tenant).ToString();

                _rootUri = configurationProvider.GetOrThrowSync<string>("Ida:RootUri");
                _redirectUri = new Uri(new Uri(_rootUri), "admin").ToString();
            }
        }
    }
}