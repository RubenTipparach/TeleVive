/*
* MASTER SERVER Script
* --------------------
* Will be used for handling, synchronizing and processing data.
* Will also be used for distributing image processing, and creating thumbnails for clients.
* Will also host and manage 3D models for downloading and storage. Should probably run on a Mongo DB server.
*
* NOTE: This is also the policy server. All clients connect through here from now on!
*/
var urlLink = 'http://192.168.0.22:3000';
var other_server = require("socket.io-client")(urlLink);

var app = require('express')();
var server = require('http').createServer(app);
var io = require('socket.io')(server);

var fs = require('fs');
var sharp = require('sharp');


other_server.on("connect",function()
{
    console.log("connection made from MASTER to PI SERVERS.") // TODO: Register Pi array through JSON script
    other_server.on('message',function(data)
    {
        // We received a message from Server 2
        // We are going to forward/broadcast that message to the "Lobby" room
        // TODO: Idk how to use a lobby system yet, but it does sound useful
        io.to('lobby').emit('message',data);
    });
});

io.sockets.on("connection",function(socket){
    // Display a connected message
    console.log("User-Client Connected!");

    // Lets force this connection into the lobby room.
    socket.join('lobby');

    // Some roster/user management logic to track them
    // This would be upto you to add :)

    // When we receive a message...
    socket.on("message",function(data){
        // We need to just forward this message to our other guy
        // We are literally just forwarding the whole data packet
        other_server.emit("message",data);
    });

    socket.on("disconnect",function(data){
        // We need to notify Server 2 that the client has disconnected
        other_server.emit("message","UD,"+socket.id);

        // Other logic you may or may not want
        // Your other disconnect code here
    });
});
