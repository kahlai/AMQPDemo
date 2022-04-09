"use strict";

var connection = new signalR.HubConnectionBuilder().withUrl("/chatHub").build();

async function start() {
    try {
        await connection.start();
        console.log("SignalR Connected.");
        document.getElementById("conn_status").innerHTML = '<div style="background-color:green;color:white;text-align:center">Connected<div>';

        document.getElementById("sendButton").disabled = false;
    } catch (err) {
        console.log(err);
        setTimeout(start, 5000);
    }
};

connection.onclose(async () => {
    console.log("Lose connection to server");
    document.getElementById("conn_status").innerHTML = '<div style="background-color:red;color:white;text-align:center">Disconnected<div>';
    await start();
});

// Start the connection.
start();


//Disable send button until connection is established
document.getElementById("sendButton").disabled = true;

connection.on("ReceiveMessage", function (user, message) {
    var msg = message.replace(/&/g, "&amp;").replace(/</g, "&lt;").replace(/>/g, "&gt;");
    var encodedMsg = user + " - " + msg;
    var li = document.createElement("li");
    li.textContent = encodedMsg;
    document.getElementById("messagesList").appendChild(li);
});

connection.start().then(function () {
    document.getElementById("sendButton").disabled = false;
}).catch(function (err) {
    return console.error(err.toString());
});

document.getElementById("sendButton").addEventListener("click", function (event) {
    var user = document.getElementById("targetInput").value;
    var message = document.getElementById("messageInput").value;
    connection.invoke("SendMessage", user, message).catch(function (err) {
        return console.error(err.toString());
    });
    event.preventDefault();
});


document.getElementById("toogleButton").addEventListener("click", function (event){
        var display = document.getElementById("advance_opt").style.display;
        if(display=='none'){
            document.getElementById("advance_opt").style.display = "block";
            document.getElementById("basic_opt").style.display = "none";
            this.value="Less";
        }else{
            document.getElementById("advance_opt").style.display = "none";
            document.getElementById("basic_opt").style.display = "block";
            this.value="More";
        }
    });

var count=1;
var started=false;
function startLoop(){
    if(started){
        var user = document.getElementById("targetInput").value;
        var message = document.getElementById("messageInput").value;
        var appendCount = document.getElementById("appendCount").checked;
        var intervalTime = parseInt(document.getElementById("interval").value);
        console.log("appendCount" + appendCount);
        console.log("intervalTime" + intervalTime);
        if(appendCount){
            message += ' ' + count;
            count++;
        }
        connection.invoke("SendMessage", user, message).catch(function (err) {
            return console.error(err.toString());
        });
        setTimeout( startLoop,intervalTime);
    }
    
}


document.getElementById("startButton").addEventListener("click", function (event) {
    started = !started;
    startLoop();
    if(started){
        document.getElementById("startButton").value="Stop";
    }else{
        document.getElementById("startButton").value="Start";
    }
    event.preventDefault();
});