/*
* DESKTOP PC SERVER script
* --------------------
* This script is meant to be run on the Desktop/Laptops waiting for a client or another server to request photos.
* It will then deliver photos (via some timing mechanism) to a master server or client.
*
* NOTE: same as PI script, but ported over to PCs.
*/
var app = require('express')();
var server = require('http').createServer(app);
var io = require('socket.io')(server);

var fs = require('fs');
var NodeWebcam = require( 'node-webcam' );

// receiveing from a server <-> server
io.sockets.on('connection', (socket) =>
{
    console.log("MASTER SERVER has connected");

    socket.on( 'photo_request',
        (data) =>
        {
            console.log("recieved data: " + data.my);

            // do other stuff like take a picture!
            var spawn = require("child_process").spawn;

            var date = new Date().toISOString().replace(/:/g, '-').replace(/\..+/, '');
            var photoName = 'photo-PC-' + date + '.jpg';

            var opts = {
                width: 1280,
                height: 720,
                delay: 0,
                quality: 100,
                output: "jpeg",
                verbose: true
            }

            var Webcam = NodeWebcam.create( opts );

            //Will automatically append location output type
            Webcam.capture(photoName);

            console.log("Photo taken @ " + date);
            var msgPack = {my: "Photo taken @ " + date};

            setTimeout(() => {
                sendImage(photoName, socket);
            }, 5000);

            socket.emit('photo_taken', {msg: msgPack.my});
    });
});

function sendImage(photoName, socket)
{
    fs.readFile(photoName, (err, data) => {

        if (err)
        {
             throw err; // Fail if the file can't be read.
        }

        socket.emit('image', { image: true, buffer: data.toString('base64'), imageName: photoName});
    });
}

server.listen(3001);
