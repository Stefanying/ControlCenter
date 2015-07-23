using System;
using SFLib;

namespace ControlCenter.Control
{
    internal abstract class Listener
    {
        private ScriptEngineer _scriptEngineer = new ScriptEngineer();
        protected string _pack_buf;
        public abstract void Send(string data);

        public void Start(string projectName)
        {
            _scriptEngineer.Start(projectName);
        }

        protected bool FireRecv(string cmd)
        {
            Logger.Info("接收请求"+cmd+".lua");
            _scriptEngineer.ExecuteScript(cmd,null);
            return true;
        }

        protected void ProcessPack(string received_data)
        {
            try
            {
                _pack_buf += received_data;
                bool flag = FireRecv(_pack_buf);
                _pack_buf = "";
            }

            catch (Exception ex)
            {
                _pack_buf = "";
                Logger.Exception(ex.Message);
            }
        }

        public void Stop()
        {
            _scriptEngineer.Stop();
        }
    }
}
