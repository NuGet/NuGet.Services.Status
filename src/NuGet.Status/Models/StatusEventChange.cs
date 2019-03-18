using NuGet.Services.Status;
using System.ComponentModel.DataAnnotations;

namespace NuGet.Status.Models
{
    public class StatusEventChange
    {
        [Required]
        public string AffectedComponentPath { get; set; }

        [Required]
        public ComponentStatus AffectedComponentStatus { get; set; }

        [Required]
        public string StartTime { get; set; }

        public string EndTime { get; set; }

        public StatusEventChange()
        {
        }

        public StatusEventChange(Event targetEvent)
        {
            AffectedComponentPath = targetEvent.AffectedComponentPath;
            StartTime = targetEvent.StartTime.ToString("o");
            EndTime = targetEvent.EndTime?.ToString("o");
            AffectedComponentStatus = ComponentStatus.Up;
        }
    }
}