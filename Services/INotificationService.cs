using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using WaterMyPlant.Models.Entity;

namespace WaterMyPlant.Services
{
    public interface INotificationService
    {
        void SendThristyAlert(int plantId);
        void SendWateringStartedStatus(object obj);
        void SendWateredStatus(object obj);
        void SendWateringStopStatus(object obj);
        void SendWateringFailedStatus(object obj);
    }
}
