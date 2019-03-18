using NuGet.Services.Status;
using System.ComponentModel.DataAnnotations;
using System.Web.Mvc;

namespace NuGet.Status.Models
{
    public class AddStatusEventMessage : StatusEventChange
    {
        [AllowHtml]
        [Required]
        [Display(Name = "Message (accepts simple Markdown)")]
        public string Message { get; set; }

        [Display(Name = "Deactivate this event")]
        public bool ShouldDeactivate { get; set; }

        public AddStatusEventMessage()
        {
        }

        public AddStatusEventMessage(Event targetEvent)
            : base(targetEvent)
        {
        }
    }
}