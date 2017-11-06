/*
import nodejs package(s)
*/
var express = require('express');
var app = express();

/*
respond with "hello world" when a GET request is made to the homepage
*/
app.get('/', function (req, res) {
  res.sendFile(__dirname + '/index.html');
});

/*
listen for client connections on localhost and port 3000
*/
app.listen(3000, function () {
  console.log('example app listening on port 3000');
});
