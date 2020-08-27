using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using BF_API.Data.Model;
using Microsoft.EntityFrameworkCore;

namespace BF_API.Data
{    
    public class QuaddieGroup
    {       
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public int QuaddieGroupId { get; set; }
        public string Description { get; set; }
        public Venue Venue { get; set; }
        
        public ICollection<SelectedRunner> Selections { get; set; }

    }
}
