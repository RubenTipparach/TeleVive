var urlLink = 'http://192.168.0.22:3000';
//var urlLink = 'http://localhost:3000';
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
    console.log("photo taken " + data.msg);
})

socket.on("image", (info) => {
  if (info.image) {
    var cvs = document.getElementById('canvas');
    var ctx = cvs.getContext('2d');

    var img = new Image();
    img.src = 'data:image/jpeg;base64,' + info.buffer;
    ctx.drawImage(img, 0, 0);
  }
});

function callServer()
{
    console.log("server called");
    socket.emit('event', {my: 'data1234'});
}
