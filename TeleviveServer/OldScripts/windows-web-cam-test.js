
var NodeWebcam = require( "node-webcam" );


//Default options

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

Webcam.capture( "test_picture" );
