namespace WaterMango.Factory
{
    /// <summary>
    /// Interface for WateringPlantFactory
    /// </summary>
    public interface IWateringPlantFactory
    {
        /// <summary>
        /// Method for returning created Plants.
        /// </summary>
        /// <returns></returns>
        IEnumerable<IWateringPlant> CreatePlants();
    }
}