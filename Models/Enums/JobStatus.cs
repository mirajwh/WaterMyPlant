using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace WaterMyPlant.Models.Enums
{
    public enum JobStatus
    {
        Enqueued = 1,
        Scheduled = 2,
        Processing = 3,
        Succeeded = 4,
        Failed = 5,
        Deleted = 6,
        Awaiting = 7
    }
}
