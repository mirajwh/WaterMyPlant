using System;
using System.Threading.Tasks;
using WaterMyPlant.Models.Dto;
using WaterMyPlant.Models.Entity;

namespace WaterMyPlant.Services
{
    public interface IWateringService
    {
        Task<ResponseModel> StartWateringPlant(int plantId);
        Task<ResponseModel> StopWateringPlant(int plantId);
        Task<PlantModel> GetPlantWateringStatus(int plantId);
        Task<ResponseModel> GetPlantWateringHistory(int plantId);        
    }
}