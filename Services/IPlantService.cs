using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using WaterMyPlant.Models.Dto;
using WaterMyPlant.Models.Entity;

namespace WaterMyPlant.Services
{
    public interface IPlantService
    {
        Task<PlantModel> AddPlantAsync(PlantModel plant);
        Task<PlantModel> GetPlantByIdAsync(int id);
        Task<List<PlantModel>> GetAllPlantsAsync();
        Task<ResponseModel> UpdatePlantAsync(PlantModel plant);
        Task<ResponseModel> DeletePlantAsync(int id);
    }
}
