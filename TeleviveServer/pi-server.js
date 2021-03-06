
/*
* PI SERVER script
* --------------------
* This script is meant to be run on the Raspberry PIs waiting for a client or another server to request photos.
* It will then deliver photos (via some timing mechanism) to a master server or client.
*/
var app = require('express')();
var server = require('http').createServer(app);
var io = require('socket.io')(server);

var fs = require('fs');

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
            var args = ['-o','photo-PC-' + date + '.jpg'];

            var PHOTO_CMD = 'raspistill';
            spawn(PHOTO_CMD, args);

            console.log("Photo taken @ " + date);
            var msgPack = {my: "Photo taken @ " + date};

            setTimeout(() => {
                sendImage(photoName, socket);
            }, 10000);

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

server.listen(3000);
