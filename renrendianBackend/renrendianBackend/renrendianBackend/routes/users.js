'use strict';
var express = require('express');
var fs = require('fs');
var router = express.Router();
var app = express();

// 初始化示例
const tcbapp = require('tcb-admin-node');



/* GET users listing. */
router.get('/', function (req, res) {

    readJson(res);

    res.send('respond with a resource');
});

module.exports = router;


function readJson(res) {
    //现将json文件读出来
    fs.readFile('./setup/qcloud.json', 'utf-8', function (err, data) {
        if (err) {
            return console.error(err);
        }
        var person = data.toString();//将二进制的数据转换为字符串
        person = JSON.parse(person);//将字符串转换为json对象
        app.set("SecretId", person.SecretId);
        app.set("SecretKey", person.SecretKey);

        // 初始化资源
        // 云函数下不需要secretId和secretKey。
        // env如果不指定将使用默认环境
        tcbapp.init({ secretId: person.SecretId, secretKey: person.SecretKey, env: 'subrac-89aa62' });
        tcbapp.config.timeout = 1000 * 5 * 60;
        const db = tcbapp.database();
        //res.send("open database! ");

        db.collection('store_bannermenu').where({
            storeuuid: 'W_UO50XacNtiP6m5'
        })
            .get({
                success: function (res0) {
                    //console.log(res.data);
                    res.send(" store_bannermenu ok! ");
                },
                fail: function (e) {
                    //console.log(e);
                    res.send(e);
                }
            });

    });
}