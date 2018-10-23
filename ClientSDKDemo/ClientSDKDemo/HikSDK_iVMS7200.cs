using System;
using System.Runtime.InteropServices;
using System.Text;
using System.Windows.Forms;

namespace Hik_iVMS7200
{
    /// <summary>
    /// 海康视频库
    /// </summary>
    public static class HikSDK_iVMS7200
    {

        /// <summary>
        /// 视频流数据（包括复合流和音视频分开的视频流数据）
        /// </summary>
        public const int STREAM_DATATYPE_DATA = 0;
        /// <summary>
        /// 视频头信息
        /// </summary>
        public const int STREAM_DATATYPE_HDR = 1;
        /// <summary>
        /// 视频实时
        /// </summary>
        public const UInt32 STREAME_REALTIME = 0;

        /// <summary>
        /// 视频文件
        /// </summary>
        public const UInt32 STREAME_FILE = 1;

        /// <summary>
        /// 视频编码形式
        /// </summary>
        public enum BICompression : uint
        {
            /// <summary>
            /// 不压缩
            /// </summary>
            BI_RGB = 0,
            /// <summary>
            /// 8位运行长度编码
            /// </summary>
            BI_RLE8 = 1,
            /// <summary>
            /// 4位运行长度编码
            /// </summary>
            BI_RLE4 = 2,
            /// <summary>
            /// 使用颜色位屏蔽
            /// </summary>
            BI_BITFIELDS = 3
        }


        /// <summary>
        /// 视频图像头信息
        /// </summary>
        public struct BitmapInfoHeader
        {
            /// <summary>
            /// 结构的大小>=40
            /// </summary>
            public uint biSize;
            /// <summary>
            /// 图像宽度（像素）
            /// </summary>
            public int biWidth; // 
            /// <summary>
            /// 图像高度（像素）
            /// </summary>
            public int biHeight; // 
            /// <summary>
            /// = 1
            /// </summary>
            public ushort biPlanes;
            /// <summary>
            /// 每像素的位数（1,4,8,16,24,32）
            /// </summary>
            public ushort biBitCount;
            /// <summary>
            /// 压缩
            /// </summary>
            public BICompression biCompression;
            /// <summary>
            /// 图像的字节数
            /// </summary>
            public uint biSizeImage;
            /// <summary>
            /// 横向分辨率
            /// </summary>
            public int biXPelsPerMeter;
            /// <summary>
            /// 纵向分辨率
            /// </summary>
            public int biYPelsPerMeter;
            /// <summary>
            /// 使用的颜色数
            /// </summary>
            public uint biClrUsed;
            /// <summary>
            /// 重要颜色数
            /// </summary>
            public uint biClrImportant;

            /// <summary>
            /// 视频图像头信息
            /// </summary>
            /// <param name="src">数据</param>
            /// <param name="off">开始位置</param>
            public BitmapInfoHeader(byte[] src, int off)
            {
                biSize = BitConverter.ToUInt32(src, off);
                biWidth = BitConverter.ToInt32(src, off + 4);
                biHeight = BitConverter.ToInt32(src, off + 8);
                biPlanes = BitConverter.ToUInt16(src, off + 12);
                biBitCount = BitConverter.ToUInt16(src, off + 14);
                biCompression = (BICompression)BitConverter.ToUInt32(src, off + 16);
                biSizeImage = BitConverter.ToUInt32(src, off + 20);
                biXPelsPerMeter = BitConverter.ToInt32(src, off + 24);
                biYPelsPerMeter = BitConverter.ToInt32(src, off + 28);
                biClrUsed = BitConverter.ToUInt32(src, off + 32);
                biClrImportant = BitConverter.ToUInt32(src, off + 36);
            }
        }

        /// <summary>
        /// 图像文件头信息
        /// </summary>
        public struct BitmapFileHeader
        {
            /// <summary>
            /// 0x4D42
            /// </summary>
            public ushort bfType;
            /// <summary>
            /// 整个文件的大小
            /// </summary>
            public uint bfSize;
            /// <summary>
            /// 保留为0
            /// </summary>
            public ushort bfReserved1;
            /// <summary>
            /// 保留为0
            /// </summary>
            public ushort bfReserved2;
            /// <summary>
            /// 位块的偏移量
            /// </summary>
            public uint bfOffsetBits;
            /// <summary>
            /// 图像文件头信息
            /// </summary>
            /// <param name="src">数据</param>
            /// <param name="off">开始位置</param>
            public BitmapFileHeader(byte[] src, int off)
            {
                bfType = BitConverter.ToUInt16(src, off);
                bfSize = BitConverter.ToUInt32(src, off + 2);
                bfReserved1 = BitConverter.ToUInt16(src, off + 6);
                bfReserved2 = BitConverter.ToUInt16(src, off + 8);
                bfOffsetBits = BitConverter.ToUInt32(src, off + 10);
            }
        }

        /// <summary>
        /// 注册服务器登录信息
        /// </summary>
        [StructLayoutAttribute(LayoutKind.Sequential)]
        public struct ST_ACCESS_SERVER_INFO
        {
            /// <summary>
            /// 注册服务器IP
            /// </summary>
            [MarshalAsAttribute(UnmanagedType.ByValTStr, SizeConst = 32)]
            public string szAcessServerIP;
            /// <summary>
            /// 注册服务器端口
            /// </summary>
            public ushort nAcessServerPort;
            /// <summary>
            /// 保留字段
            /// </summary>
            public ushort nReserved;
            /// <summary>
            /// 登录用户名
            /// </summary>
            [MarshalAsAttribute(UnmanagedType.ByValTStr, SizeConst = 32)]
            public string szUserName;
            /// <summary>
            /// 登录密码
            /// </summary>
            [MarshalAsAttribute(UnmanagedType.ByValTStr, SizeConst = 32)]
            public string szUserPwd;
        }

        /// <summary>
        /// 注册服务上的设备信息
        /// </summary>
        [StructLayoutAttribute(LayoutKind.Sequential)]
        public struct ST_DEVICEINFO_ONSERVER
        {
            /// <summary>
            /// 设备外网IP
            /// </summary>
            [MarshalAsAttribute(UnmanagedType.ByValTStr, SizeConst = 32)]
            public string szDeviceIP;
            /// <summary>
            /// 设备外网通信端口
            /// </summary>
            public ushort nDevicePort;
            /// <summary>
            /// 设备在线状态 0不在线 1 在线
            /// </summary>
            public ushort nDeviceOnlie;
            /// <summary>
            /// 设备内网IP
            /// </summary>
            [MarshalAsAttribute(UnmanagedType.ByValTStr, SizeConst = 32)]
            public string szDeviceLocalIP;
            /// <summary>
            /// 设备内网端口
            /// </summary>
            public ushort nDeviceLocalPort;
            /// <summary>
            /// 设备所外网络类型  0内网 1外网
            /// </summary>
            public ushort nNetType;
            /// <summary>
            /// 设备固件版本号
            /// </summary>
            [MarshalAsAttribute(UnmanagedType.ByValTStr, SizeConst = 128)]
            public string szFirmWareVersion;
            /// <summary>
            /// 设备协议版本号
            /// </summary>
            [MarshalAsAttribute(UnmanagedType.ByValTStr, SizeConst = 128)]
            public string szProtocalVersion;
            /// <summary>
            /// 设备在线时长
            /// </summary>
            public Int32 tOnlineTime;
        }

        /// <summary>
        /// GPS信息
        /// </summary>
        [StructLayoutAttribute(LayoutKind.Sequential)]
        public struct ST_GPS_INFO
        {
            [MarshalAsAttribute(UnmanagedType.ByValTStr, SizeConst = 64)]
            public string szSampleTime;     //GPS采样时间
            [MarshalAsAttribute(UnmanagedType.ByValTStr, SizeConst = 64)]
            public string szDeviceID;       //设备ID
            [MarshalAsAttribute(UnmanagedType.ByValArray, SizeConst = 2, ArraySubType = UnmanagedType.I1)]
            public byte[] division;         //division[0] 'E' 东经 'W' 西经   division[1]  'N'北纬 'S' 南纬
            [MarshalAsAttribute(UnmanagedType.ByValArray, SizeConst = 2, ArraySubType = UnmanagedType.I1)]
            public byte[] reserve;          //保留
            public int longitude;           //经度  取值范围0~180*3600*100    转换公式为：longitude= 实际度*3600*100+实际分*60*100+实际秒*100
            public int latitude;            //纬度，取值范围为（0～90*3600*100），转换公式为：latitude = 实际度*3600*100+实际分*60*100+实际秒*100 
            public int direction;           //方向，取值范围为（0～359.9*100,-1），转换公式为：direction= 实际方向*100 
            public int speed;               //速度，取值范围为（0～999.9*100000），转换公式为：speed =实际速度*100000，相当于cm/h
        }

        /// <summary>
        /// 设备信息
        /// </summary>
        [StructLayoutAttribute(LayoutKind.Sequential)]
        public struct ST_DEVICE_NETINFO
        {
            [MarshalAsAttribute(UnmanagedType.ByValTStr, SizeConst = 32)]
            public string szAccessSrvIP;     /*接入务器IP*/
            public ushort nAccessSrvPort;   /*接入务器I端口*/
            public byte nNetLine;   /*设备所属网络线路编号，从1开始*/
            public byte nStreamEnCodeing;   /*保留转码标识*/
            [MarshalAsAttribute(UnmanagedType.ByValTStr, SizeConst = 64)]
            public string szDeviceID;   /*设备序列号*/
            public int nChan;   /*通道号从1开始*/
            public int nStreamType; /*流类型 0主码流 1子码流*/
            public int nTransPortType;  /*要求流媒体从前端取流的方式 0 UDP 1 TCP*/
        }

        public struct ST_ALARM_SERVER_INFO
        {
            [MarshalAsAttribute(UnmanagedType.ByValTStr, SizeConst = 32)]
            public string szAlarmServerIP;     /*报警嗠器IP*/
            public ushort nAlarmServerPort; //报警服务器端口
            public ushort nReserved;    //保留字段
            [MarshalAsAttribute(UnmanagedType.ByValTStr, SizeConst = 32)]
            public string szUserName;
            [MarshalAsAttribute(UnmanagedType.ByValTStr, SizeConst = 32)]
            public string szUserPwd;
        }

        public enum EN_PTZ_COMMAND
        {
            PTZ_CMD_ZOOM_IN = 11, 		/**< 焦距变大(倍率变大) */
            PTZ_CMD_ZOOM_OUT, 			/**< 焦距变小(倍率变小) */
            PTZ_CMD_FOCUS_IN, 			/**< 焦点前调 */
            PTZ_CMD_FOCUS_OUT, 			/**< 焦点后调 */
            PTZ_CMD_IRIS_ENLARGE,	 	/**< 光圈扩大 */
            PTZ_CMD_IRIS_SHRINK, 		/**< 光圈缩小 */
            PTZ_CMD_TILT_UP = 21, 		/**< 云台向上 */
            PTZ_CMD_TILT_DOWN,			/**< 云台向下 */
            PTZ_CMD_PAN_LEFT, 			/**< 云台左转 */
            PTZ_CMD_PAN_RIGHT, 			/**< 云台右转 */
            PTZ_CMD_UP_LEFT,			/**< 云台左上 */
            PTZ_CMD_UP_RIGHT,			/**< 云台右上 */
            PTZ_CMD_DOWN_LEFT,			/**< 云台左下 */
            PTZ_CMD_DOWN_RIGHT,			/**< 云台右下 */
            PTZ_CMD_SET_PRESET = 31,	/**< 设置预置点 */
            PTZ_CMD_CLE_PRESET,			/**< 清除预置点 */
            PTZ_CMD_GOTO_PRESET,		/**< 转到预置点 */
            PTZ_CMD_LIGHT_PWRON = 41,	/**< 灯光开关 */
            PTZ_CMD_WIPER_PWRON,		/**< 雨刷开关 */
            PTZ_CMD_PAN_AUTO,           /**< 自动巡航 */
        }

        public enum EN_CONN_MODE
        {
            CONN_MODE_P2P = 0,      /**< 点对点 */
            CONN_MODE_TRANSFER,		/**< 通过服务器中转 */
        }

        /**	@enum SEEK_TYPE
        *	@brief (文件)偏移类型
        */
        public enum SEEK_TYPE
        {
            SEEK_BY_SIZE = 1,           /**< 偏移文件长度 BYSIZE */
            SEEK_BY_TIME = 2,           /**< 偏移时间坐标 BYTIME */
        }

        /**	@enum EN_VOD_CONTROL_CMD
        *	@brief vod控制命令
        */
        public enum EN_VOD_CONTROL_CMD
        {
            VOD_CONTROL_CMD_GETPOS = 3,     /**< 获取回放进度 */
            VOD_CONTROL_CMD_SETPOS,         /**< 设置回放进度 */
        }


        /**
        *@brief 下载时数据回调
        *@param iDownloadID 区分不同音视频流的ID
        *@param pFileData 指向数据流缓冲
        *@param nFileDataLen 流数据字节数
        *@param nUsrData 用户数据
        *@return void
        **/
        public delegate void pFileDownloadDataCallBack (int iDownloadID,  IntPtr pFileData, uint nFileDataLen, uint nUsrData );


        /*********************************************************
		Function:	pProgressNotifyCallBack
		Desc:		调用某些函数时的处理过程
		Input:	nUserData 用户数据 
				nReserved 保留
				Desc 当前步骤描述  
		Output:	
		Return:	void
		**********************************************************/
        public delegate void pProgressNotifyCallBack(uint nUserData, uint nReserved, string Desc);

        /*********************************************************
		Function:	pRSMAVDataCallBack
		Desc:		调用某些函数时的处理过程
		Input:	hSessionID      区分不同音视频流的ID  
				nUsrData        用户数据 
				iDataType       流数据类型(STREAM_DATATYPE_DATA:视频数据;STREAM_DATATYPE_HDR:视频头)   
                pAVData         指向流数据缓冲 
                nAVDataLen      流数据字节数 
		Return:	void
		**********************************************************/
        //public delegate void pRSMAVDataCallBack(int hSessionID, IntPtr nUsrData, int iDataType, IntPtr pAVData, uint nAVDataLen, IntPtr handle);
        public delegate void pRSMAVDataCallBack(int hSessionID, IntPtr nUsrData, int iDataType, IntPtr pAVData, uint nAVDataLen);

        /*********************************************************
		Function:	pAlarmCallBack
		Desc:		报警回调函数声明
		Input:	nUserDataForCallBack        用户数据ID  
				nReserved                   保留
				pDesc                       报警信息数据   
                dwDescLen                   报警信息长度
		Return:	void
		**********************************************************/
        public delegate void pAlarmCallBack(uint nUserDataForCallBack, uint nReserved, IntPtr pDesc, uint dwDescLen);

        /*********************************************************
		Function:	pNotifyCallBack
		Desc:		通知回调函数类型声明
		Input:	iNotifySubsribeID           订阅通知信息的返回句柄   
				pNotifyBuffer               通知信息数据 
				dwNotifySize                通知信息长度    
                nUserDataForCallBack        用户数据
                nReserved                   保留
		Return:	void
		**********************************************************/
        public delegate void pNotifyCallBack(int iNotifySubsribeID, IntPtr pNotifyBuffer, uint dwNotifySize, IntPtr nUserDataForCallBack, uint nReserved);

        /*********************************************************
        Function:	PPVConnectDeviceByACS
        Desc:		连接设备通过点对点方式连接设备。
        Input:	pszDeviceID             设备注册ID 
                stPassbyServer          设备注册的服务器 
                pfnProgressNotifyUser   过程回调函数指针 
                nUserData               用户数据,回调时传给回调函数  
        Output:	pConnMode               指向存放连接类型的地址,函数返回后该地址存放的是连接类型
        Return:	>=0作为标识被连接设备的ID,即设置的IP标识;<0表示失败
        **********************************************************/
        [DllImport(@"Hik7200\libPPVClient2.dll", CallingConvention = CallingConvention.Cdecl)]
        public static extern int PPVConnectDeviceByACS(string pszDeviceID, ST_ACCESS_SERVER_INFO stPassbyServer, pProgressNotifyCallBack pfnProgressNotifyUser, UInt32 nUserDataForCallBack, ref EN_CONN_MODE pConnMode);

        /*********************************************************
        Function:	PPVGetDeviceInfoOnServer
        Desc:		从服务器上获取设备状态。
        Input:	pszDeviceID             设备注册ID 
                stPassbyServer          设备注册的服务器 
        Output:	pSTDeviceStatus         设备信息
        Return:	0代表获取成功,<0表示失败
        **********************************************************/
        [DllImport(@"Hik7200\libPPVClient2.dll", CallingConvention = CallingConvention.Cdecl)]
        public static extern int PPVGetDeviceInfoOnServer(string pszDeviceID, ST_ACCESS_SERVER_INFO stPassbyServer, ref ST_DEVICEINFO_ONSERVER pSTDeviceStatus);

        /*********************************************************
        Function:	PPVDisConnectDevice
        Desc:		断开与设备的连接
        Input:	iDevice 设备连接标识(由PPVConnectDevice或者PPVConnectDeviceByACS得到)  
        Return:	void
        **********************************************************/
        [DllImport(@"Hik7200\libPPVClient2.dll", CallingConvention = CallingConvention.Cdecl)]
        public static extern void PPVDisConnectDevice(int iDevice);
        /*********************************************************
        Function:	PPVNotifySubscribe 
        Desc:		通知信息订阅,包括设备上下线
        Input:	stPassbyServer          注册服务器的信息
                pfnNotifyCallBack       接收Notify的回调函数 
                nUserDataForCallBack    用户参数
        Return:	>=代表订阅成功，返回句柄。<0代表失败
        **********************************************************/
        [DllImport(@"Hik7200\libPPVClient2.dll", CallingConvention = CallingConvention.Cdecl)]
        public static extern int PPVNotifySubscribe(ST_ACCESS_SERVER_INFO stPassbyServer, pNotifyCallBack pfnNotifyCallBack, IntPtr nUserDataForCallBack);

        [DllImport(@"Hik7200\libPPVClient2.dll", CallingConvention = CallingConvention.Cdecl)]
        //iNotifySubsribeID 订阅通知信息的返回句柄  
        /*********************************************************
        Function:	PPVNotifyUnSubscribe
        Desc:		取消通知信息订阅,包括设备上下线
        Input:	iNotifySubsribeID 订阅通知信息的返回句柄 
        Return:	void
        **********************************************************/
        public static extern void PPVNotifyUnSubscribe(int iNotifySubsribeID);
        /*********************************************************
        Function:	PPVPTZControl
        Desc:		云台控制。
        Input:	iDevice                         设备连接标识 
                iChan                           通道号,从1开始 
                iPTZCommand                     云台控制的命令,具体参考EN_PTZ_COMMAND 
                iAction                         云台启停,0-启动;1-停止 
                iSpeed                          云台运动的速度,取值为0～7 
        Return:	void
        **********************************************************/
        /// <summary>
        /// 云台控制。
        /// </summary>
        /// <param name="iDevice">设备连接标识 </param>
        /// <param name="iChan">通道号,从1开始 </param>
        /// <param name="iPTZCommand">云台控制的命令,具体参考EN_PTZ_COMMAND</param>
        /// <param name="iAction">云台启停,0-启动;1-停止</param>
        /// <param name="iSpeed">云台运动的速度,取值为0～7</param>
        [DllImport(@"Hik7200\libPPVClient2.dll", CallingConvention = CallingConvention.Cdecl)]
        public static extern void PPVPTZControl(int iDevice, int iChan, int iPTZCommand, int iAction, int iSpeed);

        /**
        *@brief 语音数据回调函数类型声明
        *@param iVoiceID 区分不同语音流的ID
        *@param pVoiceData 指向语音数据缓存
        *@param nVoiceDataLen 语音数据字节数
        *@param nFrameNumber 帧数
        *@param nUsrData 用户数据
        *@return void
        **/
        public delegate void pVoiceDataCallBack (int iVoiceID,  IntPtr pVoiceData, uint nVoiceDataLen, int nFrameNumber, uint nUsrData);


        /**
        *@brief 开始语音对讲或广播
        *@param pszDeviceID 设备注册ID
        *@param nVoiceChan 语音通道,从1开始
        *@param vtype 语音类型(0-对讲;1-广播)
        *@param stPassbyServer 设备注册的服务器
        *@param pfnVoiceDataCallBack 语音数据回调
        *@param nVoiceDataCBUsrData 用户数据
        *@return >=0代表语音标识;<0代表开启失败（-1其他错误,-2设备未准备好,-3设备语音通道正在使用）
        **/
        [DllImport(@"Hik7200\libPPVClient2.dll", CallingConvention = CallingConvention.Cdecl)]
        public static extern int PPVVoiceTalkStart(string pszDeviceID, int nVoiceChan, int vtype, ST_ACCESS_SERVER_INFO stPassbyServer,
        pVoiceDataCallBack pfnVoiceDataCallBack, uint nVoiceDataCBUsrData);

        /**
          *@brief 停止语音对讲
          *@param iVoiceID 语音标识(由PPVVoiceTalkStart得到)
          *@return void
          **/
        [DllImport(@"Hik7200\libPPVClient2.dll", CallingConvention = CallingConvention.Cdecl)]
        public static extern void PPVVoiceTalkStop(int iVoiceID);

        /**
*@brief 通过服务器查找设备录像或者图片文件
*@param iDevice 设备连接标识 
*@param iChan 通道号,从1开始
*@param iFileType 文件类型(0xff-全部（不包含图片类型，指全部录像类型）,0-定时录像,1-移动报警,2-报警触发,3-报警|动测,4-报警&动测,5-命令触发,6-手动录像,100-图片)
*@param stStart 开始日期时间
*@param stStop 结束日期时间
*@retval pArray 文件列表结构体指针,为输出的查询结果
*@retval iFileNum 最多查找文件个数,输入查询条目和输出真实查询条目
*@return =0代表成功;-1表示失败
**/
        [DllImport(@"Hik7200\libPPVClient2.dll", CallingConvention = CallingConvention.Cdecl)]
        public static extern int PPVFindFile(int iDevice, int iChan, int iFileType, ST_DATETIME stStart, ST_DATETIME stStop, ref ST_FINDFILE[] pArray , ref int iFileNum);


        /**
        *@brief 开始通过服务器下载设备上的文件
        *@param pszDeviceID 设备连接标识
        *@param iChan 通道号,从1开始
        *@param pszFileName 设备上的文件名 
        *@param nFileLen 文件长度
        *@param enumSeekType 录像偏移类型
        *@param uSeek 录像偏移量
        *@param stPassbyServer 设备注册的服务器
        *@param pDownCallBack 下载时数据回调
        *@param nDownCallBackUsrData 下载回调函数的用户数据
        *@return >=0代表成功返回值为下载数据流标识;否则表示失败
        **/
        [DllImport(@"Hik7200\libPPVClient2.dll", CallingConvention = CallingConvention.Cdecl)]
        public static extern int PPVStartDownloadThroughACS( string pszDeviceID, int iChan, string pszFileName, uint nFileLen, SEEK_TYPE enumSeekType, uint uSeek, ST_ACCESS_SERVER_INFO stPassbyServer, pFileDownloadDataCallBack pDownCallBack, uint nDownCallBackUsrData );


        /**
        *@brief 停止通过服务器下载设备上的文件
        *@param iDownloadID 下载数据流标识（由PPVStartDownloadThroughACS得到） 
        *@return void
        **/
        [DllImport(@"Hik7200\libPPVClient2.dll", CallingConvention = CallingConvention.Cdecl)]
        public static extern void PPVStopDownloadThroughACS(int iDownloadID);


        /**
        *@brief 开始历史视频浏览(TCP模式),按照文件名称
        *@param iDevice 设备连接标识 
        *@param iChannel 通道号,从1开始
        *@param pszFileName 设备上的文件名 
        *@param nFileLen 文件长度
        *@param stPassbyServer 设备注册的服务器PPVSServer
        *@param pfnAVDataCallBack 视频数据回调
        *@param nAVDataCBUsrData 用户数据
        *@param pfnProgressNotifyUser 通知回调函数
        *@param nProgressCBUsrData 通知用户数据
        *@return >=0代表成功返回值为数据流标识;否则表示失败
        **/
        [DllImport(@"Hik7200\libPPVClient2.dll", CallingConvention = CallingConvention.Cdecl)]
        public static extern int PPVPlayBackByFileNameStart(int iDevice, int iChannel, string pszFileName, uint nFileLen, ST_ACCESS_SERVER_INFO stPassbyServer, pAVDataCallBack pfnAVDataCallBack, uint nAVDataCBUsrData,
       pProgressNotifyCallBack pfnProgressNotifyUser, uint nProgressCBUsrData );

        /**
        *@brief 视频数据接收回调函数类型声明
        *@param iStreamID 区分不同音视频流的ID
        *@param iDataType 流数据类型:流头数据还是普通数据
        *@param pAVData 指向流数据缓冲
        *@param nAVDataLen 流数据字节数
        *@param nUsrData 用户数据
        *@param pNALU NALU数据
        *@param nNALU NALU数据长度
        *@param pRawRTP 包括rtp头部的原始rtp数据,可以为NULL
        *@param nRawRTP 原始rtp数据的长度,可以为0
        *@return void
        **/
        public delegate void pAVDataCallBack (int iStreamID, int iDataType, IntPtr pAVData,
           uint nAVDataLen, uint nUsrData, IntPtr pNALU, uint nNALU, IntPtr pRawRTP, uint nRawRTP );


        /**
*@brief 停止历史视频浏览(TCP模式)
*@param iVodStream 流标识(由PPVPlayBackByFileNameStart得到) 
*@return void
**/
        [DllImport(@"Hik7200\libPPVClient2.dll", CallingConvention = CallingConvention.Cdecl)]
        public static extern void PPVPlayBackStop(int iVodStream);

        /**
        *@brief 历史视频回放进度控制(TCP模式)
        *@param iVodStream 流标识(由PPVPlayBackByFileNameStart得到)
        *@param iControlCmd 回放控制命令, 参见EN_VOD_CONTROL_CMD
        *@param iParamIn 控制命令相关的参数 
        *@param iParamOut 拖放控制时如果(*iParamOut)为0，则代表按字节拖放；如果(*iParamOut)大于0，则代表按时间精确拖放
        *@retval  iParamOut 获取进度时表示控制返回结果
        *@return =0代表成功;-1表示失败
        **/
        [DllImport(@"Hik7200\libPPVClient2.dll", CallingConvention = CallingConvention.Cdecl)]
        public static extern int PPVPlayBackControl(int iVodStream, int iControlCmd, int iParamIn, ref int iParamOut);


        //初始化PPV
        [DllImport(@"Hik7200\libPPVClient2.dll", CallingConvention = CallingConvention.Cdecl)]
        public static extern int PPVInitLib();
        //销毁PPV库
        [DllImport(@"Hik7200\libPPVClient2.dll", CallingConvention = CallingConvention.Cdecl)]
        public static extern int PPVFiniLib();
        //初始化流媒体客户端库
        [DllImport(@"Hik7200\libRSMClient.dll")]
        public static extern int InitRSMClientLib();
        //卸载流媒体客户端库
        [DllImport(@"Hik7200\libRSMClient.dll")]
        public static extern int FiniRSMClientLib();
        /*********************************************************
        Function:	PlayRSMAVStreamByTCP
        Desc:		连接设备通过点对点方式连接设备。
        Input:	pszSrvIP                        服务端IP 
                nSrvPort                        服务端端口 
                pszPrivate                      私有串 
                pDeviceInfo                     指向要连接的设备信息 
                pRSMAVDataCallBack              获取到的视频数据回调函数指针 
                nUsrData                        与流相关的用户数据 
        Return:	0成功 -1失败
        **********************************************************/
        [DllImport(@"Hik7200\libRSMClient.dll")]
        public static extern int PlayRSMAVStreamByTCP(string pszSrvIP, ushort nSrvPort, StringBuilder pszPrivate,
            ref ST_DEVICE_NETINFO pDeviceInfo, pRSMAVDataCallBack pfn, IntPtr nUsrData);

        [DllImport(@"Hik7200\libRSMClient.dll")]
        public static extern int PlayRSMAVStream(string pszSrvIP, ushort nSrvPort, StringBuilder pszPrivate,
            ref ST_DEVICE_NETINFO pDeviceInfo, pRSMAVDataCallBack pfn, IntPtr nUsrData);

        /*********************************************************
        Function:	StopRSMAVStream
        Desc:		停止从服务器取流。
        Input:	hSessionID      流标识(通过PlayRSMAVStreamByTCP得到)  
                
        Return:	void
        **********************************************************/
        [DllImport(@"Hik7200\libRSMClient.dll")]
        public static extern void StopRSMAVStreamByTCP(int hSessionID);


        /*********************************************************
        Function:	            StopRSMAVStream
        Desc:		            停止从服务器取流。
        Input:	hSessionID      流标识(通过PlayRSMAVStreamByTCP得到)        
        Return:	void
        **********************************************************/
        [DllImport(@"Hik7200\libRSMClient.dll")]
        public static extern void StopRSMAVStream(int hSessionID);

        public const uint GW_OWNER = 4;
        public delegate bool EnumWindowsProc(IntPtr hWnd, IntPtr lParam);
        [DllImport("User32.dll", CharSet = CharSet.Auto)]
        public static extern bool EnumWindows(EnumWindowsProc lpEnumFunc, IntPtr lParam);


        /**	@enum EN_ALARM_TYPE_EX
        *	@brief 报警类型
*/
        public enum EN_ALARM_TYPE_EX
        {
            ALARM_TYPE_DISK_FULL = 0x00,        /**< 0、硬盘满 */
            ALARM_TYPE_DISK_WRERROR,            /**< 1、读写硬盘出错 */

            ALARM_TYPE_VIDEO_LOST = 0x05,       /**< 5、视频(信号)丢失 */
            ALARM_TYPE_EXTERNAL,                /**< 6、外部(信号量)报警 */
            ALARM_TYPE_VIDEO_COVERED,           /**< 7、视频遮盖 */
            ALARM_TYPE_MOTION,                  /**< 8、移动侦测 */
            ALARM_TYPE_STANDARD_NOTMATCH,       /**< 9、制式不匹配 */
            ALARM_TYPE_SPEEDLIMIT_EXCEED,       /**< 10、超速 */
            ALARM_TYPE_SOUNDLIMIT_EXCEED = 80,  /**< 80、声音分贝数超标 */
            ALARM_TYPE_ALL = 100,               /**< 所有报警类型 */
        }


        /**	@struct ST_ALARM_INFO
        *  @brief 报警信息
        * 
        */

        [StructLayoutAttribute(LayoutKind.Sequential)]
        public struct ST_ALARM_INFO
        {
            [MarshalAsAttribute(UnmanagedType.ByValTStr, SizeConst = 64)]
            public string szAlarmTime;//[64];  /**< 报警上传时间 格式2009-02-24 12:11:12 */
            [MarshalAsAttribute(UnmanagedType.ByValTStr, SizeConst = 64)]
            public string szDeviceID;//[64]; /**<设备注册ID*/
            public uint nAlarmType; /**< 报警类型 见EN_ALARM_TYPE枚举变量 */
            public uint nAlarmAction; /**< 报警动作 0:开始	1:停止 */
            public uint nVideoChannel;  /**<发生视频丢失报警、移动侦测报警、遮挡报警时的视频通道号，当报警类型为10时代表允许最大速度，为80时代表允许声音分贝数阀值*10*/
            public uint nAlarmInChannel;  /**<发生报警的报警输入通道号，当报警类型为10时代表实际速度，为80时代表采样声音分贝数*10*/
            public uint nDiskNumber;  /**<发生硬盘错或者硬盘满报警的硬盘编号*/
        }

        /**	@struct ST_DATETIME
        *  @brief 日期时间结构
        */
        [StructLayoutAttribute(LayoutKind.Sequential)]
        public struct ST_DATETIME
        {
            public int iYear;                  /**< 年 */
            public int iMonth;                 /**< 月 */
            public int iDay;                   /**< 日 */
            public char chHours;               /**< 时 */
            public char chMinutes;             /**< 分 */
            public char chSeconds;             /**< 秒 */
            public char chReserved;                /**< 保留 */
        }

        /**	@struct ST_FINDFILE
        *  @brief 历史文件查找信息
        */
        [StructLayoutAttribute(LayoutKind.Sequential)]
        public struct ST_FINDFILE
        {
            [MarshalAsAttribute(UnmanagedType.ByValTStr, SizeConst = 100)]
            public string sFileName;//[100];    /**< 文件名 */
            public ST_DATETIME struStartTime;  /**< 文件的开始时间 */
            public ST_DATETIME struStopTime;   /**< 文件的结束时间 */
            public int iFileSize;      /**< 文件的大小 */
            public int iFileType;      /**< 文件类型  */
    }
    



        // 初始化SDK（所有操作之前必须调用）
        [DllImport(@"Hik7200\libAlarmClient.dll", CharSet = CharSet.Auto, CallingConvention = CallingConvention.Cdecl)]
        public static extern int AlarmInitLib();

        //注销SDK（所有操作结束后记住调用）
        [DllImport(@"Hik7200\libAlarmClient.dll", CharSet = CharSet.Auto, CallingConvention = CallingConvention.Cdecl)]
        public static extern int AlarmFiniLib();

        /*********************************************************
        Function:	GPSSubscribe
        Desc:		订阅GPS。
        Input:	stAlarmServerInfo       报警服务器信息
                pfnAlarmCallBack        接受GPS的回调函数
                nUserDataForCallBack    用户参数
        Return:	>=0代表订阅成功返回句柄，-1代表失败
        **********************************************************/
        [DllImport(@"Hik7200\libAlarmClient.dll", CharSet = CharSet.Auto, CallingConvention = CallingConvention.Cdecl)]
        public static extern int GPSSubscribe(ST_ALARM_SERVER_INFO stAlarmServerInfo, pAlarmCallBack pfnAlarmCallBack, IntPtr nUserDataForCallBack);
        [DllImport(@"Hik7200\libAlarmClient.dll", CharSet = CharSet.Auto, CallingConvention = CallingConvention.Cdecl)]
        /// <summary>
        /// 退订GPS信息
        /// </summary>
        /// <param name="iAlarmUsrID">订阅GPS的返回句柄 </param>
        public static extern void GPSUnSubscribe(int iAlarmUsrID);



        /**
        *@brief 全部报警订阅
        *@param stAlarmServerInfo 报警服务器的信息
        *@param pfnAlarmCallBack 接收报警的回调函数
        *@param nUserDataForCallBack 接收报警的回调函数的用户数据
        *@return >=0代表订阅成功返回的句柄;-1代表失败
        **/
        [DllImport(@"Hik7200\libAlarmClient.dll", CharSet = CharSet.Auto, CallingConvention = CallingConvention.Cdecl)]
        public static extern int AlarmSubscribe(ST_ALARM_SERVER_INFO stAlarmServerInfo, pAlarmCallBack pfnAlarmCallBack, uint nUserDataForCallBack);




        /*********************************************************
        Function:	PlayM4_SetStreamOpenMode
        Desc:		停止TCP方式从服务器取流。
        Input:	hSessionID      流标识(通过PlayRSMAVStreamByTCP得到)  
                
        Return:	void
        **********************************************************/
        [DllImport(@"Hik7200\PlayCtrl.dll")]
        public static extern bool PlayM4_SetStreamOpenMode(Int32 nPort, UInt32 nMode);

        [DllImport(@"Hik7200\PlayCtrl.dll")]
        public static extern bool PlayM4_OpenStream(Int32 nPort, IntPtr pAVData, uint nSize, uint nBufPoolSize);

        [DllImport(@"Hik7200\PlayCtrl.dll")]
        public static extern bool PlayM4_Play(Int32 nPort, IntPtr handle);

        [DllImport(@"Hik7200\PlayCtrl.dll")]
        public static extern bool PlayM4_InputData(Int32 nPort, IntPtr pAVData, uint nSize);

        [DllImport(@"Hik7200\PlayCtrl.dll")]
        public static extern bool PlayM4_GetPort(ref Int32 nPort);

        [DllImport(@"Hik7200\PlayCtrl.dll")]
        public static extern bool PlayM4_FreePort(Int32 nPort);

        [DllImport(@"Hik7200\PlayCtrl.dll")]
        public static extern bool PlayM4_Stop(Int32 nPort);

        [DllImport(@"Hik7200\PlayCtrl.dll")]
        public static extern bool PlayM4_CloseStream(Int32 nPort);

        /*********************************************************
        Function:	PlayM4_GetPictureSize
        Desc:		抓拍获取图片大小。
        Input:	nPort       端口
                width       图片宽度
                height      图片高度
        Return:	void
        **********************************************************/
        [DllImport(@"Hik7200\PlayCtrl.dll")]
        public static extern bool PlayM4_GetPictureSize(Int32 nPort, ref Int32 width, ref Int32 height);

        [DllImport(@"Hik7200\PlayCtrl.dll")]
        public static extern bool PlayM4_GetBMP(Int32 nPort, Byte[] pBitmap, uint nBufSize, ref uint pBmpSize);

        [DllImport(@"Hik7200\PlayCtrl.dll")]
        public static extern int PlayM4_GetLastError(Int32 nPort);

        [DllImport(@"Hik7200\PlayCtrl.dll")]
        public static extern bool PlayM4_GetJPEG(Int32 nPort, Byte[] pJpeg, uint nBufSize, ref uint pBmpSize);

        [DllImport(@"Hik7200\PlayCtrl.dll")]
        public static extern bool PlayM4_Fast(Int32 nPort);

        [DllImport(@"Hik7200\PlayCtrl.dll")]
        public static extern bool PlayM4_Slow(Int32 nPort);

        [DllImport(@"Hik7200\PlayCtrl.dll")]
        public static extern bool PlayM4_ResetBuffer(Int32 nPort, UInt32 nBufType);


        /**	@enum EN_AUDIO_TYPE
         *  @brief 语音采集编码格式。
         *	每一项只能占用不同的二进制位
         */
        public enum EN_AUDIO_TYPE
        {
            AUDIO_TYPE_G711 = 0x0001,
            AUDIO_TYPE_G722 = 0x0002,
        };

        /**	@fn	typedef void (__stdcall * HIK_AudioDataCallBack(int nAudioType, void* pAudioBuf, unsigned int nAudioLen, int nUserData)
         *	@brief 用户语音数据回调函数定义
         *	@param[in] nAudioType 语音类型 EN_AUDIO_TYPE的值
         *	@param[in] pAudioBuf 语音数据
         *	@param[in] nAudioLen 语音长度
         *	@param[in] nUserData 用户数据
         *	@return	无
         *	public delegate void pNotifyCallBack(int iNotifySubsribeID, IntPtr pNotifyBuffer, uint dwNotifySize, IntPtr nUserDataForCallBack, uint nReserved);
         */
        public delegate void HIK_AudioDataCallBack (int nAudioType, IntPtr pAudioBuf, uint nAudioLen, int nUserData);

        /**	@fn	VOICEDIALOGUE_API	bool CALLBACK HIK_AUDIO_StartPlay(int nAudioType)
         *	@brief 开始语音数据的播放
         *	@param[in] nAudioType 语音数据类型 EN_AUDIO_TYPE 的值
         *	@return	播放是否成功
         */
        [DllImport(@"Hik7200\VoiceDialogue.dll")]
        public static extern bool HIK_AUDIO_StartPlay(int nAudioType);

        /**	@fn	VOICEDIALOGUE_API	bool CALLBACK HIK_AUDIO_StopPlay()
         *	@brief 停止语音对讲的数据播放
         *	@return	操作是否成功
         */
        [DllImport(@"Hik7200\VoiceDialogue.dll")]
        public static extern bool HIK_AUDIO_StopPlay();

        /**	@fn	VOICEDIALOGUE_API	bool CALLBACK HIK_AUDIO_IputAudioData(char* pAduioBuf, unsigned int nLen)
         *	@brief 插入需要播放的语音数据
         *	@param[in] pAduioBuf 语音数据
         *	@param[in] nLen 语音数据长度
         *	@return	操作是否成功
         */
        [DllImport(@"Hik7200\VoiceDialogue.dll")]
        public static extern bool HIK_AUDIO_IputAudioData(IntPtr pAduioBuf, uint nLen);

        /**	@fn	VOICEDIALOGUE_API	bool CALLBACK HIK_AUDIO_SetAduioVolume(WORD wVolume)
         *	@brief 设置语音播放的声音大小
         *	@param[in] wVolume 声音值
         *	@return	操作是否成功
         */
        [DllImport(@"Hik7200\VoiceDialogue.dll")]
        public static extern bool HIK_AUDIO_SetAduioVolume(UInt16 wVolume);


        /**	@fn	VOICEDIALOGUE_API	bool CALLBACK HIK_AUDIO_StartAudioIn(int nAudioType)
         *	@brief 开始语音采集
         *	@param[in] nAudioType 语音类型 可为EN_AUDIO_TYPE值得交集
         *	@return	操作是否成功
         */
        [DllImport(@"Hik7200\VoiceDialogue.dll")]
        public static extern bool HIK_AUDIO_StartAudioIn(int nAudioType);

        /**	@fn	VOICEDIALOGUE_API	bool CALLBACK HIK_AUDIO_StopAudioIn()
         *	@brief 停止语音采集
         *	@return	操作是否成功
         */
        [DllImport(@"Hik7200\VoiceDialogue.dll")]
        public static extern bool HIK_AUDIO_StopAudioIn();

        /**	@fn	VOICEDIALOGUE_API   bool CALLBACK HIK_AUDIO_SetAudioInCallBack(HIK_AudioDataCallBack pCbf, int nUserData)
         *	@brief 设置语音采集的用户回调函数和信息
         *	@param[in] pCbf 回调函数
         *	@param[in] nUserData 用户数据
         *	@return	操作是否成功
         */
        [DllImport(@"Hik7200\VoiceDialogue.dll")]
        public static extern bool HIK_AUDIO_SetAudioInCallBack(HIK_AudioDataCallBack pCbf, int nUserData);




        [System.Runtime.InteropServices.DllImport("user32.dll ")]
        public static extern int SetWindowLong(IntPtr hWnd, int nIndex, int wndproc);
        [System.Runtime.InteropServices.DllImport("user32.dll ")]
        public static extern int GetWindowLong(IntPtr hWnd, int nIndex);

        public const int GWL_STYLE = -16;
        public const int WS_DISABLED = 0x8000000;

        public static void SetControlEnabled(Control c, bool enabled)
        {
            if (enabled) { SetWindowLong(c.Handle, GWL_STYLE, (~WS_DISABLED) & GetWindowLong(c.Handle, GWL_STYLE)); } else { SetWindowLong(c.Handle, GWL_STYLE, WS_DISABLED + GetWindowLong(c.Handle, GWL_STYLE)); }
        }
    }
}
