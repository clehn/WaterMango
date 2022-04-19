using WaterMango.Global;

namespace WaterMango.Controllers
{
    /// <summary>
    /// MVC Watering plant model
    /// </summary>
    public class WateringPlantModel
    {
        public Guid guid { get; set; }
        public string? name { get; set; }
        public WateringState wateringState { get; set; }

    }
}