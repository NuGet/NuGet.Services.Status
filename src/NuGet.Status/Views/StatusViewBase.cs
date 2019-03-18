using NuGet.Services.Configuration;
using System.Collections.Generic;
using System.Web.Mvc;

namespace NuGet.Status.Views
{
    public class CurrentConfig
    {
        public static IConfigurationProvider configurationProvider = MvcApplication.ConfigurationProvider;

        public string Brand => "NuGet";

        public string WarningBanner => "";

        public string RedesignBanner => "";

        public CookieConsentMessage CookieConsentMessage => null;

#pragma warning disable CS0618 // Type or member is obsolete
        public string ExternalBrandingMessage => configurationProvider.GetOrDefaultSync<string>("ExternalBrandingMessage");

        public string ExternalBrandingUrl => configurationProvider.GetOrDefaultSync<string>("ExternalBrandingUrl");

        public string ExternalAboutUrl => configurationProvider.GetOrDefaultSync<string>("ExternalAboutUrl");

        public string ExternalPrivacyPolicyUrl => configurationProvider.GetOrDefaultSync<string>("ExternalPrivacyPolicyUrl");

        public string ExternalTermsOfUseUrl => configurationProvider.GetOrDefaultSync<string>("ExternalTermsOfUseUrl");

        public string TrademarksUrl => configurationProvider.GetOrDefaultSync<string>("TrademarksUrl");
#pragma warning restore CS0618 // Type or member is obsolete

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