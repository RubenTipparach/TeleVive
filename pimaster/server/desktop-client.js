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

// prepare image to be recieved and processed.
socket.on("image", (info) => {
  if (info.image) {
    var cvs = document.getElementById('canvas');
    var ctx = cvs.getContext('2d');

    //cvs.height = 3280;
    //cvs.width = 2464;

    console.log("Creating image...");

    var img = new Image();
    img.src = 'data:image/jpeg;base64,' + info.buffer;

    console.log("Downloading image..." + info.imageName);

    //download image, might just do previews of images instead. idealy, this is localhost only!
    // var dt = cvs.toDataURL('image/jpeg');
    // var link = document.createElement('a');
    // link.innerHTML = info.imageName;
    // document.getElementById("imageLoadSection").appendChild(link);

    // link.addEventListener('click', function() {
    //     link.href = cvs.toDataURL();
    //     link.download = info.imageName;
    // }, false);

    //link.click();

    // Then create thumbnail.
    console.log("Creating thumbnail...");
    new thumbnailer(cvs, img, 600, 3); //this produces lanczos3

    // ctx.drawImage(img, 0, 0);
  }
});


function callServer()
{
    console.log("server called");
    socket.emit('photo_request', {my: 'data1234'});
}
