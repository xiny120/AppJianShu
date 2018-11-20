//index.js
const app = getApp()
// 引入SDK核心类
var QQMapWX = require('../map/qqmap-wx-jssdk.js');
var qqmapsdk;

Page({
  data: {
    avatarUrl: './user-unlogin.png',
    userInfo: {},
    logged: false,
    takeSession: false,
    requestResult: ''
  },

  tapforum:function(){
    wx.navigateTo({
      url: '../forum/forum',
    })    
  },

  tapmy:function(){
    wx.navigateTo({
      url: '../userConsole/userConsole',
    })    
  },

  onLoad: function() {
    if (!wx.cloud) {
      wx.redirectTo({
        url: '../chooseLib/chooseLib',
      })
      return
    }

    // 实例化API核心类
    //qqmapsdk = new QQMapWX({
    //  key: 'VERBZ-LYKKX-PIU45-763SC-UR666-6ZFNI'
    //});    

  },

})
