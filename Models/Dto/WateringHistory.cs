using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace WaterMyPlant.Models.Dto
{
    public class WateringHistory
    {
        public string StartTime { get; set; }
        public string FinishedTime { get; set; }
        public string ProcessingTime { get; set; }
        public string Status { get; set; }
    }
}
