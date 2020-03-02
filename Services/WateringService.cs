using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Hangfire;
using Microsoft.EntityFrameworkCore;
using WaterMyPlant.Services;
using WaterMyPlant.Models.Dto;
using WaterMyPlant.Data.DatabseContext;
using WaterMyPlant.Models.Entity;
using System.Linq;
using WaterMyPlant.Models.Enums;
using System.Threading;
using Microsoft.Extensions.Logging;

namespace WaterMyPlant.Services
{
    public class WateringService : IWateringService
    {
        private readonly WaterMyPlantDbContext _waterMyPlantDbContext;
        private readonly IPlantService _plantService;
        private readonly INotificationService _notificationService;
        private readonly ILogger<WateringService> _logger;

        public WateringService(WaterMyPlantDbContext waterMyPlantDbContext,
            ILogger<WateringService> logger,
            IPlantService plantService,
            INotificationService notificationService)
        {
            _waterMyPlantDbContext = waterMyPlantDbContext;
            _plantService = plantService;
            _logger = logger;
            _notificationService = notificationService;
        }

        public async Task<ResponseModel> StartWateringPlant(int plantId)
        {
            try
            {
                var response = new ResponseModel();

                var plant = await _plantService.GetPlantByIdAsync(plantId);
                if (plant != null)
                {
                    response.Data = plant;

                    if (ValidateStartingRequest(plant, response))
                    {
                        var jobId = BackgroundJob.Enqueue(() => ProcessWatering(plant));

                        var wateringplantjob = new WateringPlantJobModel()
                        {
                            JobStatus = (int)JobStatus.Processing,
                            HangfireJobId = jobId,
                            PlantId = plant.Id,
                            StartTime = DateTime.Now
                        };
                        await _waterMyPlantDbContext.WateringPlantJobs.AddAsync(wateringplantjob);
                        await _waterMyPlantDbContext.SaveChangesAsync();

                        plant.WateringStatus = (int)WateringStatus.Watering;
                        plant.LastWateredDateTime = wateringplantjob.StartTime;
                        await _plantService.UpdatePlantAsync(plant);

                        response.Message = $"Watering started";
                        response.IsSuccess = true;
                    }
                }
                else
                {
                    response.Data = plant;
                    response.Message = $"No plant found";
                    response.IsSuccess = false;
                }
                return response;

            }
            catch (Exception ex)
            {
                _logger.LogError(ex, ex.Message);
                throw ex;
            }
        }

        private bool ValidateStartingRequest(PlantModel plant, ResponseModel response)
        {
            if (!plant.WateringStatus.Equals((int)WateringStatus.Watering))
            {
                if (DateTime.Compare(plant.LastWateredDateTime.AddSeconds(plant.RestingTimeInSec),
                    DateTime.Now) <= 0)
                {
                    return true;
                }
                else
                {
                    response.Message = $"Cannot water this plant within {plant.RestingTimeInSec} seconnds of last watering session";
                    response.IsSuccess = false;
                    return false;
                }
            }
            else
            {
                response.Message = $"Already watering this plant.";
                response.IsSuccess = false;
                return false;
            }
        }

        public void ProcessWatering(PlantModel plant)
        {
            try
            {
                plant.WateringStatus = (int)WateringStatus.Watering;
                new Thread(_notificationService.SendWateringStartedStatus).Start(plant);

                Thread.Sleep(TimeSpan.FromSeconds(plant.WateringTimeInSec));


                var wateringplantjob = (from wpj in _waterMyPlantDbContext.WateringPlantJobs
                                        where wpj.PlantId == plant.Id
                                        && (wpj.JobStatus == (int)JobStatus.Processing || wpj.JobStatus == (int)JobStatus.Deleted)
                                        select wpj).OrderByDescending(wpj => wpj.StartTime)
                                        .FirstOrDefaultAsync().GetAwaiter().GetResult();

                if (wateringplantjob != null)
                {
                    if (wateringplantjob.JobStatus.Equals((int)JobStatus.Processing))
                    {
                        wateringplantjob.JobStatus = (int)JobStatus.Succeeded;
                        wateringplantjob.FinishTime = DateTime.Now;
                        wateringplantjob.ExecutionTime = (int)(wateringplantjob.FinishTime - wateringplantjob.StartTime).TotalSeconds;
                        _waterMyPlantDbContext.SaveChanges();

                        plant.WateringStatus = (int)WateringStatus.Watered;
                        plant.LastWateredDateTime = wateringplantjob.FinishTime;
                        _plantService.UpdatePlantAsync(plant).GetAwaiter().GetResult();

                        new Thread(_notificationService.SendWateredStatus).Start(plant);
                    }
                }
                else
                {
                    plant.WateringStatus = (int)WateringStatus.Failed;
                    _plantService.UpdatePlantAsync(plant).GetAwaiter().GetResult();
                    new Thread(_notificationService.SendWateringFailedStatus).Start(plant);
                }

            }
            catch (Exception ex)
            {
                _logger.LogError(ex, ex.Message);
                throw ex;
            }
        }

        public async Task<ResponseModel> StopWateringPlant(int plantId)
        {
            try
            {
                var response = new ResponseModel();

                var plant = await _plantService.GetPlantByIdAsync(plantId);
                if (plant != null)
                {
                    response.Data = plant;

                    if (plant.WateringStatus.Equals((int)WateringStatus.Watering))
                    {

                        var wateringplantjob = (from wpj in _waterMyPlantDbContext.WateringPlantJobs
                                                where wpj.PlantId == plant.Id && wpj.JobStatus == (int)JobStatus.Processing
                                                select wpj).FirstOrDefault();

                        if (wateringplantjob != null)
                        {
                            BackgroundJob.Delete(wateringplantjob.HangfireJobId);

                            new Thread(_notificationService.SendWateringStopStatus).Start(plant);

                            wateringplantjob.JobStatus = (int)JobStatus.Deleted;
                            wateringplantjob.FinishTime = DateTime.Now;
                            wateringplantjob.ExecutionTime = (int)(wateringplantjob.FinishTime - wateringplantjob.StartTime).TotalSeconds;
                            _waterMyPlantDbContext.SaveChanges();

                            plant.WateringStatus = (int)WateringStatus.Stopped;
                            plant.LastWateredDateTime = wateringplantjob.FinishTime;

                            response.Message = "Watering stopped successfully";
                            response.IsSuccess = true;

                        }
                        else
                        {
                            plant.WateringStatus = (int)WateringStatus.Stopped;
                            response.Message = "Something went wrong. Couldn't stop watering.";
                            response.IsSuccess = false;
                        }

                        _plantService.UpdatePlantAsync(plant).GetAwaiter().GetResult();
                    }
                    else
                    {
                        response.Message = "Plant is not watering.";
                        response.IsSuccess = false;
                    }
                }
                else
                {
                    response.Data = plant;
                    response.Message = $"No plant found.";
                    response.IsSuccess = false;
                }
                return response;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, ex.Message);
                throw ex;
            }
        }

        public async Task<ResponseModel> GetPlantWateringHistory(int plantId)
        {
            try
            {
                var wateringplantjob = await (from wpj in _waterMyPlantDbContext.WateringPlantJobs
                                              where wpj.PlantId == plantId
                                              select new WateringHistory()
                                              {
                                                  StartTime = wpj.StartTime.ToString(),
                                                  FinishedTime = wpj.FinishTime.ToString(),
                                                  ProcessingTime = wpj.ExecutionTime.ToString() + " Sec(s)",
                                                  Status = wpj.JobStatus.Equals((int)JobStatus.Processing) ? "Watering" :
                                                            wpj.JobStatus.Equals((int)JobStatus.Succeeded) ? "Succeeded" :
                                                            wpj.JobStatus.Equals((int)JobStatus.Deleted) ? "Stopped" :
                                                            wpj.JobStatus.Equals((int)JobStatus.Failed) ? "Failed" : "Unknown"
                                              }).ToListAsync();
                if (wateringplantjob.Count > 0)
                {
                    return new ResponseModel()
                    {
                        Data = wateringplantjob.OrderByDescending(wj => wj.StartTime),
                        IsSuccess = true,
                        Message = $"{wateringplantjob.Count} records found."
                    };
                }
                else
                {
                    return new ResponseModel()
                    {
                        Data = null,
                        IsSuccess = false,
                        Message = $"No watering history found for this plant."
                    };
                }
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, ex.Message);
                throw ex;
            }
        }

        public async Task<PlantModel> GetPlantWateringStatus(int plantId)
        {
            try
            {
                return await _plantService.GetPlantByIdAsync(plantId);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, ex.Message);
                throw ex;
            }
        }

    }
}