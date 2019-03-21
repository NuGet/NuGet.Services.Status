using Markdig;
using NuGet.Services.Status;

namespace NuGet.Status.Extensions
{
    public static class MessageExtensions
    {
        /// <summary>
        /// Converts the <see cref="Message.Contents"/> of <paramref name="message"/> to HTML.
        /// </summary>
        public static string GetHtmlContents(this Message message)
        {
            var pipelineBuilder = new MarkdownPipelineBuilder();
            pipelineBuilder.DisableHtml();
            return Markdown.ToHtml(message.Contents, pipelineBuilder.Build());
        }
    }
}