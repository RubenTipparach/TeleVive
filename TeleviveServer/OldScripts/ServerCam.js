const net = require("net");

// Runs on the PI
// Create a simple server
var server = net.createServer(function (conn) {
    console.log("Server: Client connected");

    // If connection is closed
    conn.on("end", function() {
        console.log('Server: Client disconnected');
        // Close the server
        server.close();
        // End the process
        process.exit(0);
    });

    // Handle data from client
    conn.on("data", function(data) {
        data = JSON.parse(data);
        console.log("Response from client: %s", data.response);
    });

    // Let's response with a hello message


    // do other stuff like take a picture!
    var spawn = require("child_process").spawn;
    var PHOTO_CMD = 'raspistill';

    var date = new Date().toISOString().replace(/:/, '-').replace(/\..+/, '');
    var args = ['-o','photo'+date+'.jpg'];

    spawn(PHOTO_CMD, args);
    conn.write("Photo taken @ " + date);

});

// Listen for connections
server.listen(61337 , "192.168.0.22", function () {
    console.log("Server: Listening");
});
