using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;


namespace BF_API.Data
{
    public class Runner
    {
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public int RunnerId { get; set; }
        public int BfSelectionId { get; set; }
        public Race Race { get; set; }
        public string Name { get; set; }
        public DateTime DateModified { get; set; }
        public double? LastPriceTraded { get; set; }
        public string JockeyName { get; set; }
        public string TrainerName { get; set; }
        public string Weight { get; set; }
        public string Form { get; set; }
        public string Barrier { get; set; }
    }
}
