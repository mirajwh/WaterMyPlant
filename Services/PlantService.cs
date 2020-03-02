using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Hangfire;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using WaterMyPlant.Data.DatabseContext;
using WaterMyPlant.Models.Dto;
using WaterMyPlant.Models.Entity;
using WaterMyPlant.Models.Enums;

namespace WaterMyPlant.Services
{
    public class PlantService : IPlantService
    {
        private readonly WaterMyPlantDbContext _waterMyPlantDbContext;
        private readonly INotificationService _notificationService;
        private readonly ILogger<PlantService> _logger;

        public PlantService(WaterMyPlantDbContext waterMyPlantDbContext,
            ILogger<PlantService> logger,
            INotificationService notificationService)
        {
            _waterMyPlantDbContext = waterMyPlantDbContext;
            _logger = logger;
            _notificationService = notificationService;
        }

        public async Task<PlantModel> AddPlantAsync(PlantModel plant)
        {
            try
            {
                FormatPlant(plant);

                await _waterMyPlantDbContext.Plants.AddAsync(plant);
                await _waterMyPlantDbContext.SaveChangesAsync();

                await SetThirstyPlantAlert(plant);

                return plant;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, ex.Message);
                throw ex;
            }
        }

        private void FormatPlant(PlantModel plant)
        {
            TimeZoneInfo infotime = TimeZoneInfo.FindSystemTimeZoneById("Eastern Standard Time");
            plant.LastWateredDateTime = plant.LastWateredDateTime.Kind == DateTimeKind.Utc ? 
                TimeZoneInfo.ConvertTimeFromUtc(plant.LastWateredDateTime, infotime)
                : plant.LastWateredDateTime;
        }

        public async Task<PlantModel> GetPlantByIdAsync(int id)
        {
            try
            {
                var plant = await (from p in _waterMyPlantDbContext.Plants
                                   where p.Id == id
                                   select p).FirstOrDefaultAsync();
                return plant;

            }
            catch (Exception ex)
            {
                _logger.LogError(ex, ex.Message);
                throw ex;
            }
        }

        public async Task<List<PlantModel>> GetAllPlantsAsync()
        {
            var result = new List<PlantModel>();
            try
            {
                result = await _waterMyPlantDbContext.Plants.ToListAsync();
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, ex.Message);
                throw ex;
            }
            return result;
        }

        public async Task<ResponseModel> UpdatePlantAsync(PlantModel plantUpdates)
        {
            ResponseModel responseModel = new ResponseModel();

            try
            {
                FormatPlant(plantUpdates);
                var plant = await _waterMyPlantDbContext.Plants.FindAsync(plantUpdates.Id);
                var old_LastWateredDateTime = plant.LastWateredDateTime;
                if (plant != null)
                {
                    plant.Name = plantUpdates.Name;
                    plant.ImageUrl = plantUpdates.ImageUrl;
                    plant.Id = plantUpdates.Id;
                    plant.Location = plantUpdates.Location;
                    plant.WateringStatus = plantUpdates.WateringStatus;
                    plant.LastWateredDateTime = plantUpdates.LastWateredDateTime;
                    plant.RestingTimeInSec = plantUpdates.RestingTimeInSec;
                    plant.WateringTimeInSec = plantUpdates.WateringTimeInSec;
                    plant.CanStayWithoutWaterInMin = plantUpdates.CanStayWithoutWaterInMin;

                    _waterMyPlantDbContext.Plants.Update(plant);
                    await _waterMyPlantDbContext.SaveChangesAsync();

                    await SetThirstyPlantAlert(plant);


                    responseModel.Message = $"Plant with Id {plantUpdates.Id} was updated.";
                    responseModel.IsSuccess = true;
                    responseModel.Data = null;

                    return responseModel;
                }

                responseModel.Message = $"Plant with Id {plantUpdates.Id} could not be updated.";
                responseModel.IsSuccess = false;
                responseModel.Data = null;

                return responseModel;

            }
            catch (Exception ex)
            {
                _logger.LogError(ex, ex.Message);
                throw ex;
            }
        }

        public async Task<ResponseModel> DeletePlantAsync(int id)
        {
            ResponseModel responseModel = new ResponseModel();

            try
            {
                var result = await _waterMyPlantDbContext.Plants.FindAsync(id);

                if (result != null)
                {
                    _waterMyPlantDbContext.Plants.Remove(result);
                    await _waterMyPlantDbContext.SaveChangesAsync();
                    responseModel.Message = $"Plant with Id {id} was deleted.";
                    responseModel.IsSuccess = true;
                    responseModel.Data = null;
                    return responseModel;
                }

                responseModel.Message = $"Plant with Id {id} could not be deleted.";
                responseModel.IsSuccess = false;
                responseModel.Data = null;
                return responseModel;

            }
            catch (Exception ex)
            {
                responseModel.Message = $"Error => {ex.Message}";
                responseModel.IsSuccess = false;
                responseModel.Data = null;
                return responseModel;
            }
        }

        public async Task<string> SetThirstyPlantAlert(PlantModel plant)
        {
            var jobId = BackgroundJob.Schedule(() => TriggerThirstyNotification(plant),
                plant.LastWateredDateTime.AddMinutes(plant.CanStayWithoutWaterInMin));

            var thirstyPlantAlert = await (from TPAs in _waterMyPlantDbContext.ThirstyPlantAlerts
                                           where TPAs.PlantID == plant.Id
                                           select TPAs).FirstOrDefaultAsync();

            if (thirstyPlantAlert != null)
            {
                BackgroundJob.Delete(thirstyPlantAlert.AlertJobId); //deleting old scheduled alert
                thirstyPlantAlert.AlertJobId = jobId;
            }
            else
            {
                thirstyPlantAlert = new ThirstyPlantAlertModel()
                {
                    PlantID = plant.Id,
                    AlertJobId = jobId
                };
            }

            _waterMyPlantDbContext.ThirstyPlantAlerts.Update(thirstyPlantAlert);
            await _waterMyPlantDbContext.SaveChangesAsync();

            return jobId;
        }

        public void TriggerThirstyNotification(PlantModel plant)
        {
            _notificationService.SendThristyAlert(plant.Id);

            //logic to remind every one minute untill watered.
            plant.LastWateredDateTime = plant.LastWateredDateTime.AddMinutes(1);
            SetThirstyPlantAlert(plant).GetAwaiter().GetResult();
        }
    }
}
