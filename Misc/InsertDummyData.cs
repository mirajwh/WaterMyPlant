using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using WaterMyPlant.Models.Entity;
using WaterMyPlant.Services;

namespace WaterMyPlant.Misc
{
    public static class InsertDummyData
    {
        public static async Task Insert(IPlantService _plantService)
        {
            var plants = await _plantService.GetAllPlantsAsync();
            if(plants.Count == 0)
            {
                foreach (var plant in getDummyPlants())
                {
                    await _plantService.AddPlantAsync(plant);
                }
            }
        }

        public static List<PlantModel> getDummyPlants()
        {
            List<PlantModel> plants = new List<PlantModel>();

            plants.Add(new PlantModel()
            {
                Name = "Succulent",
                Location = "On Andrei's desk",
                ImageUrl = "https://www.linenchest.com/media/catalog/product/cache/1/image/1000x1000/9df78eab33525d08d6e5fb8d27136e95/g/i/ginof_259713_web_cc_1.jpg"
            });
            plants.Add(new PlantModel()
            {
                Name = "Fig Tree",
                Location = "In front of Jaclyn's Desk",
                ImageUrl = "https://i.etsystatic.com/10233645/r/il/b13907/2110591592/il_794xN.2110591592_j11t.jpg"
            });
            plants.Add(new PlantModel()
            {
                Name = "Fiddle Leaf",
                Location = "On Reception desk",
                ImageUrl = "https://mobilia.ca/media/catalog/product/cache/5000b143587f9c7465a6a302957091a0/7/6/766841346_fiddle_leaf-2.jpg",
                LastWateredDateTime = DateTime.Now.AddMinutes(-359).AddSeconds(-30)
            });
            plants.Add(new PlantModel()
            {
                Name = "DaySpring",
                Location = "In kitchen",
                ImageUrl = "https://www.416-flowers.com/image/product/0/0/99/274pd_800x800.jpg"
            });
            plants.Add(new PlantModel()
            {
                Name = "Bamboo",
                Location = "In the entrance lobby",
                ImageUrl = "https://i.etsystatic.com/10233645/r/il/0e71c1/1999966234/il_794xN.1999966234_4a9w.jpg"
            });

            return plants;
        }
    }
}
