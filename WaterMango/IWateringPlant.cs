using WaterMango.Global;

namespace WaterMango
{
    /// <summary>
    /// Interface for IWateringPlant
    /// </summary>
    public interface IWateringPlant
    {
        void StartWatering();
        void StopWatering();
        WateringState WateringState { get; }
        string Name { get; }
        Guid guid { get; }

    }
}