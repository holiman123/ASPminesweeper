//"use strict";

var connection = new signalR.HubConnectionBuilder().withUrl("/game").build();
var bodyEl = document.getElementById("bodyId");
var canDig = true;

connection.on("OpenField", function (x, y) {
    console.log("X: " + x);
    console.log("Y: " + y);
    var color = false;
    let fieldDiv = document.getElementById("field");
    fieldDiv.innerHTML = "<div id=\"div2\"></div>";
    //fieldDiv.remove();
    //fieldDiv = document.createElement("div");
    //fieldDiv.id = "field";
    //bodyEl.append(fieldDiv);
    fieldDiv.style.width = x * 1.5 + "rem";
    fieldDiv.style.height = y * 1.5 + "rem";

    for (var i = 0; i < y; i++) {
        for (var j = 0; j < x; j++) {
            var tempCell = document.createElement("div");
            if (color) {
                tempCell.className = "closedCell1";
                color = false;
            }
            else {
                tempCell.className = "closedCell0";
                color = true;
            }

            //tempCell.className = "cell";
            tempCell.id = j + ";" + i;
            tempCell.dataset.X = j;
            tempCell.dataset.Y = i;
            tempCell.dataset.color = color;
            tempCell.append(document.createElement("div"));

            tempCell.addEventListener("click", function (event) {
                if (canDig) {
                    if (event.ctrlKey || document.getElementById("flagCheckID").checked)
                        connection.invoke("flagCell", this.id);
                    else
                        connection.invoke("digCell", this.id);
                }
            });
            fieldDiv.append(tempCell);
        }
        if (x % 2 === 0)
            color = !color;
    }
});

connection.on("OpenCell", function (x, y, state, flagsLeft) {
    let tempCell = document.getElementById(x + ";" + y);
    document.getElementById("flagsCountDiv").innerHTML = "Flags left: " + flagsLeft;

    switch (state) {
        case -5:
            tempCell.className = "detonatedCell";
            break;
        case -4:
        case -3:
            if (tempCell.dataset.color === 'true')
                tempCell.className = "flagCell0";
            else
                tempCell.className = "flagCell1";
            break;
        case -2:
        case -1:
            if (tempCell.dataset.color === 'true')
                tempCell.className = "closedCell0";
            else
                tempCell.className = "closedCell1";
            break;
        default:
            tempCell.className = "openCell";
            tempCell.dataset.digit = state;
            if(state != 0)
                tempCell.innerHTML = state;
            break;
    }
});

connection.on("loose", function () {
    document.getElementById("message").innerHTML = "haha you lost looser! XD";
    canDig = false;
});

connection.on("win", function () {
    document.getElementById("message").innerHTML = "haha you win looser! XD";
    canDig = false;
});

connection.on("playSound", function (soundString) {
    if (document.getElementById("soundCheckBox").checked) {
        if (soundString === "dirt") {
            var audio = new Audio("css/dirt.wav");
            audio.play();
        }
        if (soundString === "flag") {
            var audio = new Audio("css/flag.wav");
            audio.volume = 0.2;
            audio.play();
        }
    }
});

document.getElementById("startBtn").addEventListener("click", function (event) {
    var tempX = Number(document.getElementById("sizeXinput").value);
    var tempY = Number(document.getElementById("sizeYinput").value);
    var tempBombsCount = Number(document.getElementById("bombsCountInput").value);
    document.getElementById("message").innerHTML = "";
    document.getElementById("flagsCountDiv").innerHTML = "Flags left: " + tempBombsCount;
    canDig = true;
    connection.invoke("startGame", tempX, tempY, tempBombsCount);
});

connection.start();