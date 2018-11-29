// pages/userConsole/userConsole.js
const app = getApp();
const db = wx.cloud.database({ env: app.globalData.cloudenv });

Page({

  data: {
    openid: ''
  },

  onLoad: function (options) {
    /*
    this.setData({
      openid: getApp().globalData.openid
    })
    */
  },

  testCgi:function(e){
    //this.loadgoodslist();
  },



  loadgoodslist: function () {
    var this0 = this;
    wx.showLoading({
      title: '加载中',
    });
    db.collection('store_goods').where({
      storeuuid: 'W_UO50XacNtiP6m5',
    }).field({
      brandid : true,
      classid:true,
      detail:true,
      discountprice:true,
      liketotal:true,
      logo: true,
      originalprice: true,
      pvtotal: true,
      storeuuid: true,
      svmonth: true,
      svtotal: true,
      text: true,
      text02: true,
    })
      .get({
        success: function (res) {
          console.log(res.data)
          //data.bannerUrls.push(res.data);
          db.collection('store_goods').add({data:res.data[0]})
          for(index in res.data){

          }
          if (res.data.length > 0) {
          }
          wx.hideLoading();
          if (res.data.length <= 0) {
            wx.showToast({
              title: '到底了哦！',
            })
          }
        },
        fail: function (e) {
          console.log(e);
          wx.hideLoading();
        }
      })

  }  


})