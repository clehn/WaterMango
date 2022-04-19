namespace WaterMango.Factory
{
    /// <summary>
    /// Garden Watering Plant Factory
    /// </summary>
    public class GardenWateringPlantFactory : IWateringPlantFactory
    {
        /// <summary>
        /// GardenWateringPlantFactory
        /// </summary>
        public GardenWateringPlantFactory() { 
        }


        /// <summary>
        /// Create Plants from Factory
        /// </summary>
        /// <returns></returns>
        public IEnumerable<IWateringPlant> CreatePlants()
        {
            return GardenWateringPlant.plantNames.Select(x => new GardenWateringPlant(Guid.NewGuid(), x));                                
        }


    }
}
