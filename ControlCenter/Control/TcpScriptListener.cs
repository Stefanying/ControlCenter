using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using SFLib;
using System.Net;
using System.Net.Sockets;
using System.Threading;

namespace ControlCenter.Control
{
   internal class TcpScriptListener :Listener
    {
       private class StateObject
       {
           public const int BufferSize = 1024;
           public TcpClient client = null;
           public byte[] buffer  = new byte[1024];
       }
       private int _port = -1;
       private TcpListener _listenr = null;
       private Thread _thread = null;
       private volatile bool _terminated = false;
       private Socket _sock = null;
       private IPEndPoint _ip_from = null;

       public TcpScriptListener(int port)
       {
           _port = port;
           _listenr = new TcpListener(port);
           _thread = new Thread(new ThreadStart(ThreadFunc));
           _thread.Start();
           _thread.IsBackground = true;
           _sock = new Socket(AddressFamily.InterNetwork,SocketType.Dgram,ProtocolType.Udp);           
       }

       public override void Send(string data)
       {
           ASCIIEncoding aSCIIEncoding = new ASCIIEncoding();
           byte[] bytes = aSCIIEncoding.GetBytes(data);
           if (_sock != null && _ip_from != null)
           {
               _sock.SendTo(bytes,_ip_from);
           }
       }

       

       protected void ThreadFunc()
       {
           try
           {
               _ip_from = new IPEndPoint(IPAddress.Any, _port);
               _listenr.Start();
               byte[] array = new byte[1024];
               while (_terminated)
               {
                   TcpClient tcpClient = _listenr.AcceptTcpClient();
                   TcpScriptListener.StateObject stateObject = new TcpScriptListener.StateObject();
                   stateObject.client = tcpClient;
                   tcpClient.GetStream().BeginRead(stateObject.buffer, 0, stateObject.buffer.Length, new AsyncCallback(ProcessCommand), stateObject);
                   Thread.Sleep(10);
               }
               _listenr.Stop();
           }
           catch (Exception ex)
           {
               Console.WriteLine(ex.ToString());
           }
           finally
           {
 
           }
       }

       private void ProcessCommand(IAsyncResult ar)
       {
           TcpScriptListener.StateObject stateObject  =(TcpScriptListener.StateObject)ar.AsyncState;
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
                       base.FireRecv(@string);
                   }
               }
           }
           catch (Exception ex)
           {
               Logger.Exception(ex.Message);
           }
           TcpScriptListener.StateObject stateObject2 = new TcpScriptListener.StateObject();
           stateObject2.client = client;
           client.GetStream().BeginRead(stateObject2.buffer, 0, stateObject2.buffer.Length, new AsyncCallback(this.ProcessCommand), stateObject2);
       }

    }
}
