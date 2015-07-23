using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Net.Sockets;
namespace ControlCenter.Control
{
   public  class StateObject
    {
        public const int BufferSize = 1024;
        public TcpClient client = null;
        public byte[] buffer = new byte[1024];
    }
}
