using Microsoft.AspNetCore.Mvc;
using WaterMango.Factory;

namespace WaterMango.Controllers
{
    /// <summary>
    /// Plant Controller is the primary controller for this call.
    /// </summary>
    [ApiController]
    [Route("[controller]")]
    public class PlantController : ControllerBase
    {
        /// <summary>
        /// Watering Plant Factory is the reference to the watering plant builder.
        /// </summary>
        private readonly IWateringPlantFactory wateringPlantFactory;

        /// <summary>
        /// Watering Plants is the static storage location of the watering plants in memory.
        /// </summary>
        private static IEnumerable<IWateringPlant> wateringPlants;

        /// <summary>
        /// Plant Controller Constructor take the Dependency Injected factory
        /// to create the plants needed.
        /// </summary>
        /// <param name="wateringPlantFactory"></param>
        public PlantController(IWateringPlantFactory wateringPlantFactory)
        {
            if (wateringPlantFactory == null)
            {
                wateringPlantFactory = wateringPlantFactory;
            }
            if (wateringPlants == null)
            {
                wateringPlants = wateringPlantFactory.CreatePlants().ToArray();
            }
        }

        /// <summary>
        /// Get All the plants
        /// </summary>
        /// <returns></returns>
        [HttpGet]
        [ActionName("GetAll")]
        [Route("GetAll")]
        public IEnumerable<WateringPlantModel> GetAll()
        {         
            return wateringPlants.ToArray()
                .Select( x => 
                    new WateringPlantModel { 
                        guid = x.guid, 
                        name = x.Name, 
                        wateringState = x.WateringState 
                    });
        }

        /// <summary>
        /// Get plant by ID
        /// </summary>
        /// <param name="guid"></param>
        /// <returns></returns>
        [HttpGet]
        [ActionName("GetByID")]
        [Route("GetByID/{guid}")]
        public WateringPlantModel GetByID(Guid guid)
        {
            var plant = FindWateringPlant(guid);
            return 
                   new WateringPlantModel()
                   {
                       guid = plant.guid,
                       name = plant.Name,
                       wateringState = plant.WateringState
                   };
        }

        /// <summary>
        /// Water a plant by ID
        /// </summary>
        /// <param name="guid"></param>
        [HttpGet]
        [ActionName("WateringPlantByID")]
        [Route("WateringPlantByID/{guid}")]
        public void WateringPlantByID(Guid guid) {
            var plant = FindWateringPlant(guid);
            plant.StartWatering();
        }

        /// <summary>
        /// Stop Watering a Plant by ID
        /// </summary>
        /// <param name="guid"></param>
        [HttpGet]
        [ActionName("StopWateringPlantByID")]
        [Route("StopWateringPlantByID/{guid}")]
        public void StopWateringPlantByID(Guid guid)
        {
            var plant = FindWateringPlant(guid);
            plant.StopWatering();
        }

        /// <summary>
        /// Find Watering Plant Helper
        /// </summary>
        /// <param name="guid"></param>
        /// <returns></returns>
        private IWateringPlant FindWateringPlant(Guid guid) {
            return wateringPlants.ToArray().Where(x => x.guid == guid).First();
        }
        


    }
}