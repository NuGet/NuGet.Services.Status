using NuGet.Services.Status;
using System.ComponentModel.DataAnnotations;
using System.Web.Mvc;

namespace NuGet.Status.Models
{
    public class CreateStatusEvent
    {
        [Display(Name = "Is Active")]
        public bool IsActive { get; set; }

        [Required]
        [Display(Name = "Affected Component Path (e.g. NuGet/NuGet.org or NuGet/Restore/V2 Protocol/North Central US)")]
        public string AffectedComponentPath { get; set; }

        [Display(Name = "Affected Component Status")]
        public ComponentStatus AffectedComponentStatus { get; set; }

        [AllowHtml]
        [Required]
        [Display(Name = "Message (accepts simple Markdown)")]
        public string Message { get; set; }

        public CreateStatusEvent()
        {
            IsActive = true;
            AffectedComponentPath = "NuGet";
            AffectedComponentStatus = ComponentStatus.Degraded;
        }
    }
}