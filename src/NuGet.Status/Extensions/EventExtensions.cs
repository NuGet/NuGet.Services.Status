using NuGet.Services.Status;
using NuGet.Services.Status.Table;
using System;

namespace NuGet.Status.Extensions
{
    public static class EventExtensions
    {
        /// <summary>
        /// Returns a string that can be used as the value of an HTML id attribute for <paramref name="target"/>.
        /// See <see cref="StringExtensions.ToHtmlSafeId(string)"/>.
        /// </summary>
        public static string ToHtmlSafeRowKeyId(this Event target)
        {
            if (target == null)
            {
                throw new ArgumentNullException(nameof(target));
            }

            return new EventEntity(
                target.AffectedComponentPath, 
                target.StartTime, 
                ComponentStatus.Up, 
                target.EndTime)
                .RowKey
                .ToHtmlSafeId();
        }
    }
}