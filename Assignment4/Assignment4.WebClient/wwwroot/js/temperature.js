"use strict";

var connection = new signalR.HubConnectionBuilder().withUrl("/temperatureHub").build();

connection.on("ReceiveTemperature", function (temperature) {
    var li = document.createElement("li");
    document.getElementById("temperatureList").appendChild(li);

    li.textContent = `Temperature recieved: ${temperature}`;
});

connection.start().catch(function (err) {
    return console.error(err.toString());
});