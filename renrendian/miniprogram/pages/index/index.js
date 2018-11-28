//index.js
const app = getApp();
const db = wx.cloud.database({ env: app.globalData.cloudenv});

Page({
  data: {
    lastcount: 0, // 上次加载了多少条数据。
    percount: 18, // 每次加载多少条数据。
    avatarUrl: './user-unlogin.png',
    userInfo: {},
    logged: false,
    takeSession: false,
    requestResult: '',
    bannerUrls: [
      {uri:'https://developers.weixin.qq.com/miniprogram/dev/image/cat/0.jpg',uuid:'goodsid0'},//图片src
      {uri:'../../images/ad01.png', uuid:'goodsid1'},
    ],
    bannerMenus:[],
    hotTitleAds:[],
    goodslist:[],
    discountlist:[],
    hotlist:[],
    ap:false,
    autoplay1: true,//是否自动播放
    autoplaytxt: "停止自动播放",
    indicatorDots: true,//指示点
    // indicator-color："white",//指示点颜色 暂未启动
    // indicator-active-color："red",//当前选中的指示点颜色暂未启动
    indicatorDotstxt: "隐藏指示灯",
    interval: 5000,//图片切换间隔时间
    duration: 500,//每个图片滑动速度,
    circular: true,//是否采用衔接滑动
    current: 0,//初始化时第一个显示的图片 下标值（从0）index
    indicatorDots2:false,
    autoplay2:true,
  },

  onLoad: function() {
    if (!wx.cloud) {
      wx.redirectTo({
        url: '../chooseLib/chooseLib',
      })
      return
    }

    this.initbannerads();
    this.initbannermenu();
    this.inithotTitleAds();
    this.loadgoodsdiscountlist();
    this.loadgoodslist();


    // 获取用户信息
    wx.getSetting({
      success: res => {
        if (res.authSetting['scope.userInfo']) {
          // 已经授权，可以直接调用 getUserInfo 获取头像昵称，不会弹框
          wx.getUserInfo({
            success: res => {
              this.setData({
                avatarUrl: res.userInfo.avatarUrl,
                userInfo: res.userInfo
              })
            }
          })
        }
      }
    })
  },

  /**
   * 页面上拉触底事件的处理函数
   */
  onReachBottom: function () {
    this.loadgoodslist();
  },  

  onGetUserInfo: function(e) {
    if (!this.logged && e.detail.userInfo) {
      this.setData({
        logged: true,
        avatarUrl: e.detail.userInfo.avatarUrl,
        userInfo: e.detail.userInfo
      })
    }
  },

  onGetOpenid: function() {
    // 调用云函数
    wx.cloud.callFunction({
      name: 'login',
      data: {},
      success: res => {
        console.log('[云函数] [login] user openid: ', res.result.openid)
        app.globalData.openid = res.result.openid
        wx.navigateTo({
          url: '../userConsole/userConsole',
        })
      },
      fail: err => {
        console.error('[云函数] [login] 调用失败', err)
        wx.navigateTo({
          url: '../deployFunctions/deployFunctions',
        })
      }
    })
  },


  swiperitemtap:function(event){
    const detailuri = '../goods/detail?id=' + event.currentTarget.dataset.aduri;
    console.log(detailuri)
    wx.navigateTo({
      url: detailuri,
    })
  },

  autoplaychange: function (event) {//停止、播放按钮

    if (this.data.autoplaytxt == "停止自动播放") {
      this.setData({
        autoplaytxt: "开始自动播放",
        autoplay1: !this.data.autoplay1
      })
    } else {
      this.setData({
        autoplaytxt: "停止自动播放",
        autoplay1: !this.data.autoplay1
      })
    };

  },

  swipermenu_tap:function(e){
    const classuri = '../goods/list?id=' + e.currentTarget.dataset.classid;
    console.log(classuri)    
    wx.navigateTo({
      url: classuri,
    })
  },

  goodsitemtap: function (event) {
    const detailuri = '../goods/detail?id=' + event.currentTarget.dataset.aduri;
    console.log(detailuri)
    wx.navigateTo({
      url: detailuri,
    })
  },

  imgchange: function (e) {//监听图片改变函数
    console.log(e.detail.current)//获取当前显示图片的下标值
  },

  inithotTitleAds:function(){
    var this0 = this;
    db.collection('store_hotgoodstitleads').where({
      storeuuid: 'W_UO50XacNtiP6m5'
    })
      .get({
        success: function (res) {
          console.log(res.data)
          if (res.data.length > 0) {
            this0.setData({
              hotTitleAds: res.data,
            })
          }
        },
        fail: function (e) {
          console.log(e);
        }
      })    

  },

  initbannerads: function (){
    var this0 = this;
    db.collection('store_bannerads').where({
      storeuuid: 'W_UO50XacNtiP6m5'
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
        },
        fail:function(e){
          console.log(e);
        }
      })    

  },

  initbannermenu: function () {
    //bannerUrls = [];
    var this0 = this;
    //const db = wx.cloud.database({ env: 'renrendian-749a2d' });
    db.collection('store_bannermenu').where({
      storeuuid: 'W_UO50XacNtiP6m5'
    })
      .get({
        success: function (res) {
          console.log(res.data)
          if(res.data.length > 0){
            this0.setData({
              bannerMenus: res.data[0].menupages,
            })
          }
        },
        fail: function (e) {
          console.log(e);
        }
      })
  } ,

  loadgoodsdiscountlist: function(){
    var this0 = this;
    //wx.showLoading({
    //  title: '加载中',
    //});
    const _ = db.command;
    db.collection('store_goodsdiscount').where({
      storeuuid: 'W_UO50XacNtiP6m5',
    })
      .get({
        success: function (res) {
          if (res.data.length > 0) {
            var gl = [];
            for (var index in res.data) {
              gl.push(res.data[index].goodsuuid);
            }
            db.collection('store_goods').where({
              storeuuid:'W_UO50XacNtiP6m5',
              _id: _.in(gl)//_.in(['5bfcedf84fd3484a16cf6336'])
            })
              .get({
                success: function (res) {
                  if (res.data.length > 0) {
                    this0.setData({ discountlist:res.data});
                  }
                }
              })


          }
          //wx.hideLoading();
        },
        fail: function (e) {
          console.log(e);
          //wx.hideLoading();
        }
      })

  },

  loadgoodslist: function () {
    var this0 = this;
    wx.showLoading({
      title: '加载中',
    });
    db.collection('store_goods').where({
      storeuuid: 'W_UO50XacNtiP6m5',
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
