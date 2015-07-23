using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using System.Net;
using System.Threading;
using ControlCenter.Control;
using SFLib;
using System.IO;
using System.Collections.Specialized;
namespace ControlCenter
{
    public partial class Form1 : Form
    {
       

        private string[] _keepScript = new string[]
		{
			"scriptTemplate.lua",
			"TimeLineTemplate.lua",
			"TimeLine.lua",
			"RunTimeLine.lua",
			"TimeShaft.lua"
		};


       
        private TextBox txtLog;
        private NotifyIcon notifyIcon;
        private ContextMenuStrip cmsNotifyIcon;
        private ToolStripMenuItem 还原ToolStripMenuItem;
        private ToolStripMenuItem 退出ToolStripMenuItem;
        private HttpServer _httpServer = new HttpServer();
        private ClientReceiver _receiver = new ClientReceiver();
        private Listener _udpServer;
        private Listener _comServer;
        private TcpScriptListener _tcpServer;

        public Form1()
        {
            InitializeComponent();
        }

        private void Form1_Load(object sender, EventArgs e)
        {
            Config.Load(AppDomain.CurrentDomain.BaseDirectory + "Config.ini");
            this._udpServer = new UDPListener(int.Parse(Config.Items["UdpPort"]));
            if (Config.Items["IsComEnable"] == "1")
            {
                this._comServer = new COMListener(Config.Items["ComPort"], Config.Items["ComNewLine"]);
            }
            this._tcpServer = new TcpScriptListener(int.Parse(Config.Items["TcpPort"]));
            this.InitLogger();
            this.StartServers();
        }

        private void InitLogger()
        {
            int num = 100;
            Logger.RegisterTextBoxListener(this.txtLog, num);
            Logger.RegisterConsoleListener();
            Logger.RegisterTextWriterListener();
          /*Logger.ExitApplication = (LoggerExitApplicationDelegate)Delegate.Combine(Logger.ExitApplication, delegate
            {
                base.Hide();
                this.DisposeIcon();
            });*/
        }

        private void StartServers()
        {
            this._receiver.Start();
            this._receiver.OnConfigUpdated += new EventHandler(this.UpdateConfig);
            this._receiver.OnTimeLineUpdated += new EventHandler(this.UpdateTimeConfig);
           // this._receiver.Dog = this._lockDog;
           
                if (Config.Items["Http"] == "1")
                {
                  //  this._httpServer.Start(new ScriptServer(Config.Items["ProjectName"]));
                }
                if (Config.Items["UDP"] == "1")
                {
                    this._udpServer.Start(Config.Items["ProjectName"]);
                }
                if (this._comServer != null)
                {
                    this._comServer.Start(Config.Items["ProjectName"]);
                }
                if (Config.Items["TCP"] == "1")
                {
                    this._tcpServer.Start(Config.Items["ProjectName"]);
                }
                string str = AppDomain.CurrentDomain.BaseDirectory + "Script\\";
                if (File.Exists(str + "TimeLine.lua"))
                {
                    
                }
            
        }


        //更新配置
        private void UpdateConfig(object sender, EventArgs e)
        {
            DirectoryInfo directoryInfo = new DirectoryInfo(AppDomain.CurrentDomain.BaseDirectory + "Script");
            FileInfo[] files = directoryInfo.GetFiles("*.lua");
            FileInfo[] array = files;
            for (int i = 0; i < array.Length; i++)
            {
                FileInfo fileInfo = array[i];
                if (!this._keepScript.Contains(fileInfo.Name))
                {
                    File.Delete(fileInfo.FullName);
                }
            }
            ConfigWriter.LoadConfig();
            ConfigWriter.LoadTimeShaft();
        }

        //更新时间配置
        private void UpdateTimeConfig(object sender, EventArgs e)
        {
            ConfigWriter.LoadTimeLineConfig();
        }

        //广播
        private void BroadcastServerAlive()
        {
            byte[] addressBytes = IPAddress.Parse(NetLib.GetLocalIpString()).GetAddressBytes();
            while (true)
            {
                if (!NetLib.BroadcastUdpData(int.Parse(Config.Items["BroadcastPort"]), addressBytes))
                {
                    Logger.Warning("Server Alive 广播发送失败!");
                }
                Thread.Sleep(1000);
            }
        }


        //窗口关闭
        private void MainForm_FormClosed(object sender, FormClosedEventArgs e)
        {
            Logger.Info("回收资源中，请稍候...");
            this._httpServer.Stop();
            this._udpServer.Stop();
            this._receiver.Stop();
            this.DisposeIcon();
        }


        internal void DisposeIcon()
        {
            if (this.notifyIcon != null)
            {
                this.notifyIcon.Visible = false;
                this.notifyIcon.Dispose();
            }
        }

        private void MainForm_Shown(object sender, EventArgs e)
        {
            base.Visible = false;
        }

        private void notifyIcon_DoubleClick(object sender, EventArgs e)
        {
            base.Show();
            base.WindowState = FormWindowState.Normal;
        }

        private void MainForm_SizeChanged(object sender, EventArgs e)
        {
            if (base.WindowState == FormWindowState.Minimized)
            {
                base.Hide();
            }
        }

        private void 还原ToolStripMenuItem_Click(object sender, EventArgs e)
        {
            base.Show();
            base.WindowState = FormWindowState.Normal;
        }

        private void 退出ToolStripMenuItem_Click(object sender, EventArgs e)
        {
            base.Close();
        }

        
       
    }
}
