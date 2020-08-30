using System;

namespace EssentialUIKit.Models.Api
{
    public class SelectedRunner
    {
        public int SelectedRunnerId { get; set; }
        public Runner Runner { get; set; }
        public User User { get; set; }
        public DateTime? DateSelected { get; set; }
    }
}
