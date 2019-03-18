using NuGet.Services.Status;
using System;

namespace NuGet.Status.Extensions
{
    public static class ComponentExtensions
    {
        /// <summary>
        /// Returns a string that can be used as the value of an HTML id attribute for <paramref name="component"/>.
        /// See <see cref="StringExtensions.ToHtmlSafeId(string)"/>.
        /// </summary>
        public static string ToHtmlSafePathId(this IReadOnlyComponent component)
        {
            if (component == null)
            {
                throw new ArgumentNullException(nameof(component));
            }

            return component.Path.ToHtmlSafeId();
        }
    }
}