// miniprogram/pages/goods/detail.js
const app = getApp();
const db = wx.cloud.database({ env: app.globalData.cloudenv });
Page({

  /**
   * 页面的初始数据
   */
  data: {
    goodsuuid:"",
    bannerUrls: [],
    goods:{},
    autoplay1: false,//是否自动播放
    autoplaytxt: "停止自动播放",
    indicatorDots: true,//指示点
    // indicator-color："white",//指示点颜色 暂未启动
    // indicator-active-color："red",//当前选中的指示点颜色暂未启动
    indicatorDotstxt: "隐藏指示灯",
    interval: 5000,//图片切换间隔时间
    duration: 500,//每个图片滑动速度,
    circular: true,//是否采用衔接滑动
    current: 0,//初始化时第一个显示的图片 下标值（从0）index    
  },

  /**
   * 生命周期函数--监听页面加载
   */
  onLoad: function (opts) {
    this.setData({ goodsuuid: opts.id });
    this.initbannerads();
    this.loadgoods();
  },


  initbannerads: function () {
    wx.showLoading({
      title: '加载中...',
    })
    var this0 = this;
    db.collection('store_goodsswiper').where({
      storeuuid: 'W_UO50XacNtiP6m5',
      goodsuuid:this0.data.goodsuuid
    })
      .get({
        success: function (res) {
          console.log(res.data)
          //data.bannerUrls.push(res.data);
          if (res.data.length > 0) {
            this0.setData({
              bannerUrls: res.data,
            })
          }

          wx.hideLoading();
        },
        fail: function (e) {
          console.log(e);
          wx.hideLoading();
        }
      })

  },


  loadgoods: function () {
    wx.showLoading({
      title: '加载中...',
    })
    var this0 = this;
    db.collection('store_goods').where({
      storeuuid: 'W_UO50XacNtiP6m5',
      _id: this0.data.goodsuuid
    })
      .get({
        success: function (res) {
          console.log(res.data)
          //data.bannerUrls.push(res.data);
          if (res.data.length > 0) {
            wx.setNavigationBarTitle({
              title: res.data[0].text,
            })
            this0.setData({
              goods: res.data[0],
            })
          }

          wx.hideLoading();
        },
        fail: function (e) {
          console.log(e);
          wx.hideLoading();
        }
      })

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

  }
})