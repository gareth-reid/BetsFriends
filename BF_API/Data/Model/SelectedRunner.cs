using System;
using System.ComponentModel.DataAnnotations.Schema;

namespace BF_API.Data.Model
{
    public class SelectedRunner
    {
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public int SelectedRunnerId { get; set; }
        public Runner Runner { get; set; }
        public User User { get; set; }
        public DateTime? DateSelected { get; set; }
    }
}
