// miniprogram/pages/goods/detail.js
const app = getApp();
const db = wx.cloud.database({ env: app.globalData.cloudenv });
Page({

  /**
   * 页面的初始数据
   */
  data: {
    detailbanner:"",
    ccstylechoose: "请选择",
    skuchoose:[],
    stylechoose:"请选择",
    stock:"_",
    styles:[],
    stylesname:[],
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


  testFunction() {
    if(this.data.skuchoose.length < 1){
      wx.showToast({
        title: '请选择商品规格！',
        icon:'none',
      })
      return;
    }

    wx.cloud.callFunction({
      name: 'shoppingcart_add',
      data: {
        goodsuuid: this.data.goodsuuid,
        sku: this.data.skuchoose,
      },
      success: res => {
        wx.showToast({
          icon: 'none',
          title: '加入购物车成功！',
        })

      },
      fail: err => {
        wx.showToast({
          icon: 'none',
          title: '加入购物车失败！',
        })
        console.error('[云函数] [sum] 调用失败：', err)
      }
    })
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
            var styles0 = [];
            var sc = "请选择: ";
            if (res.data[0].styles && res.data[0].styles.length > 0) {
              styles0 = new Array(res.data[0].styles.length);
              guige = "";
              res.data[0].styles.forEach(function (value, index, arrSelf) {
                if(guige==""){
                  guige = value.name;
                }else{
                  guige += "·" + value.name;
                }
                sc = sc + " " + value.name;
                styles0[index] = "";
              });
            }

            
            this0.setData({
              goods: res.data[0],
              goodsjifen: parseInt(res.data[0].discountprice / 100),
              stylename:guige,
              styles:styles0,
              stylechoose:sc,
              ccstylechoose:sc,
              detailbanner: res.data[0].detailbanners[0],

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

  stylestap:function(e){
    var this0 = this;
    var sc = this.data.ccstylechoose;
    var sced = "已选择:";
    var s = this.data.styles;
    var sn = this.data.stylesname
    if (s[e.target.dataset.idx] == ""){
      s[e.target.dataset.idx] = e.target.dataset.value;
      sn[e.target.dataset.idx] = e.target.dataset.name;
    }else{
      if (s[e.target.dataset.idx] == e.target.dataset.value)
        s[e.target.dataset.idx] = "";
      else
        s[e.target.dataset.idx] = e.target.dataset.value;
        sn[e.target.dataset.idx] = e.target.dataset.name;
    }
    var idx = 0;
    sc = "请选择:";
    s.forEach(function (value, index, arrSelf) {
      if(value == "") {
        sc = sc + " " + this0.data.goods.styles[index].name;
        idx = 1;
      }else{
        //sced = sced + " " + this0.data.stylesname[index];
      }
    });

    if(idx < 1){
      //sc = sced;

      db.collection('store_goodsskus').where({
        storeuuid: 'W_UO50XacNtiP6m5',
        goodsuuid: this0.data.goodsuuid,
      })
        .get({
          success: function (res) {
            console.log(res.data)
            if (res.data.length > 0) {
              var sku = null;
              res.data[0].skus.forEach(function(val,idx,arr){
                var count = 0;
                val.sku.forEach(function(val0,idx0,arro){
                  if(val0 == s[idx0])
                    count ++;
                  else
                    return;
                });

                if(count == val.sku.length){ // 有此SKU
                  sku = val;
                  return;
                }else{
                  // this0.setData({
                  //  stock: "_",
                  //  stylechoose: "此型号无货，请重新选择！",
                  //});                  
                }

              });
              if(sku == null){
                  this0.setData({
                    skuchoose:[],
                    stock: "_",
                    stylechoose: "此型号无货，请重新选择！",
                  });  
              }else{
                this0.setData({
                  skuchoose:sku,
                  stock: sku.stock,
                  stylechoose: "已选择：" + sku.intro,
                });
              }

            }
          },
          fail: function (e) {
            console.log(e);
          }
        })        

    }

    this.setData({
      stylesname:sn,
      styles:s,
      stylechoose:sc,
    });
  },

  tapreviews:function(e){
    wx.navigateTo({
      url: '/pages/goods/reviews?id=' + this.data.goodsuuid,
    })
  },

  tappage__cart:function(e){
    const detailuri = '../shoppingcart/shoppingcart';
    console.log(detailuri)
    wx.switchTab({
      url: '/pages/shoppingcart/shoppingcart',
    })
    //wx.navigateTo({
    //  url: '/pages/shoppingcart/shoppingcart',
    //})    

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