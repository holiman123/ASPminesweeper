"use strict";

var connection = new signalR.HubConnectionBuilder().withUrl("/game").build();

connection.on("OpenOneTile", function (x, y) {
    document.getElementById("mainText").style.color = "#" + Math.floor(Math.random() * 16777215).toString(16);
});

connection.start();

document.getElementById("sendMove").addEventListener("click", function (event) {
    connection.invoke("SendMove", 10, 10, false);
});