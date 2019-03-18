using NuGet.Services.Status;
using System.ComponentModel.DataAnnotations;
using System.Web.Mvc;

namespace NuGet.Status.Models
{
    public class EditStatusEventMessage : StatusEventChange
    {
        [Required]
        public string Timestamp { get; set; }

        [AllowHtml]
        [Display(Name = "Edit message")]
        public string EditMessage { get; set; }

        public bool Delete { get; set; }

        public EditStatusEventMessage()
        {
        }

        public EditStatusEventMessage(Event targetEvent, Message message)
            : base(targetEvent)
        {
            Timestamp = message.Time.ToString("o");
            EditMessage = message.Contents;
        }
    }
}