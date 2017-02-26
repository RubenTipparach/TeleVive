/*
* CLIENT Script
* --------------------
* Your standard client side script. Will work on any platform since it is just standard client js.
*/

//var urlLink = 'http://192.168.0.22:3000';
var urlLink = 'http://localhost:3000';
var socket = io.connect(urlLink);

var allowedOrigins = urlLink;
var client = io(urlLink, {
    origins: allowedOrigins
});

socket.on('connect', () =>
{
    // socket connected
    console.log("server connected");
    //  socket.emit('server custom event', { my: 'data' });
});

socket.on('photo_taken', (data)=>
{
    // most likely a spooling icon will run while we wait for this.
    console.log("photo taken " + data.msg);
})
var canvasNumber = 0;

// prepare image to be recieved and processed.
socket.on('image', (info) => {
  if (info.image) {
    //var cvs = document.getElementById('canvas');
    //var ctx = cvs.getContext('2d');

    var cvs = document.createElement('canvas');

    cvs.id = 'canvas' + canvasNumber;
    canvasNumber++;

    //cvs.width = 800;
    //cvs.height = 600;

    var ctx = cvs.getContext("2d");

    cvs.height = 3280;
    cvs.width = 2464;

    console.log("Creating image...");

    var img = new Image();
    img.src = 'data:image/jpeg;base64,' + info.buffer;

    console.log("Downloading image..." + info.imageName);

    // Then create thumbnail.
    console.log("Creating thumbnail...");
    //document.getElementById("imageLoadSection").appendChild(link);

    // use node to resize pictures, this takes too long!
    new thumbnailer(cvs, img, 600, 3); //this produces lanczos3

    document.getElementById("imageLoadSection").appendChild(cvs);
    ctx.drawImage(img, 0, 0);
  }
});


function callServer()
{
    console.log("server called");
    socket.emit('photo_request', {my: 'data1234'});
}


function callServer2()
{

    for (var i = 0; i < 20; i++)
    {
        setTimeout(function () {    //  call a 3s setTimeout when the loop is called
            console.log("server called");
            socket.emit('photo_request', {my: 'data1234'});
            var snd = new Audio("audio.wav"); // buffers automatically when created
            snd.play();
        }, 10000 * i);
    }

}
