using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Net;
using System.IO;
using System.Threading;
using SFLib;

namespace ControlCenter.Control
{
   internal class HttpServer
    {
        private Thread _httpListenThread;
        private volatile bool _isStarted = false;
        private volatile bool _terminated = false;
        private volatile bool _ready = false;
        private volatile bool _isRuning = false;
        private HttpImplanter _httpImplanter;

        internal HttpImplanter HttpImplanter
        {
            get
            {
                return this._httpImplanter;
            }
            set
            {
                this._httpImplanter = value;
            }
        }

        public void Start(HttpImplanter httpImplanter)
        {
            if (!HttpListener.IsSupported)
            {
                Logger.Exit("不支持HttpListener!");
            }
            if (!this._isStarted)
            {
                this._isStarted = true;
                this._ready = false;
                this._httpImplanter = httpImplanter;
                this.RunHttpServerThread();
                while (!this._ready)
                {
                }
            }
        }

        private void RunHttpServerThread()
        {
            this._httpListenThread = new Thread(new ThreadStart(()=>
            {
                HttpListener httpListener = new HttpListener();
                try
                {
                    this._httpImplanter.MakeHttpPrefix(httpListener);
                    httpListener.Start();
                }
                catch (Exception var_1_1F)
                {
                    Logger.Exit("无法启动服务器监听，请检查网络环境。");
                }
                this._httpImplanter.Start();
                IAsyncResult asyncResult = null;
                while (!this._terminated)
                {
                    while (asyncResult == null || asyncResult.IsCompleted)
                    {
                        asyncResult = httpListener.BeginGetContext(new AsyncCallback(this.ProcessHttpRequest), httpListener);
                    }
                    this._ready = true;
                    Thread.Sleep(10);
                }
                httpListener.Stop();
                httpListener.Abort();
                httpListener.Close();
                this._httpImplanter.Stop();            
            }));
            this._httpListenThread.IsBackground = true;
            this._httpListenThread.Start();
        }

        private void ProcessHttpRequest(IAsyncResult iaServer)
        {
            HttpListener httpListener = iaServer.AsyncState as HttpListener;
            HttpListenerContext httpListenerContext = null;
            try
            {
                httpListenerContext = httpListener.EndGetContext(iaServer);
                Logger.Info("接收请求" + httpListenerContext.Request.Url.ToString());
                if (this._isRuning)
                {
                    Logger.Info("正在处理请求，已忽略请求" + httpListenerContext.Request.Url.ToString());
                    HttpServer.RetutnResponse(httpListenerContext, this._httpImplanter.CreateReturnResult(httpListenerContext, new SFReturnCode(8, EnumHelper.GetEnumDescription(CommandResult.ServerIsBusy))));
                    httpListener.BeginGetContext(new AsyncCallback(this.ProcessHttpRequest), httpListener);
                    return;
                }
                this._isRuning = true;
                httpListener.BeginGetContext(new AsyncCallback(this.ProcessHttpRequest), httpListener);
            }
            catch
            {
                Logger.Warning("服务器已关闭！");
                return;
            }
            string scriptName = new UrlHelper(httpListenerContext.Request.Url).ScriptName;
            SFReturnCode result = this._httpImplanter.ProcessRequest(httpListenerContext);
            byte[] resultBytes = this._httpImplanter.CreateReturnResult(httpListenerContext, result);
            HttpServer.RetutnResponse(httpListenerContext, resultBytes);
            GC.Collect();
            this._isRuning = false;
        }

        private static void RetutnResponse(HttpListenerContext context, byte[] resultBytes)
        {
            context.Response.ContentLength64 = (long)resultBytes.Length;
            Stream outputStream = context.Response.OutputStream;
            try
            {
                outputStream.Write(resultBytes, 0, resultBytes.Length);
                outputStream.Close();
            }
            catch
            {
                Logger.Warning("客户端已经关闭!");
            }
        }

        public void Stop()
        {
            if (this._isStarted)
            {
                this._terminated = true;
                this._httpListenThread.Join();
                this._isStarted = false;
            }
        }
    }
}
