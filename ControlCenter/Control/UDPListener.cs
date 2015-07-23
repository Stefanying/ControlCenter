using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Net;
using System.Net.Sockets;
using System.Threading;

namespace ControlCenter.Control
{
    internal  class UDPListener :Listener
    {
        private int _port = -1;
        private UdpClient _listener = null;
        private Thread _thread = null;
        private Socket _sock = null;
        private IPEndPoint _ip_from = null;
        public UDPListener(int port)
        {
            this._port = port;
            this._listener = new UdpClient(port);
            this._thread = new Thread(new ThreadStart(this.ThreadFunc));
            this._thread.IsBackground = true;
            this._thread.Start();
            this._sock = new Socket(AddressFamily.InterNetwork, SocketType.Dgram, ProtocolType.Udp);
        }
        public override void Send(string data)
        {
            ASCIIEncoding aSCIIEncoding = new ASCIIEncoding();
            byte[] bytes = aSCIIEncoding.GetBytes(data);
            if (this._sock != null && this._ip_from != null)
            {
                this._sock.SendTo(bytes, this._ip_from);
            }
        }
        protected void ThreadFunc()
        {
            try
            {
                this._ip_from = new IPEndPoint(IPAddress.Any, this._port);
                byte[] array = new byte[1024];
                while (true)
                {
                    array = this._listener.Receive(ref this._ip_from);
                    string @string = Encoding.ASCII.GetString(array, 0, array.Length);
                    base.ProcessPack(@string);
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.ToString());
            }
            finally
            {
                this._listener.Close();
            }
        }

    }
}
