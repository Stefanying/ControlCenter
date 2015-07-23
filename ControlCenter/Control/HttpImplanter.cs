using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Net;
using SFLib;

namespace ControlCenter.Control
{
    internal interface HttpImplanter
    {
        void Start();
        void Stop();
        void MakeHttpPrefix(HttpListener server);
        SFReturnCode ProcessRequest(HttpListenerContext context);
        byte[] CreateReturnResult(HttpListenerContext context, SFReturnCode result);

    }
}
