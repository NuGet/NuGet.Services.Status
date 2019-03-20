// Copyright (c) .NET Foundation. All rights reserved. 
// Licensed under the Apache License, Version 2.0. See License.txt in the project root for license information. 

using System.Collections.Generic;
using System.Web.Mvc;

namespace NuGet.Status.Views
{
    public class CurrentConfig
    {
        public string Brand => "NuGet";

        public bool BlockSearchEngineIndexing => false;

        public string WarningBanner => "";

        public string RedesignBanner => "";

        public CookieConsentMessage CookieConsentMessage => null;

        public string ExternalBrandingMessage => MvcApplication.StatusConfiguration.ExternalBrandingMessage;

        public string ExternalBrandingUrl => MvcApplication.StatusConfiguration.ExternalBrandingUrl;

        public string ExternalAboutUrl => MvcApplication.StatusConfiguration.ExternalAboutUrl;

        public string ExternalPrivacyPolicyUrl => MvcApplication.StatusConfiguration.ExternalPrivacyPolicyUrl;

        public string ExternalTermsOfUseUrl => MvcApplication.StatusConfiguration.ExternalTermsOfUseUrl;

        public string TrademarksUrl => MvcApplication.StatusConfiguration.TrademarksUrl;

        public bool DeprecateNuGetPasswordLogins => false;

        public string ExternalStatusUrl => "/";
    }

    public class Config
    {
        public CurrentConfig Current => new CurrentConfig();
    }

    public class User
    {
        public string Username = "";
        public string EmailAddress => "";
        public string UnconfirmedEmailAddress => "";
    }

    public class CookieConsentMessage
    {
        public List<string> Javascripts => null;
        public string Message => "";
        public string MoreInfoUrl => "";
        public string MoreInfoAriaLabel => "";
        public string MoreInfoText => "";
    }

    public abstract class StatusViewBase : WebViewPage
    {
        public Config Config => new Config();

        public User CurrentUser => new User();

        public bool ShowAuthInHeader => false;
    }

    public abstract class StatusViewBase<T> : WebViewPage<T>
    {
        public Config Config => new Config();

        public User CurrentUser => new User();

        public CookieConsentMessage CookieConsentMessage => null;

        public bool ShowAuthInHeader => false;

        public bool LinkOpenSearchXml => false;
    }
}