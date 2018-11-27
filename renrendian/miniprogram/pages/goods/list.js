// miniprogram/pages/goods/list.js

const app = getApp();
const db = wx.cloud.database({ env: app.globalData.cloudenv });

Page({

  /**
   * 页面的初始数据
   */
  data: {
    classid: "",
    goodslist:{},
  },

  /**
   * 生命周期函数--监听页面加载
   */
  onLoad: function (opts) {
    var data = opts;
    this.setData({classid:opts.id});
    this.initgoodslist();

  },

  /**
   * 生命周期函数--监听页面初次渲染完成
   */
  onReady: function () {

  },

  /**
   * 生命周期函数--监听页面显示
   */
  onShow: function () {

  },

  /**
   * 生命周期函数--监听页面隐藏
   */
  onHide: function () {

  },

  /**
   * 生命周期函数--监听页面卸载
   */
  onUnload: function () {

  },

  /**
   * 页面相关事件处理函数--监听用户下拉动作
   */
  onPullDownRefresh: function () {

  },

  /**
   * 页面上拉触底事件的处理函数
   */
  onReachBottom: function () {

  },

  /**
   * 用户点击右上角分享
   */
  onShareAppMessage: function () {

  },


  initgoodslist: function () {
    var this0 = this;
    wx.showLoading({
      title: '加载中',
    });
    db.collection('store_goods').where({
      storeuuid: 'W_UO50XacNtiP6m5',
      classid:this.data.classid
    })
      .get({
        success: function (res) {
          console.log(res.data)
          //data.bannerUrls.push(res.data);
          if (res.data.length > 0) {
            this0.setData({
              goodslist: res.data,
            })
          }
          wx.hideLoading();
        },
        fail: function (e) {
          console.log(e);
          wx.hideLoading();
        }
      })

  }  
})