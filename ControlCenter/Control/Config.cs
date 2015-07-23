using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using SFLib;
using System.IO;


namespace ControlCenter.Control
{
   internal class Config
    {
       public static readonly Dictionary<string, string> Items = new Dictionary<string, string>();
       private static readonly string[] importantConfigParameters = new string[]
       {
           "ProjectName",
           "Protector",
           "TcpPort",
           "UdpPort",
           "ComPort",
           "IsComEnable",
           "ComNewLine",
           "Http",
           "TCP",
           "UDP"
       };

       public static void Load(string file)
       {
           if (!File.Exists(file))
           {
               Logger.Exit("配置文件不存在！"+file);
           }

           Config.Items.Clear();
           string[] array = File.ReadAllLines(file);
           Console.WriteLine(array.Length);
           for (int i = 0; i < array.Length; i++)
           {
               Config.ParseLine(array, i);
           }
           Config.ExitIfMissingParameters(Config.importantConfigParameters);


       }

       private static void ExitIfMissingParameters(string[] configParameters)
       {
           if (configParameters != null)
           {
               for (int i = 0; i < configParameters.Length; i++)
               {
                   string  text = configParameters[i];
                   if (!Config.Items.ContainsKey(text))
                   {
                       Logger.Exit(string.Format("程序初始化配置文件缺少参数【{0}】!",text));
                   }
               }
           }
       }

       private static void ParseLine(string[] lines, int index)
       {
           try
           {
               int num = lines[index].IndexOf('=');
               Config.Items.Add(lines[index].Substring(0, num).Trim(), lines[index].Substring(num + 1).Trim());
           }
           catch
           {
               Logger.Warning("解析程序初始化配置文件出错。 第" + (index + 1).ToString() + "行");
           }
       }

    }
}
