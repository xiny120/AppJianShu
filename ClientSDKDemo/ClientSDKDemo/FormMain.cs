using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Windows.Forms;
using System.Xml;
using Hik_iVMS7200;
using static Hik_iVMS7200.HikSDK_iVMS7200;

namespace ClientSDKDemo
{
    public partial class FormMain : Form
    {
        private int m_iDevice;
        private int m_lPort;
        int m_iNetLine;
        int m_iChannelno;
        int m_lPlayHandle;
        int m_iTalkHandle;
        int m_iAlarmUsrID;
        int m_iGPSUsrID;
        int m_iDownloadID;
        int m_iPlayBackID;
        int m_iNofifyID;
        byte[] m_pHeader;
        uint m_iHeadlen;
        bool m_bManualRecord;
        bool m_bRecordFileOpen;
        bool m_bSound;
        bool m_bFindFile;
        FileStream m_fs;
        HikSDK_iVMS7200.ST_FINDFILE m_stRecordFile;
        public FormMain()
        {
            m_iNetLine = -1;
            m_iChannelno = 1;
            InitializeComponent();
        }

        private void label2_Click(object sender, EventArgs e)
        {

        }

        private void FormMain_Load(object sender, EventArgs e)
        {
            Hik_iVMS7200.HikSDK_iVMS7200.PPVInitLib();
            HikSDK_iVMS7200.AlarmInitLib();
            HikSDK_iVMS7200.InitRSMClientLib();
            Control.CheckForIllegalCrossThreadCalls = false;
            comboBox1.Text = "子码流";

        }

        private void button8_Click(object sender, EventArgs e)
        {
            HikSDK_iVMS7200.EN_CONN_MODE iConnModeTmp = HikSDK_iVMS7200.EN_CONN_MODE.CONN_MODE_TRANSFER;  //Connect to the device through the server
            HikSDK_iVMS7200.ST_ACCESS_SERVER_INFO stPassbyServer = new HikSDK_iVMS7200.ST_ACCESS_SERVER_INFO();       //Information of Register server
            stPassbyServer.szAcessServerIP = textBox2.Text;
            stPassbyServer.nAcessServerPort = Convert.ToUInt16(textBox3.Text);
            stPassbyServer.szUserName = "admin";
            stPassbyServer.szUserPwd = "12345";
            m_iDevice = HikSDK_iVMS7200.PPVConnectDeviceByACS(textBox1.Text, stPassbyServer, ConnectProgressNotify, 0, ref iConnModeTmp);

        }

        void ConnectProgressNotify(uint nUserData, uint nReserved, string Desc)
        {
            textBox10.Text = Desc;
        }

        pRSMAVDataCallBack pRSMAVDataCallBack_;
        private void button9_Click(object sender, EventArgs e)
        {
            //OnBnClickedButtonStoppreview();
            //OnBnClickedBtnStopplayback();

            if (m_iDevice < 0)
            {
                return;
            }

            if (!HikSDK_iVMS7200.PlayM4_GetPort(ref m_lPort))
            {
                return;
            }

            //m_iNetLine default is 1
            if (m_iNetLine < 0 || m_iNetLine > 3)
            {
                m_iNetLine = 1;
            }

            HikSDK_iVMS7200.ST_DEVICE_NETINFO stDeviceRSMrequestInfo = new HikSDK_iVMS7200.ST_DEVICE_NETINFO();
            stDeviceRSMrequestInfo.szAccessSrvIP = textBox2.Text;    //IP of Register server
            stDeviceRSMrequestInfo.nAccessSrvPort = Convert.ToUInt16(textBox3.Text);       //port of Register server
            stDeviceRSMrequestInfo.nChan = m_iChannelno;           //channel number
            stDeviceRSMrequestInfo.nNetLine = (byte)m_iNetLine;        //Network line(According to the actual situation)
            stDeviceRSMrequestInfo.nStreamType = comboBox1.SelectedIndex;     //Stream type:0 Main stream ; 1:Sub stream
            stDeviceRSMrequestInfo.nTransPortType = 1;   //The way of Stream media server to access stream :0 UDP ; 1TCP
            stDeviceRSMrequestInfo.szDeviceID = textBox1.Text;       //The ID of device
            StringBuilder sb = new StringBuilder();
            pRSMAVDataCallBack_ = pRSMAVDataCallBack;
            m_lPlayHandle = HikSDK_iVMS7200.PlayRSMAVStreamByTCP(textBox4.Text, Convert.ToUInt16(textBox5.Text), sb, ref stDeviceRSMrequestInfo, pRSMAVDataCallBack_, (IntPtr)0);

            if (m_lPlayHandle < 0)
            {
                HikSDK_iVMS7200.PlayM4_FreePort(m_lPort);
                m_lPort = -1;
            }
            textBox10.Text = "开始预览...";
        }

        public void pRSMAVDataCallBack(int hSessionID, IntPtr nUsrData, int iDataType, IntPtr pAVData, uint nAVDataLen)
        {
            //you can add the code of dealing with stream data here (For example use player to play it or save it to mp4 file)
            try
            {
                if (HikSDK_iVMS7200.STREAM_DATATYPE_HDR == iDataType)     //The head of stream
                {
                    m_pHeader = new byte[nAVDataLen + 1];

                    //memcpy(pThis->m_pHeader, pAVData, nAVDataLen);
                    //pThis->m_pHeader[nAVDataLen] = '\0';
                    //pThis->m_iHeadlen = nAVDataLen;
                    Marshal.Copy(pAVData, m_pHeader, 0, (int)nAVDataLen);

                    HikSDK_iVMS7200.PlayM4_SetStreamOpenMode(m_lPort, HikSDK_iVMS7200.STREAME_REALTIME);
                    HikSDK_iVMS7200.PlayM4_OpenStream(m_lPort, pAVData, nAVDataLen, 1024 * 1024 * 5);
                    HikSDK_iVMS7200.PlayM4_Play(m_lPort, pictureBox1.Handle);
                }
                else if (HikSDK_iVMS7200.STREAM_DATATYPE_DATA == iDataType)       //The data of stream
                {
                    HikSDK_iVMS7200.PlayM4_InputData(m_lPort, pAVData, nAVDataLen);
                }

                if (m_bManualRecord)
                {
                    if (!m_bRecordFileOpen)
                    {
                        uint dwBufferSize = 0;
                        string filePath = Directory.GetCurrentDirectory() + "\\testManualRecord.mp4";
                        try
                        {
                            m_fs = new FileStream(filePath, FileMode.Create, FileAccess.ReadWrite);
                            //if (!pThis->m_RecordFile.Open(pThis->m_strManualRecordFilePath.GetBuffer(0),
                            //    CFile::modeCreate | CFile::modeReadWrite | CFile::shareDenyNone))
                        } catch (Exception e1)
                        {
                            m_bRecordFileOpen = false;
                            return;
                        }

                        m_bRecordFileOpen = true;
                        m_fs.Write(m_pHeader, 0, (int)m_iHeadlen);
                        //pThis->m_RecordFile.Write(pThis->m_pHeader, pThis->m_iHeadlen);
                    }

                    if (m_bRecordFileOpen)
                    {
                        byte[] b = new byte[nAVDataLen + 1];
                        Marshal.Copy(pAVData, b, 0, (int)nAVDataLen);
                        m_fs.Write(b, 0, (int)nAVDataLen);
                        //pThis->m_RecordFile.Write(pAVData, nAVDataLen);
                    }
                }
                return;
            }
            catch (Exception e1)
            {

            }
        }

        private void button10_Click(object sender, EventArgs e)
        {
            if (m_lPlayHandle >= 0)
            {
                HikSDK_iVMS7200.StopRSMAVStreamByTCP(m_lPlayHandle); //stop getting stream through RSM server by means of TCP
                m_lPlayHandle = -1;
            }

            if (m_lPort >= 0)
            {
                //Stop Decode 
                HikSDK_iVMS7200.PlayM4_Stop(m_lPort);
                HikSDK_iVMS7200.PlayM4_CloseStream(m_lPort);
                HikSDK_iVMS7200.PlayM4_FreePort(m_lPort);
                //GetDlgItem(IDC_STATIC_PLAYWND)->Invalidate();
                pictureBox1.Refresh();
                m_lPort = -1;
                m_bSound = false;
            }

            textBox10.Text = "停止预览...";
        }

        private void button11_Click(object sender, EventArgs e)
        {
            if (m_iDevice >= 0)
            {
                HikSDK_iVMS7200.PPVDisConnectDevice(m_iDevice);
                textBox10.Text = "断开连接...";
                m_iDevice = -1;
            }
        }

        private void button12_Click(object sender, EventArgs e)
        {
            if (m_iDevice < 0)
            {
                return;
            }

            int iAudioEncType = (int)HikSDK_iVMS7200.EN_AUDIO_TYPE.AUDIO_TYPE_G722;  //AUDIO_TYPE_G722
            HikSDK_iVMS7200.ST_ACCESS_SERVER_INFO stPassbyServer = new HikSDK_iVMS7200.ST_ACCESS_SERVER_INFO();       //Information of Register server
            stPassbyServer.szAcessServerIP = textBox2.Text;
            stPassbyServer.nAcessServerPort = Convert.ToUInt16(textBox3.Text);
            stPassbyServer.szUserName = "admin";
            stPassbyServer.szUserPwd = "12345";
            m_iTalkHandle = HikSDK_iVMS7200.PPVVoiceTalkStart(textBox1.Text, m_iChannelno, 0, stPassbyServer, pfnVoiceDataCallBack, 0);
            if (m_iTalkHandle >= 0)
            {
                if (!HikSDK_iVMS7200.HIK_AUDIO_StartAudioIn(iAudioEncType))
                {
                    textBox10.Text = "HIK_AUDIO_StartAudioIn Failed!";
                }

                if (!HikSDK_iVMS7200.HIK_AUDIO_StartPlay(iAudioEncType))
                {
                    textBox10.Text = "HIK_AUDIO_StartPlay Failed!";
                }
            }
        }

        public void pfnVoiceDataCallBack(int iVoiceID, IntPtr pVoiceData, uint nVoiceDataLen, int nFrameNumber, uint nUsrData)
        {
            if (iVoiceID >= 0)
            {
                HikSDK_iVMS7200.HIK_AUDIO_IputAudioData(pVoiceData, nVoiceDataLen);
            }
        }

        private void button13_Click(object sender, EventArgs e)
        {
            if (m_iTalkHandle >= 0)
            {
                HikSDK_iVMS7200.PPVVoiceTalkStop(m_iTalkHandle);
                m_iTalkHandle = -1;
            }

            if (!HikSDK_iVMS7200.HIK_AUDIO_StopPlay())
            {
                textBox10.Text = "Error: HIK_AUDIO_StopPlay is lost!";
            }

            if (!HikSDK_iVMS7200.HIK_AUDIO_StopAudioIn())
            {
                textBox10.Text = "Error: HIK_AUDIO_StopAudioIn is lost!";
            }
        }

        private void button14_Click(object sender, EventArgs e)
        {
            HikSDK_iVMS7200.ST_ALARM_SERVER_INFO stAlarmInfo = new HikSDK_iVMS7200.ST_ALARM_SERVER_INFO();         //Information of Alarm server
            stAlarmInfo.szAlarmServerIP = textBox6.Text;
            stAlarmInfo.nAlarmServerPort = Convert.ToUInt16(textBox7.Text);
            stAlarmInfo.szUserName = "admin";
            stAlarmInfo.szUserPwd = "12345";
            m_iAlarmUsrID = HikSDK_iVMS7200.AlarmSubscribe(stAlarmInfo, pfnAlarmCallBack, 0);
        }

        public void pfnAlarmCallBack(uint nUserDataForCallBack, uint nReserved, IntPtr pDesc, uint dwDescLen)
        {
            HikSDK_iVMS7200.ST_ALARM_INFO stAlarminfo = new HikSDK_iVMS7200.ST_ALARM_INFO();
            //var alarm = BytesToStruct(pDesc, stAlarminfo);
            var stAlarminfo1 = Marshal.PtrToStructure(pDesc, stAlarminfo.GetType());
            stAlarminfo = (HikSDK_iVMS7200.ST_ALARM_INFO)stAlarminfo1;
            textBox10.Text = "AlarmSubscribe" + stAlarminfo.szAlarmTime + " " + stAlarminfo.szDeviceID;

            //memcpy(&stAlarminfo, pDesc, sizeof(ST_ALARM_INFO)); //Infomation of Alarm
            /*
            size_t dwDescLenTmp = dwDescLen;

            ofstream outfile;   //write the information of alarm to file
            string strTemp = "--Alarm--:";
            outfile.open(".\\alarm.txt", ofstream::out | ofstream::app);
            if (outfile)
            {
                outfile << strTemp.c_str() << " Alarm time" << stAlarminfo.szAlarmTime << " Equipment serial number" << stAlarminfo.szDeviceID << " Alarm types" << stAlarminfo.nAlarmType << " Alarm action" << stAlarminfo.nAlarmAction
                    << " Alarm video channel" << stAlarminfo.nVideoChannel << " Alarm input channel number" << stAlarminfo.nAlarmInChannel << " Alarm hard disk serial number" << stAlarminfo.nDiskNumber << endl;
                outfile.close();
            }
            */
        }

        //byte[]转换为struct
        public static object BytesToStruct(byte[] bytes, Type type)
        {
            int size = Marshal.SizeOf(type);
            IntPtr buffer = Marshal.AllocHGlobal(size);
            try
            {
                Marshal.Copy(bytes, 0, buffer, size);
                return Marshal.PtrToStructure(buffer, type);
            }
            finally
            {
                Marshal.FreeHGlobal(buffer);
            }
        }

        private void button15_Click(object sender, EventArgs e)
        {
            HikSDK_iVMS7200.ST_ALARM_SERVER_INFO stAlarmInfo = new HikSDK_iVMS7200.ST_ALARM_SERVER_INFO();         //Information of Alarm server
            stAlarmInfo.szAlarmServerIP = textBox6.Text;
            stAlarmInfo.nAlarmServerPort = Convert.ToUInt16(textBox7.Text);
            stAlarmInfo.szUserName = "admin";
            stAlarmInfo.szUserPwd = "12345";
            m_iGPSUsrID = HikSDK_iVMS7200.GPSSubscribe(stAlarmInfo, pfnGPSCallBack, (IntPtr)0);
        }


        public void pfnGPSCallBack(uint nUserDataForCallBack, uint nReserved, IntPtr pDesc, uint dwDescLen)
        {
            HikSDK_iVMS7200.ST_ALARM_INFO stAlarminfo = new HikSDK_iVMS7200.ST_ALARM_INFO();
            //var alarm = BytesToStruct(pDesc, stAlarminfo);
            var stAlarminfo1 = Marshal.PtrToStructure(pDesc, stAlarminfo.GetType());
            stAlarminfo = (HikSDK_iVMS7200.ST_ALARM_INFO)stAlarminfo1;
            textBox10.Text = "GPSSubscribe" + stAlarminfo.szAlarmTime + " " + stAlarminfo.szDeviceID;

            //memcpy(&stAlarminfo, pDesc, sizeof(ST_ALARM_INFO)); //Infomation of Alarm
            /*
            size_t dwDescLenTmp = dwDescLen;

            ofstream outfile;   //write the information of alarm to file
            string strTemp = "--Alarm--:";
            outfile.open(".\\alarm.txt", ofstream::out | ofstream::app);
            if (outfile)
            {
                outfile << strTemp.c_str() << " Alarm time" << stAlarminfo.szAlarmTime << " Equipment serial number" << stAlarminfo.szDeviceID << " Alarm types" << stAlarminfo.nAlarmType << " Alarm action" << stAlarminfo.nAlarmAction
                    << " Alarm video channel" << stAlarminfo.nVideoChannel << " Alarm input channel number" << stAlarminfo.nAlarmInChannel << " Alarm hard disk serial number" << stAlarminfo.nDiskNumber << endl;
                outfile.close();
            }
            */
        }

        private void button16_Click(object sender, EventArgs e)
        {
            if (m_iDevice < 0)
            {
                return;
            }
            DateTime now = DateTime.Now;
            HikSDK_iVMS7200.ST_DATETIME stStart = new HikSDK_iVMS7200.ST_DATETIME(), stStop = new HikSDK_iVMS7200.ST_DATETIME();
            stStart.iYear = now.Year;
            stStart.iMonth = now.Month;
            stStart.iDay = now.Day;
            stStart.chHours = (char)0;
            stStart.chMinutes = (char)0;
            stStart.chSeconds = (char)0;

            stStop.iYear = now.Year;
            stStop.iMonth = now.Month;
            stStop.iDay = now.Day;
            stStop.chHours = (char)23;
            stStop.chMinutes = (char)59;
            stStop.chSeconds = (char)59;

            int iNum = 10;
            HikSDK_iVMS7200.ST_FINDFILE[] struFindFile = new HikSDK_iVMS7200.ST_FINDFILE[10];

            if (0 == HikSDK_iVMS7200.PPVFindFile(m_iDevice, m_iChannelno, 0xff, stStart, stStop, ref struFindFile, ref iNum) && iNum > 0)
            {
                m_bFindFile = true;
                //HikSDK_iVMS7200.ST_FINDFILE stAlarminfo = new HikSDK_iVMS7200.ST_FINDFILE();
                // var stAlarminfo1 = Marshal.PtrToStructure(struFindFile, stAlarminfo.GetType());
                //stAlarminfo = (HikSDK_iVMS7200.ST_ALARM_INFO)stAlarminfo1;
                StringBuilder sb = new StringBuilder();
                m_stRecordFile = struFindFile[0];
                //memcpy(&m_stRecordFile, &struFindFile[0], sizeof ST_FINDFILE);
                //strFile0.Format("RecordName%s FileSize:%d Total:%d\r\n", struFindFile[0].sFileName, struFindFile[0].iFileSize, iNum);
                //AfxMessageBox(strFile0);
                sb.AppendFormat("RecordName {0} FileSize:{1} Total:{2}\r\n", struFindFile[0].sFileName, struFindFile[0].iFileSize, iNum);
                MessageBox.Show(sb.ToString());
            }
            else
            {
                m_bFindFile = false;
                MessageBox.Show("PPVFindFile Failed");
            }
        }

        private void button17_Click(object sender, EventArgs e)
        {
            //OnBnClickedBtnStopdownload();
            button18_Click(null, null);

            if (m_iDevice < 0)
            {
                return;
            }

            //if (!CheckFolderExist(m_strDownloadFilePath.GetBuffer(0)))
            //{
            //    return;
            //}

            HikSDK_iVMS7200.ST_ACCESS_SERVER_INFO stPassbyServer = new HikSDK_iVMS7200.ST_ACCESS_SERVER_INFO();//Information of Register server
            stPassbyServer.szAcessServerIP = textBox2.Text;
            stPassbyServer.nAcessServerPort = Convert.ToUInt16(textBox3.Text);
            stPassbyServer.szUserName = "admin";
            stPassbyServer.szUserPwd = "12345";

            //PPVStartDownloadThroughACS/PPVStartDownloadThroughACSByTime
            m_iDownloadID = HikSDK_iVMS7200.PPVStartDownloadThroughACS(textBox1.Text,
                m_iChannelno,
                m_stRecordFile.sFileName,
                (uint)m_stRecordFile.iFileSize,
                HikSDK_iVMS7200.SEEK_TYPE.SEEK_BY_SIZE,
                0,
                stPassbyServer,
                pfnFileDownloadDataCallBack,
                0
        );

            if (m_iDownloadID < 0)
            {
                StringBuilder sb = new StringBuilder();

                sb.AppendFormat("PPVStartDownloadThroughACS Failed,code:{0}", m_iDownloadID);
                MessageBox.Show(sb.ToString());
            }
        }

        public void pfnFileDownloadDataCallBack(int iDownloadID, IntPtr pFileData, uint nFileDataLen, uint nUsrData)
        {
            FileStream fs = null;
            bool bOpen = false;
            string filePath = Directory.GetCurrentDirectory() + "\\testdownload.mp4";
            if (!File.Exists(filePath)) // (_access(m_strDownloadFilePath.GetBuffer(0), 0) == -1)
            {
                //bOpen = file.Open(pDlg->m_strDownloadFilePath.GetBuffer(0), CFile::modeCreate | CFile::modeWrite | CFile::typeBinary);
                try
                {
                    fs = new FileStream(filePath, FileMode.CreateNew, FileAccess.ReadWrite);
                    bOpen = true;
                }
                catch (Exception e1)
                {

                }
            }
            else
            {
                //bOpen = file.Open(pDlg->m_strDownloadFilePath.GetBuffer(0), CFile::modeWrite | CFile::typeBinary);
                try
                {
                    fs = new FileStream(filePath, FileMode.Open, FileAccess.ReadWrite);
                    bOpen = true;
                }
                catch (Exception e1)
                {

                }
            }

            if (!bOpen)
            {
                return;
            }
            else if (fs != null)
            {
                //file.SeekToEnd();
                //file.Write((const char*)pFileData, nFileDataLen );
                //file.Close();
                byte[] w = new byte[nFileDataLen];
                Marshal.Copy(pFileData, w, 0, (int)nFileDataLen);
                fs.Seek(0, SeekOrigin.End);
                fs.Write(w, 0, (int)nFileDataLen);
                fs.Close();
            }
        }

        private void button18_Click(object sender, EventArgs e)
        {
            // TODO: Add your control notification handler code here
            if (m_iDownloadID < 0)
            {
                return;
            }

            HikSDK_iVMS7200.PPVStopDownloadThroughACS(m_iDownloadID);
            m_iDownloadID = -1;
        }

        private void button19_Click(object sender, EventArgs e)
        {

            //OnBnClickedButtonStoppreview();
            //OnBnClickedBtnStopplayback();
            button10_Click(null, null);
            button20_Click(null, null);

            if (m_iDevice < 0 || !m_bFindFile)
            {
                return;
            }

            if (!HikSDK_iVMS7200.PlayM4_GetPort(ref m_lPort))
            {
                return;
            }

            HikSDK_iVMS7200.ST_ACCESS_SERVER_INFO stPassbyServer = new HikSDK_iVMS7200.ST_ACCESS_SERVER_INFO();       //Information of Register server
            stPassbyServer.szAcessServerIP = textBox2.Text;
            stPassbyServer.nAcessServerPort = Convert.ToUInt16(textBox3.Text);
            stPassbyServer.szUserName = "admin";
            stPassbyServer.szUserPwd = "12345";

            m_iPlayBackID = HikSDK_iVMS7200.PPVPlayBackByFileNameStart(m_iDevice,
                m_iChannelno,
                m_stRecordFile.sFileName,
                (uint)m_stRecordFile.iFileSize,
                stPassbyServer,
                pfnFileDataCallBack,
                0,
                null,
                0);

            if (m_iPlayBackID < 0)
            {
                HikSDK_iVMS7200.PlayM4_FreePort(m_lPort);
                m_lPort = -1;
            }
        }

        public void pfnFileDataCallBack(int iStreamID, int iDataType, IntPtr pAVData,
           uint nAVDataLen, uint nUsrData, IntPtr pNALU, uint nNALU, IntPtr pRawRTP, uint nRawRTP){
        }

        private void button20_Click(object sender, EventArgs e)
        {
            if (m_iPlayBackID >= 0)
            {
                HikSDK_iVMS7200.PPVPlayBackStop(m_lPlayHandle);
                m_iPlayBackID = -1;
            }

            if (m_lPort >= 0)
            {
                //Stop Decode 
                HikSDK_iVMS7200.PlayM4_Stop(m_lPort);
                HikSDK_iVMS7200.PlayM4_CloseStream(m_lPort);
                HikSDK_iVMS7200.PlayM4_FreePort(m_lPort);
                //GetDlgItem(IDC_STATIC_PLAYWND)->Invalidate();
                pictureBox1.Refresh();
                m_lPort = -1;
                m_bSound = false;
            }
        }

        private void button21_Click(object sender, EventArgs e)
        {
            if (m_iPlayBackID >= 0 && m_lPort >= 0)
            {
                HikSDK_iVMS7200.PlayM4_Slow(m_lPort);
            }
        }

        private void button22_Click(object sender, EventArgs e)
        {
            if (m_iPlayBackID >= 0 && m_lPort >= 0)
            {
                HikSDK_iVMS7200.PlayM4_Fast(m_lPort);
            }
        }

        private void button23_Click(object sender, EventArgs e)
        {
            if (m_iPlayBackID >= 0 && m_lPort >= 0)
            {
                if (!HikSDK_iVMS7200.PlayM4_ResetBuffer(m_lPort, 1))
                {
                    return;
                }

                int iResult = 0;
                if (HikSDK_iVMS7200.PPVPlayBackControl(m_iPlayBackID, (int)HikSDK_iVMS7200.EN_VOD_CONTROL_CMD.VOD_CONTROL_CMD_SETPOS, 50, ref iResult) < 0)
                {
                    return;
                }
            }
        }

        private void button25_Click(object sender, EventArgs e)
        {
            if (m_iNofifyID >= 0)
            {
                HikSDK_iVMS7200.PPVNotifyUnSubscribe(m_iNofifyID);
                m_iNofifyID = -1;
            }
        }

        pNotifyCallBack pNotifyCallBack_;
        private void button24_Click(object sender, EventArgs e)
        {
            button25_Click(null, null);
            HikSDK_iVMS7200.ST_ACCESS_SERVER_INFO stPassbyServer = new HikSDK_iVMS7200.ST_ACCESS_SERVER_INFO();       //Information of Register server
            stPassbyServer.szAcessServerIP = textBox2.Text;
            stPassbyServer.nAcessServerPort = Convert.ToUInt16(textBox3.Text);
            stPassbyServer.szUserName = "admin";
            stPassbyServer.szUserPwd = "12345";
            pNotifyCallBack_ = pfnNotifyCallBack;
            m_iNofifyID = HikSDK_iVMS7200.PPVNotifySubscribe(stPassbyServer, pNotifyCallBack_, (IntPtr)0);
            
            if (m_iNofifyID < 0)
            {
                MessageBox.Show("NotifySubscribe Failed!");
            }
        }



        public void pfnNotifyCallBack(int iNotifySubsribeID, IntPtr pNotifyBuffer, uint dwNotifySize, IntPtr nUserDataForCallBack, uint nReserved)
        {
            if (null == pNotifyBuffer || 0 == dwNotifySize)
            {
                return;
            }

            if (m_iNofifyID == iNotifySubsribeID)
            {
                string text = Marshal.PtrToStringAnsi(pNotifyBuffer);
                XmlDocument xmldoc = new XmlDocument();
                xmldoc.LoadXml(text);
                XmlNode root = xmldoc.SelectSingleNode("Notify");//查找<Employees>
                if(root.Attributes["Type"].Value == "DeviceInfo")
                {
                    XmlNode deviceInfo = root.SelectSingleNode("DeviceID");
                    string strDevAccount = deviceInfo.InnerText;
                    XmlNode onoffLine = root.SelectSingleNode("OnOffLine");
                    string strStatus = onoffLine.InnerText;
                    StringBuilder sb = new StringBuilder("UNKOWN");
                    if(strStatus == "On")
                    {
                        sb.Clear();
                        sb.AppendFormat("ID:{0} on-line", strDevAccount);
                    }
                    else
                    {
                        sb.Clear();
                        sb.AppendFormat("ID:{0} off-line", strDevAccount);
                    }
                    textBox10.Text = sb.ToString();
                    sb = null;
                }
                xmldoc.RemoveAll();
                xmldoc = null;

            }
        }

        private void button26_Click(object sender, EventArgs e)
        {
            HikSDK_iVMS7200.ST_ACCESS_SERVER_INFO stPassbyServer = new HikSDK_iVMS7200.ST_ACCESS_SERVER_INFO();       //Information of Register server
            stPassbyServer.szAcessServerIP = textBox2.Text;
            stPassbyServer.nAcessServerPort = Convert.ToUInt16(textBox3.Text);
            stPassbyServer.szUserName = "admin";
            stPassbyServer.szUserPwd = "12345";
            HikSDK_iVMS7200.ST_DEVICEINFO_ONSERVER stDeviceStatus = new HikSDK_iVMS7200.ST_DEVICEINFO_ONSERVER();
            HikSDK_iVMS7200.PPVGetDeviceInfoOnServer(textBox1.Text, stPassbyServer, ref stDeviceStatus);
        }

        private void button27_Click(object sender, EventArgs e)
        {
            if (m_iDevice < 0)
            {
                return;
            }

            StringBuilder sb = new StringBuilder();
            uint iSize = 10 * 1024;
            HikSDK_iVMS7200.PPVGetDeviceWorkStatus(m_iDevice, sb, ref iSize);
        }

        private void button1_Click(object sender, EventArgs e)
        {
            if ((m_lPlayHandle >= 0 || m_iPlayBackID >= 0) && m_lPort >= 0)
            {
                
                if (m_bSound)
                {
                    HikSDK_iVMS7200.PlayM4_StopSound();
                    m_bSound = false;
                    button1.Text = "打开声音";

                }
                else
                {
                    if (HikSDK_iVMS7200.PlayM4_PlaySound(m_lPort))
                    {
                        m_bSound = true;
                        button1.Text = "关闭声音";
                    }
                }
                
            }
        }

        private void button2_Click(object sender, EventArgs e)
        {
            // TODO: Add your control notification handler code here
            //if (!CheckFolderExist(m_strCaptureFilePath.GetBuffer(0)))
            //{
            //    return;
            //}

            if ((m_lPlayHandle >= 0 || m_iPlayBackID >= 0) && m_lPort >= 0)
            {
                CapturePicture("test.jpg");
            }
        }



        private bool CapturePicture(String strSnapshotSavePath)
        {
            bool bFlag = false;
            byte [] pBmp = null;
            try
            {
                if (m_lPort < 0)
                {
                    return bFlag;
                }

                int dwHeight = 0, dwWidth = 0;
                if (!HikSDK_iVMS7200.PlayM4_GetPictureSize(m_lPort, ref dwHeight, ref dwWidth))
                {
                    StringBuilder sb = new StringBuilder();
                    sb.AppendFormat("Error Info:PlayM4_GetPictureSize Failed, error code:{0}!\r\n", HikSDK_iVMS7200.PlayM4_GetLastError(m_lPort));
                    MessageBox.Show(sb.ToString());

                    return bFlag;
                }

                uint dwPicSize = 1024 * 1024 * 3;

                pBmp = new byte[dwPicSize];
                uint dwBmpSize = 0;

                bool bGet = HikSDK_iVMS7200.PlayM4_GetBMP(m_lPort, pBmp, dwPicSize, ref dwBmpSize);//PlayM4_GetJPEG or PlayM4_GetBMP(in Playctrl.dll)
                if (bGet)
                {
                    //CFile file;
                    //CFileException exError;
                    //BOOL bOpen = file.Open(strSnapshotSavePath.GetBuffer(0), CFile::modeCreate | CFile::modeWrite, &exError);
                    try
                    {
                        string filePath = Directory.GetCurrentDirectory() + "\\testcapture.jpg";
                        FileStream fs = new FileStream(filePath, FileMode.Create, FileAccess.ReadWrite);
                        fs.Write(pBmp,0, (int)dwBmpSize);
                        fs.Close();
                        bFlag = true;
                    }
                    catch(Exception e1)
                    {
                        MessageBox.Show("PlayM4_GetBMP Open file failed");
                    }
                }
                else
                {
                    StringBuilder sb = new StringBuilder();
                    sb.AppendFormat("Error Info:PlayM4_GetPictureSize Failed, error code:{0}!\r\n", HikSDK_iVMS7200.PlayM4_GetLastError(m_lPort));
                    MessageBox.Show(sb.ToString());
                }
            }
            catch (Exception ex)
            {
                bFlag = false;
            }

            return bFlag;
        }

        private void button3_Click(object sender, EventArgs e)
        {
            if (m_lPlayHandle >= 0)
            {
                
                if (!m_bManualRecord)
                {
                    if (m_bRecordFileOpen)
                    {
                        m_fs.Close();
                        m_bRecordFileOpen = false;
                    }
                    //DeleteFile(m_strManualRecordFilePath.GetBuffer(0));
                    button3.Text = "停止录像";
                }
                else
                {
                    if (m_bRecordFileOpen)
                    {
                        m_fs.Close();
                        m_bRecordFileOpen = false;
                    }
                    button3.Text = "手动录像";
                }

                m_bManualRecord = !m_bManualRecord;
                
            }
        }

        private void button4_Click(object sender, EventArgs e)
        {
            if (m_lPlayHandle < 0 || m_iDevice < 0)
            {
                return;
            }

            //PTZ control must operate in real-time preview condition
            int lSpeed = 3;
            HikSDK_iVMS7200.PPVPTZControl(m_iDevice, m_iChannelno, (int)HikSDK_iVMS7200.EN_PTZ_COMMAND.PTZ_CMD_TILT_UP, 0, lSpeed);//start
            Thread.Sleep(100);
            HikSDK_iVMS7200.PPVPTZControl(m_iDevice, m_iChannelno, (int)HikSDK_iVMS7200.EN_PTZ_COMMAND.PTZ_CMD_TILT_UP, 1, lSpeed);//stop
        }

        private void button5_Click(object sender, EventArgs e)
        {
            if (m_lPlayHandle < 0 || m_iDevice < 0)
            {
                return;
            }

            //PTZ control must operate in real-time preview condition
            int lSpeed = 3;
            HikSDK_iVMS7200.PPVPTZControl(m_iDevice, m_iChannelno, (int)HikSDK_iVMS7200.EN_PTZ_COMMAND.PTZ_CMD_PAN_RIGHT, 0, lSpeed);//start
            Thread.Sleep(100);
            HikSDK_iVMS7200.PPVPTZControl(m_iDevice, m_iChannelno, (int)HikSDK_iVMS7200.EN_PTZ_COMMAND.PTZ_CMD_PAN_RIGHT, 1, lSpeed);//stop
        }

        private void button6_Click(object sender, EventArgs e)
        {
            if (m_lPlayHandle < 0 || m_iDevice < 0)
            {
                return;
            }

            //PTZ control must operate in real-time preview condition
            int lSpeed = 3;
            HikSDK_iVMS7200.PPVPTZControl(m_iDevice, m_iChannelno, (int)HikSDK_iVMS7200.EN_PTZ_COMMAND.PTZ_CMD_TILT_DOWN, 0, lSpeed);//start
            Thread.Sleep(100);
            HikSDK_iVMS7200.PPVPTZControl(m_iDevice, m_iChannelno, (int)HikSDK_iVMS7200.EN_PTZ_COMMAND.PTZ_CMD_TILT_DOWN, 1, lSpeed);//stop
        }

        private void button7_Click(object sender, EventArgs e)
        {
            if (m_lPlayHandle < 0 || m_iDevice < 0)
            {
                return;
            }

            //PTZ control must operate in real-time preview condition
            int lSpeed = 3;
            HikSDK_iVMS7200.PPVPTZControl(m_iDevice, m_iChannelno, (int)HikSDK_iVMS7200.EN_PTZ_COMMAND.PTZ_CMD_PAN_LEFT, 0, lSpeed);//start
            Thread.Sleep(100);
            HikSDK_iVMS7200.PPVPTZControl(m_iDevice, m_iChannelno, (int)HikSDK_iVMS7200.EN_PTZ_COMMAND.PTZ_CMD_PAN_LEFT, 1, lSpeed);//stop
        }
    }
}
