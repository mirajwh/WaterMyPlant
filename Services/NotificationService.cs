using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Hangfire;
using Microsoft.AspNetCore.SignalR;
using Microsoft.Extensions.Logging;
using WaterMyPlant.Models.Entity;

namespace WaterMyPlant.Services
{
    public class NotificationService : INotificationService
    {
        private readonly IHubContext<PlantHub> _hubContext;
        private readonly ILogger<NotificationService> _logger;

        public NotificationService(IHubContext<PlantHub> hubContext, ILogger<NotificationService> logger)
        {
            _hubContext = hubContext;
            _logger = logger;
        }


        public void SendThristyAlert(int plantId)
        {
            _hubContext.Clients.All.SendAsync("SendThristyAlert", "Attention Needed", plantId);
            _logger.LogInformation($"Watering Alert for Plant Id : {plantId}.");
        }

        public void SendWateredStatus(object obj)
        {
            var plant = (PlantModel)obj;
            _hubContext.Clients.All.SendAsync("SendWateredStatus", "Plant Watered", plant);
            _logger.LogInformation($"Watering Stopped for Plant Id : {plant.Id}.");
            BackgroundJob.Schedule(() => RestingFinish(plant),
                plant.LastWateredDateTime.AddSeconds(plant.RestingTimeInSec));
        }

        public void SendWateringStartedStatus(object obj)
        {
            var plant = (PlantModel)obj;
            _hubContext.Clients.All.SendAsync("SendWateringStartedStatus", "Plant Watering", plant);
            _logger.LogInformation($"Watering Started for Plant Id : {plant.Id}.");
        }

        public void SendWateringStopStatus(object obj)
        {
            var plant = (PlantModel)obj;
            _hubContext.Clients.All.SendAsync("SendWateringStopStatus", "Plant Watering Stopped", plant);
            _logger.LogInformation($"Watering Stopped for Plant Id : {plant.Id}.");
            BackgroundJob.Schedule(() => RestingFinish(plant),
                plant.LastWateredDateTime.AddSeconds(plant.RestingTimeInSec));
        }

        public void SendWateringFailedStatus(object obj)
        {
            var plant = (PlantModel)obj;
            _hubContext.Clients.All.SendAsync("SendWateringFailedStatus", "Plant Watering Failed", plant);
            _logger.LogInformation($"Watering Failed for Plant Id : {plant.Id}.");
            BackgroundJob.Schedule(() => RestingFinish(plant),
                plant.LastWateredDateTime.AddSeconds(plant.RestingTimeInSec));
        }

        public void RestingFinish(PlantModel plant)
        {
            _hubContext.Clients.All.SendAsync("RestingFinish", "Resting finished.", plant);
        }
    }
}
