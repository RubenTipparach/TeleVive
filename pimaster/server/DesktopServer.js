var app = require('express')();
var server = require('http').createServer(app);
var io = require('socket.io')(server);

 var fs = require('fs');

io.on('connection', (socket) => {
    console.log("Client connected");

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
