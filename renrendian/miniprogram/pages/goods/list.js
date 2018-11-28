// miniprogram/pages/goods/list.js

const app = getApp();
const db = wx.cloud.database({ env: app.globalData.cloudenv });

Page({

  /**
   * 页面的初始数据
   */
  data: {
    classid: "",
    lastcount:0, // 上次加载了多少条数据。
    percount:18, // 每次加载多少条数据。
    goodslist:[],
  },

  goodsitemtap: function (event) {
    const detailuri = '../goods/detail?id=' + event.currentTarget.dataset.aduri;
    console.log(detailuri)
    wx.navigateTo({
      url: detailuri,
    })
  },  

  /**
   * 生命周期函数--监听页面加载
   */
  onLoad: function (opts) {
    var data = opts;
    this.setData({classid:opts.id});
    this.loadgoodslist();

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
    this.loadgoodslist();
  },

  

  /**
   * 用户点击右上角分享
   */
  onShareAppMessage: function () {

  },


  loadgoodslist: function () {
    var this0 = this;
    wx.showLoading({
      title: '加载中',
    });
    db.collection('store_goods').where({
      storeuuid: 'W_UO50XacNtiP6m5',
      classid:this.data.classid
    }).skip(this0.data.lastcount).limit(this0.data.percount)
      .get({
        success: function (res) {
          console.log(res.data)
          //data.bannerUrls.push(res.data);
          if (res.data.length > 0) {
            var gl = this0.data.goodslist;
            gl = gl.concat(res.data);
            this0.setData({
              goodslist: gl,
            })
            this0.data.lastcount += res.data.length;
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