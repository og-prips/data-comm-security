//"use strict";

//var connection = new signalR.HubConnectionBuilder().withUrl("https://localhost:7205/temperatureHub").build();

//connection.on("ReceiveTemperatureData", function (temperatureData) {
//    var li = document.createElement("li");
//    document.getElementById("temperatureList").appendChild(li);

//    li.textContent = `${temperatureData.dateSent} - Temperature recieved from device ${temperatureData.deviceId}: ${temperatureData.temperature.toFixed(1)} C | H: ${temperatureData.humidity}`;
//});

//connection.start().catch(function (err) {
//    return console.error(err.toString());
//});

"use strict";

var connection = new signalR.HubConnectionBuilder().withUrl("https://localhost:7205/temperatureHub").build();

// Create an object to store lists for each device
var deviceLists = {};

connection.on("ReceiveTemperatureData", function (temperatureData) {
    // Check if the list for this device exists, and create it if not
    if (!deviceLists.hasOwnProperty(temperatureData.deviceId)) {
        deviceLists[temperatureData.deviceId] = [];
    }

    // Add the received data to the list for this device
    deviceLists[temperatureData.deviceId].push(temperatureData);

    // Create a list element for this device's list
    var ulId = `temperatureList-${temperatureData.deviceId}`;
    var li = document.createElement("li");
    var ul = document.getElementById(ulId);

    if (!ul) {
        // Create a new list element for this device if it doesn't exist
        ul = document.createElement("ul");
        ul.id = ulId;
        document.getElementById("temperatureList").appendChild(ul);
    }

    // Add the temperature data to the device-specific list
    li.textContent = `${temperatureData.dateSent} - Device ${temperatureData.deviceId}: ${temperatureData.temperature.toFixed(1)} C | H: ${temperatureData.humidity}`;
    ul.appendChild(li);
});

connection.start().catch(function (err) {
    return console.error(err.toString());
});
