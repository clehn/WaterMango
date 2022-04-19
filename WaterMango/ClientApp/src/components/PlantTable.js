import React, { useState, Component } from 'react';

/**
 * Watering States that plants take. 
 */
export const WATERING_STATES = {
    NotWatered: 0,
    Watering: 1,
    WateredResting: 2,
    Watered: 3
}

/**
 *  Watering times, after each state.
 *  Note: Buffer is a little extra time, for the Server states to complete.
 */
export const WATERING_TIMES = {
    NotWatered: 0,
    Watering: 10000,
    WateredResting: 30000,
    Watered: 21600000,
    WaterBuffer: 500
}

/**
 * The primary react table with client state allowing you to work with plants.
 */
export class PlantTable extends Component {
    /*Display Name for the component */
    static displayName = PlantTable.name;
    /*Refresh timers for the client to grab data from the server */
    timers = [];

    /*Constructor used to build the component with state for the table */
    constructor(props) {
        super(props);
        this.state = { plants: [], loading: true };
    }

    /*Component launched load the plant data from server */
    componentDidMount() {
        this.populatePlantData();
    }

    /* Component Rendering */
    render() {
        let contents = this.state.loading
            ? <p><em>Loading...</em></p>
            : this.renderPlantsTable(this.state.plants);

        return (
            <div>
                <h1 id="tabelLabel" >Plant status</h1>
                <p>This component demonstrates fetching plant data from the server.</p>
                {contents}
            </div>
        );
    }

    /* Alert div for when a plant needs to be watered */
    WateringAlert(wateringState) {
    const isNotWatered = wateringState == WATERING_STATES.NotWatered;
        return (
            <div className="alert">
            {isNotWatered ?  'Alert - The plant needs watering!':'' }
        </div>
    );

    }

    /* Start Watering the plant, with Guid and watering state.
     * Method creates a number of timers, to refresh the data when required. 
     **/
    async StartWatering(guid,wateringState) {
        await fetch(`Plant/WateringPlantByID/${guid}`)
        let intervalWateringTime = 0;
        let intervalWateredRestingTime = 0;
        let intervalWateredTime = 0;
        
        switch (wateringState) {
            case WATERING_STATES.NotWatered:
                //Grab the Watering state right away as it was called above.
                this.GetSinglePlantUpdate(guid);

                //Create refresh times for all the other states.
                intervalWateringTime = WATERING_TIMES.Watering + WATERING_TIMES.WaterBuffer;
                intervalWateredRestingTime = WATERING_TIMES.Watering + WATERING_TIMES.WateredResting + WATERING_TIMES.WaterBuffer;
                intervalWateredTime = WATERING_TIMES.Watering + WATERING_TIMES.Watered + WATERING_TIMES.WaterBuffer;

                //Create Timers for when to refresh the record when watering a plant, 
                //and remove it from timer list once complete.
                const wateringTimer = setTimeout(() => {
                    this.GetSinglePlantUpdate(guid);
                    this.RemoveTimer(guid, WATERING_STATES.Watering);
                    
                }, intervalWateringTime);
                let newWatering = { guid: guid, wateringState: WATERING_STATES.Watering, timerobj: wateringTimer };

                const waterRestingTimer = setTimeout(() => {
                    this.GetSinglePlantUpdate(guid);
                    this.RemoveTimer(guid, WATERING_STATES.WateredResting);
                }, intervalWateredRestingTime);
                let newWaterResting = { guid: guid, wateringState: WATERING_STATES.WateredResting, timerobj: waterRestingTimer };

                const wateredTimer = setTimeout(() => {
                    this.GetSinglePlantUpdate(guid);
                    this.RemoveTimer(guid, WATERING_STATES.Watered);
                }, intervalWateredTime);
                let newWatered = { guid: guid, wateringState: WATERING_STATES.Watered, timerobj: wateredTimer };

                //Push the timers, to the array, needed for StopWatering.
                this.timers.push(newWatering);
                this.timers.push(newWaterResting);
                this.timers.push(newWatered);

                break;
            case WATERING_STATES.Watering:
            case WATERING_STATES.WateredResting:
                // DN can't water in these states.
                break;
            case WATERING_STATES.Watered:
                //Remove an existing timer as time will be extended with watering in watered state.
                this.RemoveTimer(guid, wateringState);
                intervalWateredTime = WATERING_TIMES.Watered + WATERING_TIMES.WaterBuffer;

                //Create the new timer, for a refresh of the record when the plant needs watering.
                const wateredTimer2 = setTimeout(() => {
                    this.GetSinglePlantUpdate(guid);
                }, intervalWateredTime);
                let newWatered2 = { guid: guid, wateringState: WATERING_STATES.Watered, timerobj: wateredTimer2 };

                this.timers.push(newWatered2);

                break;
            default:
                break;
        }
        
    }

    /*
     * Remove Timer, removes refresh timers from the list based on the guid and wateringState.
     */
    RemoveTimer(guid, wateringState) {
        const index = this.timers.findIndex((c) => c.guid + c.wateringState === guid + wateringState);
        clearInterval(this.timers[index].timerobj);
        this.timers.splice(index, 1);

    }

    /*
     * Update the state with a single record, when requried.
     */
    async GetSinglePlantUpdate(guid) {
        const getrecord = await fetch(`Plant/GetByID/${guid}`);
        const plantrecord = await getrecord.json();
        const index = this.state.plants.findIndex((c) => c.guid === plantrecord.guid);
        let plants = [...this.state.plants];
        let plant = { ...plants[index] };
        plant.wateringState = plantrecord.wateringState;
        plants[index] = plant;
        this.setState({ plants });
    }

    /*
     * Stop Watering changes the state, and removes all the refresh timers from Watering.
     */
    async StopWatering(guid, wateringState) {
        await fetch(`Plant/StopWateringPlantByID/${guid}`)
        if (wateringState == WATERING_STATES.Watering) {
            this.GetSinglePlantUpdate(guid);
            this.RemoveTimer(guid, WATERING_STATES.Watering);
            this.RemoveTimer(guid, WATERING_STATES.WateredResting);
            this.RemoveTimer(guid, WATERING_STATES.Watered);
        }
    }

    /*
     * Start Watering Button returns the button needed for watering, and creates it enabled or disabled.
     */
    StartWateringButton(wateringState, guid) {
        const isDisabled = (wateringState == WATERING_STATES.WateredResting || wateringState == WATERING_STATES.Watering);
        return (
            <button type="button" onClick={() => this.StartWatering(guid,wateringState)} aria-disabled={String(isDisabled)} disabled={isDisabled} aria-aria-describedby="disabledReasonStartWatering">Start Watering</button>
        );
    }

    /*
     * Stop Watering Button returns the button needed for stop watering, and creates it enabled or disabled.
     */
    StopWateringButton(wateringState, guid) {
        const isDisabled = !(wateringState == WATERING_STATES.Watering);
         return (
             <button type="button" onClick={() => this.StopWatering(guid, wateringState)} aria-disabled={String(isDisabled)} disabled={isDisabled} aria-aria-describedby="disabledReasonStopWatering">Stop Watering</button>
        );
    }

    /*
     * Map the Enum values from the server to text for the watering state column.
     */
    WateringStateName(wateringState) {
        switch (wateringState) {
            case 0:
                return 'NotWatered';
            case 1:
                return 'Watering';
            case 2:
                return 'WateredResting';
            case 3:
                return 'Watered';
            default:
                return '';
        }
    }

    /*
     * RenderPlants Table maps the state data into the table.
     */
    renderPlantsTable(plants) {
    return (
        <div className="Plant-Table">
            <table className="greenTable">
                <thead>
                    <th>ID</th>
                    <th>Plant Name</th>
                    <th>Plant State</th>
                    <th>Actions</th>                    
                    <th>Notification</th>
                </thead>
                <tbody>                    
                    {plants.map((plant) => (                    
                        <tr>
                            <td>{plant.guid }</td>
                            <td>{plant.name}</td>
                            <td>{this.WateringStateName(plant.wateringState)}
                            </td>
                            <td>
                                {this.StartWateringButton(plant.wateringState, plant.guid)}
                                {this.StopWateringButton(plant.wateringState, plant.guid)}
                            </td>
                            <td>{this.WateringAlert(plant.wateringState) }</td>
                        </tr>
                    ))}
                </tbody>
            </table>
        </div>
    );
    }

    /*
     * Populate Plant Data does the first time load from the server.
     */
    async populatePlantData() {
        const response = await fetch('Plant/GetAll');
        const data = await response.json();
        this.setState({ plants: data, loading: false });
    }
    }

export default PlantTable;
