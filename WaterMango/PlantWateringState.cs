using WaterMango.Global;

namespace WaterMango
{
    /// <summary>
    /// Plant Watering State Class
    /// </summary>
    public class PlantWateringState
    {
        /// <summary>
        /// Current State of watering for the plant
        /// </summary>
        public WateringState currentState { get; set; } = WateringState.NotWatered;

        /// <summary>
        /// Watering time 10 secs.
        /// </summary>
        private const int WATERING_TIME = 10000;
        
        /// <summary>
        /// Recover Time after watering 30 secs.
        /// </summary>
        private const int RECOVERY_TIME = 30000;

        /// <summary>
        /// Watered Time 6 hours minus recovery time until plant is not watered.
        /// </summary>
        private const int WATERED_TIME = 21570000;

        /// <summary>
        /// Watering Task Cancellation.
        /// </summary>
        private CancellationTokenSource wateringCancellationToken = null;

        /// <summary>
        /// Water Recovery Task Cancellation.
        /// </summary>
        private CancellationTokenSource waterRecoveryCancellationToken = null;

        /// <summary>
        /// Watered Task Cancellation.
        /// </summary>        
        private CancellationTokenSource wateredCancellationToken = null;

        /// <summary>
        /// StartWatering goes through each state and creates delayed tasks to change the state.
        /// </summary>
        public async void StartWatering() {
            switch (currentState)
            {
                //Case Handles moving from NotWatered -> Watered and Watering timer.
                case WateringState.NotWatered:
                    currentState = WateringState.Watering;
                    wateringCancellationToken = new CancellationTokenSource();
                    var t = Task.Run(async delegate {
                        await Task.Delay(WATERING_TIME, wateringCancellationToken.Token);
                        WateredResting();
                    });
                    break;
                //If rewatering happens will reset the timer on watering.
                case WateringState.Watered:
                    wateredCancellationToken.Cancel();
                    RestartWateringTimer();
                    break;
                //Watering doesn't matter or doesn't apply.
                case WateringState.WateredResting:
                case WateringState.Watering:
                    break;
            }
        }

        /// <summary>
        /// Watered Resting handles the state after watering and timer for watered state.  
        /// </summary>
        private void WateredResting()
        {
            this.currentState = WateringState.WateredResting;
            waterRecoveryCancellationToken = new CancellationTokenSource(); 
            var t = Task.Run(async delegate
            {
                await Task.Delay(RECOVERY_TIME, waterRecoveryCancellationToken.Token);
                Watered();
            });            
        }

        /// <summary>
        /// Restart Watering Timer
        /// </summary>
        private void RestartWateringTimer() {
            wateredCancellationToken = new CancellationTokenSource();
            var t = Task.Run(async delegate
            {
                await Task.Delay(WATERED_TIME, wateredCancellationToken.Token);
                NotWatered();
            });
        }

        /// <summary>
        /// Watered and reset Watering Timer.
        /// </summary>
        private void Watered() {
            this.currentState = WateringState.Watered;
            RestartWateringTimer();
        }

        /// <summary>
        /// Not Watered
        /// </summary>
        private void NotWatered() {
            this.currentState = WateringState.NotWatered;               
        }

        /// <summary>
        /// Stop Watering Method and cancel watering timer.
        /// </summary>
        public void StopWatering() {
            if (currentState == WateringState.Watering) {
                wateringCancellationToken.Cancel();
                currentState = WateringState.NotWatered;
            }
        }

    }
}
