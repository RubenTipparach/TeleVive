var RaspiCam = require("raspicam");
var camera = new RaspiCam({ mode: "photo", output:"randomwhatever.jpg", encoding: "jpg" });

console.log("starting");
var success = camera.start();

console.log("sucess " + success);

camera.stop()

//listen for the "start" event triggered when the start method has been successfully initiated
camera.on("start", function(){
    //do stuff
   console.log("started doing stuff");
});

//listen for the "read" event triggered when each new photo/video is saved
camera.on("read", function(err, timestamp, filename){ 
    //do stuff
});

//listen for the "stop" event triggered when the stop method was called
camera.on("stop", function(){
    //do stuff
});

//listen for the process to exit when the timeout has been reached
camera.on("exit", function(){
    //do stuff
});
