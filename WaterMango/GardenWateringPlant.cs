using WaterMango.Global;

namespace WaterMango
{
    /// <summary>
    /// Garden Watering Plant Class
    /// </summary>
    public class GardenWateringPlant : IWateringPlant
    {
        /// <summary>
        /// Static list of Names to generate GardenWateringPlants from.
        /// </summary>
        public static string[] plantNames = { "SunFlower", "Watermelon", "Peas", "Cauliflower", "Onion" };

        /// <summary>
        /// State component class
        /// </summary>
        private PlantWateringState plantWateringState = new PlantWateringState();
        
        /// <summary>
        /// GardenWateringPlant Constructor
        /// </summary>
        /// <param name="guid"></param>
        /// <param name="name"></param>
        public GardenWateringPlant(Guid guid, string name) {
            this.guid = guid;
            this.Name = name;
        }

        /// <summary>
        /// Guid 
        /// </summary>
        public Guid guid { get; set; }
        
        /// <summary>
        /// Plant Name
        /// </summary>
        public string? Name { get; set; }

        /// <summary>
        /// StopWatering calling the state class.
        /// </summary>
        public void StopWatering() => plantWateringState.StopWatering();

        /// <summary>
        /// StartWatering calling the state class.
        /// </summary>
        public void StartWatering() => plantWateringState.StartWatering();

        /// <summary>
        /// Watering state calling the state class.
        /// </summary>
        public WateringState WateringState {
            get {
                return plantWateringState.currentState;
            }
        }
    }
}
