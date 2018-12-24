// miniprogram/pages/goods/detail.js
const app = getApp();
const db = wx.cloud.database({ env: app.globalData.cloudenv });
Page({

  /**
   * 页面的初始数据
   */
  data: {
    styles:[],
    stylename:"默认",
    fuwu:true,
    flag: true,
    stylesflag:true,
    goodsuuid:"",
    goodsjifen:0,
    //bannerUrls: [],
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
    this.loadgoods();
  },
  /**
   * 弹出层函数
   */
  //出现
  show: function () {this.setData({ flag: false }); },
  hide: function () {this.setData({ flag: true }); },
  stylesshow: function () { this.setData({ stylesflag: false }); },
  styleshide: function () { this.setData({ stylesflag: true }); },
  stopPageScroll() {
    return
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
          if (res.data.length > 0) {
            wx.setNavigationBarTitle({
              title: res.data[0].text,
            })

            var guige = "默认";
            
            if (res.data[0].styles && res.data[0].styles.length > 0) {
              //styles = [];
              guige = "";
              res.data[0].styles.forEach(function (value, index, arrSelf) {
                if(guige==""){
                  guige = value.name;
                }else{
                  guige += "·" + value.name;
                }
                styles.push("");
                
              });
            }

            
            this0.setData({
              goods: res.data[0],
              goodsjifen: parseInt(res.data[0].discountprice / 100),
              stylename:guige,

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

  tapreviews:function(e){
    wx.navigateTo({
      url: '/pages/goods/reviews?id=' + this.data.goodsuuid,
    })
  },

  toshop: function (event) {
    const detailuri = '../index/index' ;
    console.log(detailuri)
    wx.switchTab({
      url: '/pages/index/index',
    })
  },

  fartap:function (e){
    wx.showToast({
      title: "收藏成功",
      duration: 1000,
      icon: "sucess",
      make: true
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