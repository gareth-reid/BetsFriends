using System;
using System.Collections.Generic;

namespace EssentialUIKit.Models.Api
{    
    public class QuaddieGroup
    {       
        public int QuaddieGroupId { get; set; }
        public string Description { get; set; }
        public Venue Venue { get; set; }
        public User AdminUser { get; set; }
        public ICollection<SelectedRunner> Selections { get; set; }

    }
}
