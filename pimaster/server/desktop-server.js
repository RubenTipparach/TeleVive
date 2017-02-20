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
 var other_server = require("socket.io-client")('http://domain.com:8100'); // This is a client connecting to the SERVER 2


io.on('connection', (socket) => {
    console.log("Servers connected");

    // When we receive a message...
    socket.on("message",function(data){
        // We got a message... I dunno what we should do with this...
    });

    // original client trigger process.
    socket.on( 'event',
        (data) =>
        {
            console.log("recieved data: " + data.my);

            // do other stuff like take a picture!
            var spawn = require("child_process").spawn;

            var date = new Date().toISOString().replace(/:/, '-').replace(/\..+/, '');
            var photoName = 'photo' + date + '.jpg';
            var args = ['-o','photo'+date+'.jpg'];

            //var PHOTO_CMD = 'raspistill';
            //spawn(PHOTO_CMD, args);
            /*
            * TODO: Add Windows WebCam logic here!
            */
            console.log("Photo taken @ " + date);
            var msgPack = {my: "Photo taken @ " + date};

            setTimeout(() => {
                sendImage(photoName, socket);
            }, 10000);

            // notify client that a photo has been taken.
            socket.emit('photo_taken', {msg: msgPack.my});
    });
});

function sendImage(photoName, socket)
{
    fs.readFile(photoName, function(err, data) {
        if (err)
        {
             throw err; // Fail if the file can't be read.
        }

        socket.emit('image', { image: true, buffer: data.toString('base64') });
    });
}

server.listen(3000);
