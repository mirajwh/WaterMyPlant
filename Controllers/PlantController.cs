using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using WaterMyPlant.Models.Dto;
using WaterMyPlant.Models.Entity;
using WaterMyPlant.Services;

namespace WaterMyPlant.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class PlantController : ControllerBase
    {
        private readonly IPlantService _plantService;        

        public PlantController(IPlantService plantService, ILogger<PlantController> logger)
        {
            _plantService = plantService;

        }

        [HttpGet("[action]")]
        public async Task<IActionResult> GetListOfPlants()
        {
            var result = new ResponseModel();
            try
            {
                var plants = await _plantService.GetAllPlantsAsync();
                result.Data = (plants.Count > 0) ? plants : null;
                result.Message = (result.Data.Count > 0) ? $"Retrieved {result.Data.Count} Plants." : "No plants to show";
                result.IsSuccess = true;                
                return Ok(result);                
            }
            catch (Exception)
            {                
                result.Message = AppConsts.InternalServerError;
                result.IsSuccess = false;
            }            
            return BadRequest(result);
        }

        [HttpGet("[action]/{id}")]
        public async Task<IActionResult> GetPlantById([FromRoute] int id)
        {
            var result = new ResponseModel();

            try
            {
                var plant = await _plantService.GetPlantByIdAsync(id);
                if(plant != null)
                {
                    result.Data = plant;
                    result.Message = $"Plant found.";
                    result.IsSuccess = true;
                    return Ok(result);
                }
                else
                {
                    result.Data = plant;
                    result.Message = $"No plant found";
                    result.IsSuccess = false;
                    return Ok(result);
                }
            }
            catch (Exception)
            {
                result.Message = AppConsts.InternalServerError;
                result.IsSuccess = false;
            }
            return BadRequest(result);
        }        

        [HttpPost("[action]")]
        public async Task<IActionResult> AddPlant([FromBody] PlantModel plant)
        {
            var result = new ResponseModel();

            try
            {
                var plants = await _plantService.AddPlantAsync(plant);
                if ((plants.Id > 0))
                {
                    result.Data = plants;
                    result.Message = $"Plant has been added";
                    result.IsSuccess = true;
                    return Ok(result);
                }
                else
                {
                    result.Data = plants;
                    result.Message = $"Something went wrong. Please try again.";
                    result.IsSuccess = false;
                    return BadRequest(result);
                }                
            }
            catch (Exception)
            {
                result.Message = AppConsts.InternalServerError;
                result.IsSuccess = false;
            }
            return BadRequest(result);
        }

        [HttpPut("[action]")]
        public async Task<IActionResult> UpdatePlant([FromBody] PlantModel plant)
        {
            var result = new ResponseModel();

            try
            {
                result = await _plantService.UpdatePlantAsync(plant);

                if (result.IsSuccess)
                {
                    return Ok(result);
                }
            }
            catch (Exception)
            {
                result.Message = AppConsts.InternalServerError;
                result.IsSuccess = false;
            }
            return BadRequest(result);
        }

        [HttpDelete("[action]/{id}")]
        public async Task<IActionResult> DeletePlant([FromRoute] int id)
        {
            var result = new ResponseModel();

            try
            {
                result = await _plantService.DeletePlantAsync(id);

                if (result.IsSuccess)
                {
                    return Ok(result);
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex);
            }
            return BadRequest(result);
        }
    }
}