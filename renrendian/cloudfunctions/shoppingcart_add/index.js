// 云函数入口文件
const cloud = require('wx-server-sdk')

cloud.init({ env:'subrac-89aa62'})
const db = cloud.database();
// 云函数入口函数
exports.main = async (event, context) => {
  const wxContext = cloud.getWXContext()

/*
  return await db.collection('store_shoppingcart').add({
    data: {
      goodsuuid: event.goodsuuid,
      'sku.sku': event.sku,
    }
  })
  */

  
  try {
    console.log("查询商品是否以进购物车 \r\n" + event.goodsuuid + "\r\n");
    await db.collection('store_shoppingcart').where({
      goodsuuid:event.goodsuuid,
      //'sku.sku':event.sku,

    }).get({
      success: function (res) {
        console.log("查到已有该商品\r\n");
        if (res.data.length > 0) {
          
        }
        else{
          /*
          await db.collection('store_shoppingcart').add({
            // data 字段表示需新增的 JSON 数据
            data: {
              goodsuuid: event.goodsuuid,
              sku: event.sku,
              count: 0,
            }
          })      */    
        }
      }, 
      fail: function (e) {
        console.log("查询执行失败！\r\n");
      }           
    });

  } catch (e) {
    console.log(e)
  }  

  return {
    event,
    openid: wxContext.OPENID,
    appid: wxContext.APPID,
    unionid: wxContext.UNIONID,
  }
}