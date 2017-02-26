var filemanager = require('easy-file-manager');
var request = require('request').defaults({ encoding: null });

var path = "/public";
var filename = "test.jpg";
var imgUrl = "http://cdn2-www.cattime.com/assets/uploads/gallery/25-funny-cat-memes/funny-forrest-gump-parody-cat-memes.jpg";
request.get(imgUrl, function (err, res, data) {
      //process exif here
      filemanager.upload(path, filename, data,function(err){
          if (err) console.log(err);
      });
});
