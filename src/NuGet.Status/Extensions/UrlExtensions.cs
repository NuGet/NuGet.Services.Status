// Copyright (c) .NET Foundation. All rights reserved. 
// Licensed under the Apache License, Version 2.0. See License.txt in the project root for license information. 

using System.Web.Mvc;

namespace NuGet.Status.Helpers
{
    public static class UrlExtensions
    {
        public static string BaseUrl { get; set; }

        public static string Home(this UrlHelper url)
        {
            return BaseUrl;
        }

        public static string Absolute(this UrlHelper url, string path)
        {
            return path;
        }

        public static string Admin(this UrlHelper url)
        {
            return BaseUrl;
        }

        public static string PackageList(this UrlHelper url)
        {
            return BaseUrl + "/packages";
        }

        public static string UploadPackage(this UrlHelper url)
        {
            return BaseUrl + "/packages/manage/upload";
        }

        public static string Statistics(this UrlHelper url)
        {
            return BaseUrl + "/stats";
        }

        public static string Downloads(this UrlHelper url)
        {
            return BaseUrl + "/downloads";
        }

        public static string Contact(this UrlHelper url)
        {
            return BaseUrl + "/policies/Contact";
        }

        public static string Terms(this UrlHelper url)
        {
            return BaseUrl + "/policies/Terms";
        }

        public static string Privacy(this UrlHelper url)
        {
            return BaseUrl + "/policies/Privacy";
        }

        public static string User(this UrlHelper url, string username)
        {
            return BaseUrl + "/profiles/" + username;
        }

        public static string About(this UrlHelper url)
        {
            return BaseUrl + "/policies/About";
        }

        public static string LogOn(this UrlHelper url)
        {
            return BaseUrl + "/users/account/LogOn";
        }

        public static string LogOn(this UrlHelper url, string returnUrl)
        {
            return url.LogOn();
        }

        public static string SignUp(this UrlHelper url)
        {
            return BaseUrl + "/users/account/SignUp";
        }

        public static string SignUp(this UrlHelper url, string returnUrl)
        {
            return url.SignUp();
        }

        public static string Register(this UrlHelper url)
        {
            return url.LogOn();
        }

        public static string LogOff(this UrlHelper url)
        {
            return BaseUrl + "/users/account/LogOff";
        }

        public static string AccountSettings(this UrlHelper url)
        {
            return BaseUrl + "/account";
        }

        public static string ManageMyApiKeys(this UrlHelper url)
        {
            return BaseUrl + "/account/ApiKeys";
        }

        public static string ManageMyPackages(this UrlHelper url)
        {
            return BaseUrl + "/account/Packages";
        }

        public static string ManageMyOrganizations(this UrlHelper url)
        {
            return BaseUrl + "/account/Organizations";
        }

        public static string Current(this UrlHelper url)
        {
            return url.RequestContext.HttpContext.Request.RawUrl;
        }
    }
}