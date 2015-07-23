using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.IO;
using System.Xml;
using SFLib;
namespace ControlCenter.Control
{
   internal  class ConfigWriter
    {
       private static string configFile = AppDomain.CurrentDomain.BaseDirectory + "config.xml";
       private static string timelineFile = AppDomain.CurrentDomain.BaseDirectory + "timeline.xml";


       //加载配置文件
       public static void LoadConfig()
       {
           try
           {
               XmlDocument xmlDocument = new XmlDocument();
               xmlDocument.Load(ConfigWriter.configFile);
               XmlNode xmlNode = xmlDocument.SelectSingleNode("Root");
               XmlNodeList xmlNodeList = xmlNode.SelectNodes("Area");
               foreach (XmlNode xmlNode2 in xmlNodeList)
               {
                   XmlNodeList xmlNodeList2 = xmlNode2.SelectNodes("Action");
                   for (int i = 0; i < xmlNodeList2.Count; i++)
                   {
                       XmlNode  xmlNode3 = xmlNodeList2[i];
                       string ActionReceiveData = xmlNode3.SelectSingleNode("ActioReceiveData").InnerText;
                       string text = "";
                       string ActionName = xmlNode3.SelectSingleNode("ActionName").InnerText;
                       XmlNodeList xmlNodeList3 = xmlNode3.SelectNodes("Operation");
                       for (int j = 0; i < xmlNodeList3.Count; j++)
                       {
                           XmlNode xmlNode4 =xmlNodeList3[j];
                           string text2 = "";
                           string OperactionName = xmlNode4.SelectSingleNode("OperactionName").InnerText;
                           string OperactionType = xmlNode4.SelectSingleNode("OperactionType").InnerText;
                           string OperactionDataType = xmlNode4.SelectSingleNode("OperactionDataType").InnerText;
                           string OperactionData = xmlNode4.SelectSingleNode("OperactionData").InnerText;
                           string OperactionTime = xmlNode4.SelectSingleNode("OperactionTime").InnerText;
                           string text3 = "Setting={";
                           XmlNode xmlNode5 = xmlNode4.SelectSingleNode("OperactionSetting");
                           string text4 = OperactionType.ToLower();
                           string text5;
                           if (text4 != null)
                           {
                               if (!(text4 == "com"))
                               {
                                   if (text4 == "tcp" || text4 == "udp")
                                   {
                                       string IP = xmlNode5.SelectSingleNode("IP").InnerText;
                                       string Port = xmlNode5.SelectSingleNode("Port").InnerText;
                                       text5 = text3;
                                       text3 = string.Concat(new string[]
										{
											text5,
											"ip = \"",
											IP,
											"\",port =\"",
											Port,
											"\"},"
										});
                                   }
                                   else
                                   {
                                       string ComNumber = xmlNode5.SelectSingleNode("ComNumber").InnerText;
                                       string BaudRate = xmlNode5.SelectSingleNode("BaudRate").InnerText;
                                       string DataBit = xmlNode5.SelectSingleNode("DataBit").InnerText;
                                       string StopBit = xmlNode5.SelectSingleNode("StopBit").InnerText;
                                       string Parity = xmlNode5.SelectSingleNode("Parity").InnerText;
                                       text5 = text3;
                                       text3 = string.Concat(new string []
                                           {
                                               text5,
                                               "comnumber=\"",
                                               ComNumber,
                                               "baudrate=\"",
                                               BaudRate,
                                               "databit=\"",
                                               DataBit,
                                               "stopbit=\"",
                                               StopBit,
                                               "parity=\"",
                                               Parity,
                                               "\"},"
                                           });
                                   } 
                               }

                               text5 = text2;
                               text2 = string.Concat(new string []
                                   {
                                       text5,
                                       "[",
                                       (j+1).ToString(),
                                       "]={ operationType =\"",
                                       OperactionType,
                                       "\",operationDataType=\"",
                                       OperactionDataType,
                                       "\",operationData=\"",
                                       OperactionData,
                                       "\",",
                                       text3,
                                       "operationTime=\"",
                                       OperactionTime,
                                       "\"}"
                                   });
                               if (j != xmlNodeList3.Count - 1)
                               {
                                   text2 += ",";
                               }
                               text = text + text2 + Environment.NewLine;
                           }

                           StreamReader streamReader = new StreamReader(AppDomain.CurrentDomain.BaseDirectory + "Script\\scriptTemplate.lua");
                           string text6 = streamReader.ReadToEnd();
                           text6 = text6.Replace("#commands", text);
                           StreamWriter streamWriter = new StreamWriter(AppDomain.CurrentDomain.BaseDirectory + "Script\\" + ActionReceiveData + ".lua");
                           streamWriter.Write(text6);
                           streamWriter.Flush();
                           streamWriter.Close();
                           streamReader.Close();
                       }
 
                   }
 
               }
           }
           catch
           {
               Logger.Error("解析配置出错!");
           }
       }

       //加载时间轴配置文件
       public static void LoadTimeShaft()
       {
           try
           {
               XmlDocument xmlDocument = new XmlDocument();
               xmlDocument.Load(ConfigWriter.configFile);
               XmlNode xmlNode = xmlDocument.SelectSingleNode("Root");
               XmlNode xmlNode2 = xmlNode.SelectSingleNode("TimeShaft");
               XmlNodeList xmlNodeList = xmlNode2.SelectNodes("Action");
               for (int i=0; i < xmlNodeList.Count; i++)
               {
                   XmlNode xmlNode3 = xmlNodeList[i];
                   string ActionReceiveData = xmlNode3.SelectSingleNode("ActionReceiveData").InnerText;
                   string text = "";
                   string ActionName = xmlNode3.SelectSingleNode("ActionName").InnerText;
                   XmlNodeList xmlNodeList2 = xmlNode3.SelectNodes("Operaction");
                   for (int j=0; j < xmlNodeList2.Count; j++)
                   {
                       XmlNode xmlNode4 = xmlNodeList2[j];
                       string text2 = "";
                       string OperactionName = xmlNode4.SelectSingleNode("OperactionName").InnerText;
                       string OperactionType = xmlNode4.SelectSingleNode("OperactionType").InnerText;
                       string OperactionDataType = xmlNode4.SelectSingleNode("OperactionDataType").InnerText;
                       string OperactionData = xmlNode4.SelectSingleNode("OperactionData").InnerText;
                       string OperactionTime = xmlNode4.SelectSingleNode("OperactionTime").InnerText;
                       string text3 = "Setting={";
                       XmlNode xmlNode5 = xmlNode4.SelectSingleNode("OperactionSetting");
                       string text4 = OperactionType.ToLower();
                       string text5;
                       if (text4 != null)
                       {
                           if (!(text4 == "com"))
                           {
                               if (text4 == "tcp" || text4 == "udp")
                               {
                                   string IP = xmlNode5.SelectSingleNode("IP").InnerText;
                                   string Port = xmlNode5.SelectSingleNode("Port").InnerText;

                                   text5 = text3;
                                   text3 = string.Concat(new string[]
									{
										text5,
										"ip = \"",
										IP,
										"\",port =\"",
										Port,
										"\"},"
									});
                               }

                           }
                           else
                           {
                               string ComNumber = xmlNode5.SelectSingleNode("ComNumber").InnerText;
                               string BaudRate = xmlNode5.SelectSingleNode("BaudRate").InnerText;
                               string DataBit = xmlNode5.SelectSingleNode("DataBit").InnerText;
                               string StopBit = xmlNode5.SelectSingleNode("StopBit").InnerText;
                               string Parity = xmlNode5.SelectSingleNode("Parity").InnerText;
                               text5 = text3;
                               text3 = string.Concat(new string[]
								{
									text5,
									"comNumber =\"",
									ComNumber,
									"\",baudRate = \"",
									BaudRate,
									"\",dataBit = \"",
									DataBit,
									"\",stopBit = \"",
									StopBit,
									"\",parity = \"",
									Parity,
									"\"},"
								});
                           }
                       }

                       text5 = text2;
                       text2 = string.Concat(new string[]
						{
							text5,
							"[",
							(j + 1).ToString(),
							"]  = { operationType =\"",
							OperactionType,
							"\", operationDataType =\"",
							OperactionDataType,
							"\", operationData =\"",
							OperactionData,
							"\",",
							text3,
							"operationTime =\"",
							OperactionTime,
							"\"}"
						});
                       if (j != xmlNodeList2.Count - 1)
                       {
                           text2 += ",";
                       }
                       text = text + text2 + Environment.NewLine;
                   }
                   StreamReader streamReader = new StreamReader(AppDomain.CurrentDomain.BaseDirectory + "Script\\TimeShaft.lua");
                   string text6 = streamReader.ReadToEnd();
                   text6 = text6.Replace("#commands", text);
                   StreamWriter streamWriter = new StreamWriter(AppDomain.CurrentDomain.BaseDirectory + "Script\\" + ActionReceiveData + ".lua");
                   streamWriter.Write(text6);
                   streamWriter.Flush();
                   streamWriter.Close();
                   streamReader.Close();
               }

           }
           catch
           {
               Logger.Error("解析配置文件出错!");
           }
 
       }

       /// <summary>
       /// 加载时间轴配置
       /// </summary>
       public static void LoadTimeLineConfig()
       {
           try
           {
               XmlDocument xmlDocument = new XmlDocument();
               xmlDocument.Load(ConfigWriter.timelineFile);
               XmlNode xmlNode = xmlDocument.SelectSingleNode("Root");
               XmlNodeList xmlNodeList = xmlNode.SelectNodes("Time");
               string text = "";
               foreach (XmlNode xmlNode2 in xmlNodeList)
               {
                   string innerText = xmlNode2.SelectSingleNode("TimeValue").InnerText;
                   text = text + "[\"" + innerText + "\"] ={";
                   XmlNodeList xmlNodeList2 = xmlNode2.SelectNodes("Operation");
                   for (int i = 0; i < xmlNodeList2.Count; i++)
                   {
                       XmlNode xmlNode3 = xmlNodeList2[i];
                       string text2 = "";
                       string innerText2 = xmlNode3.SelectSingleNode("OperationName").InnerText;
                       string innerText3 = xmlNode3.SelectSingleNode("OperationType").InnerText;
                       string innerText4 = xmlNode3.SelectSingleNode("OperationDataType").InnerText;
                       string innerText5 = xmlNode3.SelectSingleNode("OperationData").InnerText;
                       string innerText6 = xmlNode3.SelectSingleNode("OperationTime").InnerText;
                       string text3 = "Setting = {";
                       XmlNode xmlNode4 = xmlNode3.SelectSingleNode("OperationSetting");
                       string text4 = innerText3.ToLower();
                       string text5;
                       if (text4 != null)
                       {
                           if (!(text4 == "com"))
                           {
                               if (text4 == "tcp" || text4 == "udp")
                               {
                                   string innerText7 = xmlNode4.SelectSingleNode("IP").InnerText;
                                   string innerText8 = xmlNode4.SelectSingleNode("Port").InnerText;
                                   text5 = text3;
                                   text3 = string.Concat(new string[]
									{
										text5,
										"ip = \"",
										innerText7,
										"\",port =\"",
										innerText8,
										"\"},"
									});
                               }
                           }
                           else
                           {
                               string innerText9 = xmlNode4.SelectSingleNode("ComNumber").InnerText;
                               string innerText10 = xmlNode4.SelectSingleNode("BaudRate").InnerText;
                               string innerText11 = xmlNode4.SelectSingleNode("DataBit").InnerText;
                               string innerText12 = xmlNode4.SelectSingleNode("StopBit").InnerText;
                               string innerText13 = xmlNode4.SelectSingleNode("Parity").InnerText;
                               text5 = text3;
                               text3 = string.Concat(new string[]
								{
									text5,
									"comNumber =\"",
									innerText9,
									"\",baudRate = \"",
									innerText10,
									"\",dataBit = \"",
									innerText11,
									"\",stopBit = \"",
									innerText12,
									"\",parity = \"",
									innerText13,
									"\"},"
								});
                           }
                       }
                       text5 = text2;
                       text2 = string.Concat(new string[]
						{
							text5,
							"[",
							(i + 1).ToString(),
							"]  = { operationType =\"",
							innerText3,
							"\", operationDataType =\"",
							innerText4,
							"\", operationData =\"",
							innerText5,
							"\",",
							text3,
							"operationTime =\"",
							innerText6,
							"\"}"
						});
                       if (i != xmlNodeList2.Count - 1)
                       {
                           text2 += ",";
                       }
                       text = text + text2 + Environment.NewLine;
                   }
                   text += "}";
                   if (xmlNode2 != xmlNodeList[xmlNodeList.Count - 1])
                   {
                       text += ",";
                   }
               }
               StreamReader streamReader = new StreamReader(AppDomain.CurrentDomain.BaseDirectory + "Script\\TimeLineTemplate.lua");
               string text6 = streamReader.ReadToEnd();
               text6 = text6.Replace("#commands", text);
               StreamWriter streamWriter = new StreamWriter(AppDomain.CurrentDomain.BaseDirectory + "Script\\TimeLine.lua");
               streamWriter.Write(text6);
               streamWriter.Flush();
               streamWriter.Close();
               streamReader.Close();
           }
           catch
           {
               Logger.Error("解析配置出错!");
           }
       }

       
       /// <summary>
       /// 十六进制转换成ASCII码
       /// </summary>
       /// <param name="hexstring"></param>
       /// <returns></returns>
       public static string HexStringToASCII(string hexstring)
       {
           byte[] array = ConfigWriter.HexStringToBinary(hexstring);
           string text = "";
           for (int i = 0; i < array.Length; i++)
           {
               text = text + array[i] + " ";
           }
           string[] array2 = text.Trim().Split(new char[]
			{
				' '
			});
           char[] array3 = new char[array2.Length];
           for (int i = 0; i < array3.Length; i++)
           {
               int value = Convert.ToInt32(array2[i]);
               array3[i] = Convert.ToChar(value);
           }
           return new string(array3);
       }


       /// <summary>
       /// 十六进制转换成二进制
       /// </summary>
       /// <param name="hexstring"></param>
       /// <returns></returns>
       public static byte[] HexStringToBinary(string hexstring)
       {
           int num;
           if (hexstring.Length % 2 == 0)
           {
               num = hexstring.Length / 2;
           }
           else
           {
               num = hexstring.Length / 2 + 1;
           }
           string[] array = new string[num];
           for (int i = 0; i < num; i++)
           {
               if (hexstring.Length - i * 2 >= 2)
               {
                   array[i] = hexstring.Substring(i * 2, 2);
               }
               else
               {
                   array[i] = "0" + hexstring.Substring(i * 2, 1);
               }
           }
           byte[] array2 = new byte[array.Length];
           for (int i = 0; i < array2.Length; i++)
           {
               array2[i] = Convert.ToByte(array[i], 16);
           }
           return array2;
       }

    }
}
