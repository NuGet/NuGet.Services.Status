using NuGet.Services.Status;
using System.ComponentModel.DataAnnotations;

namespace NuGet.Status.Models
{
    public class EditStatusEvent : StatusEventChange
    {
        [Display(Name = "Is Active")]
        public bool IsActive { get; set; }

        public bool Delete { get; set; }

        public EditStatusEvent()
        {
            IsActive = true;
        }

        public EditStatusEvent(Event targetEvent)
            : base(targetEvent)
        {
            IsActive = targetEvent.EndTime == null;
        }
    }
}