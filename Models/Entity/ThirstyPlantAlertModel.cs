using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace WaterMyPlant.Models.Entity
{
    public class ThirstyPlantAlertModel : BaseEntity
    {
        public int PlantID { get; set; }
        public string AlertJobId { get; set; }
    }
}
