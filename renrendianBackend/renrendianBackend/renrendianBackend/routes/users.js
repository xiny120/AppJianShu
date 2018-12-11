'use strict';
var express = require('express');
var fs = require('fs');
var router = express.Router();
var app = express();

/* GET users listing. */
router.get('/', function (req, res) {

    readJson();

    res.send('respond with a resource');
});

module.exports = router;


function readJson() {
    //现将json文件读出来
    fs.readFile('./setup/qcloud.json', 'utf-8', function (err, data) {
        if (err) {
            return console.error(err);
        }
        var person = data.toString();//将二进制的数据转换为字符串
        person = JSON.parse(person);//将字符串转换为json对象
        app.set("SecretId", person.SecretId);
        app.set("SecretKey", person.SecretKey);
    });
}