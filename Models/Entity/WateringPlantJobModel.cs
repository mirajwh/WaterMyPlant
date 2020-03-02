using System;

namespace WaterMyPlant.Models.Entity
{
    public class WateringPlantJobModel : BaseEntity
    {
        public string HangfireJobId { get; set; }
        public int PlantId { get; set; }
        public int JobStatus { get; set; }
        public DateTime StartTime { get; set; }
        public DateTime FinishTime { get; set; }
        public int ExecutionTime { get; set; }
    }
}