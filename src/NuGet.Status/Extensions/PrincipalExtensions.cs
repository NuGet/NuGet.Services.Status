using System.Security.Principal;

namespace NuGet.Status.Extensions
{
    /// <summary>
    /// These are extensions to <see cref="IPrincipal"/> that are defined by <see cref="NuGetGallery"/> and used in the site layout.
    /// </summary>
    public static class PrincipalExtensions
    {
        public static bool HasPasswordLogin(this IPrincipal principal)
        {
            return false;
        }

        public static bool HasExternalLogin(this IPrincipal principal)
        {
            return false;
        }

        public static bool HasMultiFactorAuthenticationEnabled(this IPrincipal principal)
        {
            return false;
        }

        public static bool WasMicrosoftAccountUsedForSignin(this IPrincipal principal)
        {
            return false;
        }
    }
}