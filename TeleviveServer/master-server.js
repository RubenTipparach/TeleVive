/*
* MASTER SERVER Script
* --------------------
* Will be used for handling, synchronizing and processing data.
* Will also be used for distributing image processing, and creating thumbnails for clients.
* Will also host and manage 3D models for downloading and storage. Should probably run on a Mongo DB server.
*
* based on this: http://stackoverflow.com/questions/14118076/socket-io-server-to-server
*
* NOTE: This is also the policy server. All clients connect through here from now on!
*/
//var urlLink = 'http://192.168.0.22:3000'; // loop through this in the future.
//var urlLink = 'http://192.168.0.5:3001';
//var other_server = require("socket.io-client")(urlLink);

var urlLinks = [
    'http://192.168.0.22:3000',
    //'http://192.168.0.5:3001',
//    'http://192.168.0.7:3001',
    //'http://192.168.0.4:3001'
];

var app = require('express')();
var server = require('http').createServer(app);
var io = require('socket.io')(server);

var fs = require('fs');
var sharp = require('sharp');

var camServer = require("socket.io-client");
var filemanager = require('easy-file-manager');

var other_servers = new Array(2);

// iterate through all of them
for (var i = 0; i < urlLinks.length; i++)
{
    var link = urlLinks[i];
    console.log("connecting to " + link);

    var dataLink = {link: link};
    // it recycles. wow.
    var serverInst = require("socket.io-client")(link, {
        forceNew: true
    });

    //var serverInst = other_servers[i];
    serverInst.on("connect", () =>
    {
        console.log("connection made from MASTER to PI SERVERS." ); // TODO: Register Pi array through JSON script

    });

    // forward message from SERVERS to CLIENTS
    serverInst.on("photo_taken", (info) =>
    {
        io.sockets.emit("photo_taken", info);
    });

    serverInst.on('image', (info) =>
    {
        //emit to clients
        console.log("photo taken! " + info.imageName); // two of the same message recieved!
        //writeImageToDisk("pics/" + info.imageName + ".jpg", info.buffer);

        filemanager.upload("pics", info.imageName , info.buffer ,function(err){
            if (err) console.log(err);
        });

        io.sockets.emit("image",  info);
    });

    other_servers[i] = serverInst;
}

// write stuff to my hard drive
function writeImageToDisk(path, buffer)
{
    fs.open(path, 'w', (err, fd) => {
        if (err) {
            throw 'error opening file: ' + err;
        }

        fs.write(fd, buffer, 0, buffer.length, null, function(err) {
            if (err) throw 'error writing file: ' + err;
            fs.close(fd, function() {
                console.log('file written');
            })
        });
    });
}

// idk what to do with this yet.
function connectToDevices(linkServer, id)
{

}

/*
* Client Service section.
*/
// Gets called every time a client connects here!
io.sockets.on("connection", (socket) =>{
    // Display a connected message
    console.log("User-Client Connected!");

    // Lets force this connection into the lobby room.
    socket.join('lobby');

    // forward message from CLIENTS to SERVERS
    socket.on("photo_request", (data) =>
    {
        console.log("photo request recieved " + data.my);

        // how to differentiate.
        for (var i = 0; i < urlLinks.length; i++)
        {
            other_servers[i].emit("photo_request", data);
        }
    });

    // termination events.
    socket.on("disconnect", (data) =>
    {
    });
});


server.listen(3000);
