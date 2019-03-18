using MarkdownDeep;
using NuGet.Services.Status;

namespace NuGet.Status.Extensions
{
    public static class MessageExtensions
    {
        /// <summary>
        /// Converts the <see cref="Message.Contents"/> of <paramref name="message"/> to HTML.
        /// </summary>
        /// <remarks>
        /// The <see cref="Markdown"/> instance is NOT thread-safe. It must be created on every request.
        /// </remarks>
        public static string GetHtmlContents(this Message message)
        {
            return new Markdown
            {
                SafeMode = true,
                ExtraMode = false
            }.Transform(message.Contents);
        }
    }
}