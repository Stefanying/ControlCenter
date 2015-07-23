using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Windows.Forms;
using SFLib;
namespace ControlCenter
{
    static class Program
    {
        /// <summary>
        /// 应用程序的主入口点。
        /// </summary>
        [STAThread]
        static void Main()
        {
            Application.EnableVisualStyles();
            Application.SetCompatibleTextRenderingDefault(false);
            AppDomain.CurrentDomain.UnhandledException += new UnhandledExceptionEventHandler(Program.CurrentDomain_UnhandledException);
            Program.CheckOneProcess("神州服联中控控制器");
            Application.Run(new Form1());
        }

        private static void CurrentDomain_UnhandledException(object sender, UnhandledExceptionEventArgs e)
        {
            Logger.Exception(e.ExceptionObject as Exception);
            MessageBox.Show((e.ExceptionObject as Exception).Message, "异常!", MessageBoxButtons.OK, MessageBoxIcon.Hand);
            Environment.Exit(Environment.ExitCode);
        }
        private static void CheckOneProcess(string title)
        {
            if (!Process.GetCurrentProcess().ProcessName.Contains(".vshost"))
            {
                if (Process.GetProcessesByName(Process.GetCurrentProcess().ProcessName).Length > 1)
                {
                    MessageBox.Show(title + "已经启动！");
                    Environment.Exit(Environment.ExitCode);
                }
            }
        }
    }
}
