using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.IO.Ports;
using SFLib;
namespace ControlCenter.Control
{
   internal class COMListener :Listener
    {

       private SerialPort _com = null;

        public override void Send(string data)
        {
            this._com.WriteLine(data);
        }

        public COMListener(string port_name, string new_line)
        {
            try
            {
                this._com = new SerialPort();
                this._com.PortName = port_name;
                this._com.BaudRate = 9600;
                this._com.DataBits = 8;
                this._com.StopBits = StopBits.One;
                this._com.NewLine = new_line;
                this._com.ReadTimeout = 200;
                this._com.RtsEnable = true;
                this._com.Open();
                this._com.DataReceived += new SerialDataReceivedEventHandler(this.OnRecv);
            }
            catch (Exception ex)
            {
                this._com = null;
                Logger.Info(ex.Message);
            }
        }

      
        private void OnRecv(object sender, SerialDataReceivedEventArgs e)
        {
            SerialPort serialPort = (SerialPort)sender;
            string received_data = serialPort.ReadLine();
            base.ProcessPack(received_data);
        }
    }
}
