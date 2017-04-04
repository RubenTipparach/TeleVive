var spawn = require("child_process").spawn;

var PHOTO_CMD = 'raspistill';

var args = ['-o','randomyo.jpg'];
spawn(PHOTO_CMD, args);
