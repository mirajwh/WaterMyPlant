using System;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using WaterMyPlant.Services;
using WaterMyPlant.Models.Dto;
using WaterMyPlant.Models.Enums;

namespace WaterMyPlant.Controllers
{
    [Route("api/[controller]")]
    public class WaterMyPlantController : Controller
    {
        private readonly IWateringService _waterMyPlantService;

        public WaterMyPlantController(IWateringService waterMyPlantService)
        {
            _waterMyPlantService = waterMyPlantService;
        }

        [HttpGet("[action]/{id}")]
        public async Task<IActionResult> StartWateringPlant([FromRoute] int id)
        {
            var result = new ResponseModel();

            try
            {
                result = await _waterMyPlantService.StartWateringPlant(id);

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

        [HttpGet("[action]/{id}")]
        public async Task<IActionResult> StopWateringPlant([FromRoute] int id)
        {
            var result = new ResponseModel();

            try
            {
                result = await _waterMyPlantService.StopWateringPlant(id);

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

        [HttpGet("[action]/{id}")]
        public async Task<IActionResult> GetPlantWateringStatus([FromRoute] int id)
        {
            var result = new ResponseModel();

            try
            {
                var plant = await _waterMyPlantService.GetPlantWateringStatus(id);

                if (plant != null)
                {                     
                    result.Message = Enum.GetName(typeof(WateringStatus), plant.WateringStatus);
                    result.Data = plant;
                    result.IsSuccess = true;
                    return Ok(result);
                }
                else
                {
                    result.Message = "Plant not found";
                    result.Data = null;
                    result.IsSuccess = false;
                    return Ok(result);
                }                
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex);
            }

            result.Message = "";
            result.Data = "";
            result.IsSuccess = false;
            return BadRequest(result);
        }

        [HttpGet("[action]/{id}")]
        public async Task<IActionResult> GetPlantWateringHistory([FromRoute] int id)
        {
            var result = new ResponseModel();

            try
            {
                result = await _waterMyPlantService.GetPlantWateringHistory(id);

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