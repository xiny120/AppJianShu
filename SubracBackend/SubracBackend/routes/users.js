'use strict';
var express = require('express');
var router = express.Router();

// 初始化示例
const cloud = require('tcb-admin-node');

// 初始化资源
// 云函数下不需要 secretId和secretKey，但如果在自己的服务器里使用则需要
// env如果不指定将使用默认环境
cloud.init({
    secretId: 'AKIDWwIl6PeYO5cMxPl7IOSOBb9atWzHtzQl',
    secretKey: 'Kfy78CA0ELJlEvY1QhokWGVcbP6cOBpJ',
});

// 获取所有数据的方法
async function getData(colName) {
    const db = cloud.database();
    const userCollection = db.collection(colName);

    // 统计数据总量
    let res = await userCollection.count();
    let total = res.total;

    let data = [];
    let length = 0;
    let start = 0;

    // 循环将数据读出来
    while (total > length) {
        let res = await userCollection.skip(start).get();

        // 读出来后将数据存到data里
        data = data.concat(res.data);
        length += res.data.length;
        start += length;
    }

    return data;
}



/* GET users listing. */
router.get('/', function (req, res) {

    getData('miniappSetup').then((data) => {
        console.log(data);
    }); // 调用方法

    res.send('respond with a resource');
});

module.exports = router;
