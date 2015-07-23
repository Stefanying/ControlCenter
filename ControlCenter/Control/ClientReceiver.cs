using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.IO;
using System.Net;
using System.Net.Sockets;
using System.Text;
using System.Diagnostics;
using SFLib;

namespace ControlCenter.Control
{
  internal  class ClientReceiver
    {
      private int _port = 10003;
      private TcpListener _listener;
      private TcpClient _connector;
      private NetworkStream _dataStream;

      private string _configPath = AppDomain.CurrentDomain.BaseDirectory +"config.xml";
      private string _timelineconfig = AppDomain.CurrentDomain.BaseDirectory + "timeline.xml";
      private int _blockLength = 1024;


      public event EventHandler OnConfigUpdated;
      public event EventHandler OnTimeLineUpdated;


      public void Start()
      {
          IPEndPoint localEP = new IPEndPoint(IPAddress.Any, this._port);
          this._listener = new TcpListener(localEP);
          this._listener.Start();
          this._listener.BeginAcceptTcpClient(new AsyncCallback(this.AcceptCallback), this._listener);
      }

      public void Stop()
      {
          this._listener.Stop();
          if (this._connector != null)
          {
              if (this._connector.Connected)
              {
                  this._connector.Close();
              }
              this._connector = null;
          }
      }

      private void AcceptCallback(IAsyncResult iar)
      {
          try
          {
              TcpListener tcpListener = (TcpListener)iar.AsyncState;
              this._connector = tcpListener.EndAcceptTcpClient(iar);
              this._dataStream = this._connector.GetStream();
              StateObject stateObject = new StateObject();
              stateObject.client = this._connector;
              this._dataStream.BeginRead(stateObject.buffer, 0, stateObject.buffer.Length, new AsyncCallback(this.AcceptData), stateObject);
              this._listener.BeginAcceptTcpClient(new AsyncCallback(this.AcceptCallback), this._listener);
          }
          catch (Exception ex)
          {
              Logger.Exception(ex.Message);
          }
      }


      private void AcceptData(IAsyncResult ar)
      {
          StateObject stateObject = (StateObject)ar.AsyncState;
          TcpClient client = stateObject.client;
          if (client.Connected)
          {
              int num = 0;
              try
              {
                  num = client.Client.EndReceive(ar);
              }
              catch
              {
                  num = 0;
              }
              if (num != 0)
              {
                  string @string = Encoding.Default.GetString(stateObject.buffer, 0, num);
                  string text = @string;
                  switch (text)
                  {
                      case "GetData":
                          this.SendFile(stateObject);
                          break;
                      case "SendData":
                          this.ReceiveFile(stateObject, this._configPath);
                          if (this.OnConfigUpdated != null)
                          {
                              this.OnConfigUpdated(this, null);
                          }
                          break;
                      case "SetIP":
                          this.SetIP(stateObject);
                          break;
                      case "SendTimeLineData":
                          this.ReceiveFile(stateObject, this._timelineconfig);
                          if (this.OnTimeLineUpdated != null)
                          {
                              this.OnTimeLineUpdated(this, null);
                          }
                          ClientReceiver.RestartThis();
                          break;
                      case "CancelTimeLine":
                          this.CancelTimeLine(stateObject);
                          break;
                      case "GetTime":
                          this.ReturnTime(stateObject);
                          break;
                      case "SetTime":
                          this.SetTime(stateObject);
                          break;
               //       case "GetLockState":
                 //         this.ReturnLockState(stateObject);
                   //       break;
               //       case "Activate":
                 //         this.ActivateLock(stateObject);
                   //       break;
                  }
              }
          }
      }

      /*
      private void ReturnLockState(StateObject receiveData)
      {
          try
          {
              string s;
              if (!this._dog.CheckDog())
              {
                  s = "未检测到加密锁！";
              }
              else
              {
                  if (!this._dog.CheckPeriod())
                  {
                      s = "加密锁已过期！" + Environment.NewLine + "序列号:" + this._dog.MakeSerialNumber().ToString();
                  }
                  else
                  {
                      if (!this._dog.CheckPassword("SF0002"))
                      {
                          s = "加密狗信息错误，不是本软件加密狗！";
                      }
                      else
                      {
                          s = "加密锁在有效期内！";
                      }
                  }
              }
              byte[] bytes = Encoding.UTF8.GetBytes(s);
              receiveData.client.GetStream().Write(bytes, 0, bytes.Length);
              receiveData.client.GetStream().Flush();
              receiveData.client.Close();
          }
          catch
          {
              receiveData.client.GetStream().Flush();
              receiveData.client.Close();
          }
        
      }
      */
        
      private static void RestartThis()
      {
          Process.Start("Restart.bat");
      }

      private void SendFile(StateObject receiveData)
      {
          FileStream fileStream = new FileStream(this._configPath, FileMode.Open);
          byte[] buffer = new byte[this._blockLength];
          int count;
          while ((count = fileStream.Read(buffer, 0, this._blockLength)) > 0)
          {
              receiveData.client.GetStream().Write(buffer, 0, count);
          }
          fileStream.Close();
          receiveData.client.GetStream().Flush();
          receiveData.client.Close();
      }

      private void ReceiveFile(StateObject receiveData, string filepath)
      {
          TcpClient client = receiveData.client;
          NetworkStream stream = client.GetStream();
          FileStream fileStream = new FileStream(filepath, FileMode.Create);
          byte[] buffer = new byte[this._blockLength];
          int count;
          while ((count = stream.Read(buffer, 0, this._blockLength)) > 0)
          {
              fileStream.Write(buffer, 0, count);
          }
          fileStream.Close();
          receiveData.client.GetStream().Flush();
      }

      private void SetIP(StateObject receiveData)
      {
          try
          {
              byte[] array = new byte[this._blockLength];
              int count = receiveData.client.GetStream().Read(array, 0, array.Length);
              string @string = Encoding.Default.GetString(array, 0, count);
              count = receiveData.client.GetStream().Read(array, 0, array.Length);
              string string2 = Encoding.Default.GetString(array, 0, count);
              NetLib.SetIp(@string, string2);
          }
          catch
          {
              Logger.Exception("");
          }
      }

      private void ReturnTime(StateObject receiveData)
      {
          string time = SystemTimer.GetTime();
          byte[] bytes = Encoding.UTF8.GetBytes(time);
          receiveData.client.GetStream().Write(bytes, 0, bytes.Length);
          receiveData.client.GetStream().Flush();
          receiveData.client.Close();
      }

      private void SetTime(StateObject receiveData)
      {
          byte[] array = new byte[this._blockLength];
          StateObject stateObject = new StateObject();
          stateObject.client = receiveData.client;
          receiveData.client.GetStream().BeginRead(stateObject.buffer, 0, stateObject.buffer.Length, new AsyncCallback(this.SetTimeCallback), stateObject);
      }

      private void SetTimeCallback(IAsyncResult ar)
      {
          StateObject stateObject = (StateObject)ar.AsyncState;
          TcpClient client = stateObject.client;
          try
          {
              if (client.Connected)
              {
                  int num = 0;
                  try
                  {
                      num = client.Client.EndReceive(ar);
                  }
                  catch
                  {
                      num = 0;
                  }
                  if (num != 0)
                  {
                      string @string = Encoding.Default.GetString(stateObject.buffer, 0, num);
                      ushort hour = ushort.Parse(@string.Split(new char[]
						{
							':'
						})[0]);
                      ushort minute = ushort.Parse(@string.Split(new char[]
						{
							':'
						})[1]);
                      string s = SystemTimer.SetTime(hour, minute);
                      byte[] bytes = Encoding.UTF8.GetBytes(s);
                      stateObject.client.GetStream().Write(bytes, 0, bytes.Length);
                      stateObject.client.GetStream().Flush();
                      stateObject.client.Close();
                  }
              }
          }
          catch
          {
              client.Close();
          }
      }

      private void CancelTimeLine(StateObject receiveData)
      {
          try
          {
              string path = AppDomain.CurrentDomain.BaseDirectory + "Script/TimeLine";
              File.Delete(path);
              byte[] bytes = Encoding.UTF8.GetBytes("sucess");
              receiveData.client.GetStream().Write(bytes, 0, bytes.Length);
              receiveData.client.GetStream().Flush();
              receiveData.client.Close();
          }
          catch
          {
              byte[] bytes = Encoding.UTF8.GetBytes("fail");
              receiveData.client.GetStream().Write(bytes, 0, bytes.Length);
              receiveData.client.GetStream().Flush();
              receiveData.client.Close();
          }
      }

    }
}
