using System;
using System.Collections.Specialized;
using System.Linq;
using System.Text;
using SFLib;
using LuaApi;
using System.IO;
using System.Threading;

namespace ControlCenter.Control
{
   public  class ScriptEngineer
    {

       private string _projectName;
       public string _scriptRoot;
       private string _throwMessage = "";
       private Thread _doFileThread = null;
       private SFReturnCode _returnCode = null;


       /// <summary>
       /// 执行脚本
       /// </summary>
       /// <param name="scriptname"></param>
       /// <param name="parameters"></param>
       public void ExecuteScript(string scriptname, NameValueCollection parameters)
       {
           try
           {
               if (!File.Exists(string.Format("{0}{1}.lua", _scriptRoot, scriptname)))
               {
                   throw new FileNotFoundException();
               }
               LuaApiRegister luaHelper = new LuaApiRegister(new LuaApiInterface(_projectName));
               this.InitLuaGlobalParameter(luaHelper, parameters);
               this.ThreadExecuteFile(luaHelper, this._scriptRoot + scriptname + ".lua");
           }
           catch (Exception ex)
           {
               Logger.Exception(ex.Message);
           }
       }



       /// <summary>
       /// 初始化脚本全局参数
       /// </summary>
       /// <param name="luaHelper"></param>
       /// <param name="parameters"></param>
       private void InitLuaGlobalParameter(LuaApiRegister luaHelper, NameValueCollection parameters)
       {
           if (parameters != null && parameters.Count > 0)
           {
               string[] allKeys = parameters.AllKeys;
               for (int i = 0; i < allKeys.Length; i++)
               {
                   string text = allKeys[i];
                   luaHelper.ExecuteString(string.Concat(new string[]
                   {
                       "a_",
                       text.Trim(),
                       "=\"",
                       parameters[text].Replace("\\","\\\\"),
                       "\";"
                   }));
               }
               luaHelper.ExecuteString("g_ProjectName=\"" + _projectName+"\";");
               luaHelper.ExecuteString("package.path = package.path..[[;" + this._scriptRoot + "?.lua]]");
           }
       }


       /// <summary>
       /// 执行文件
       /// </summary>
       /// <param name="luaHelper"></param>
       /// <param name="luaFileName"></param>
       private void ThreadExecuteFile(LuaApiRegister luaHelper, string luaFileName)
       {
           int num = 0;
           try
           {
               num = int.Parse(Config.Items["Protector"]);
           }
           catch
           {

           }

           //执行文件线程
            _doFileThread = new Thread(new ThreadStart(()=>
                {
                    try
                    {
                        _throwMessage = "";
                        _returnCode = null;
                        luaHelper.ExecuteFile(luaFileName);
                    }
                    catch (ThreadAbortException var_0_33)
                    {
                        Logger.Info("脚本引擎主动中止线程.");
                    }
                    catch (SFReturnCode returnCode)
                    {
                        _returnCode = returnCode;
                    }
                    catch (Exception ex)
                    {
                        _throwMessage = ex.Message;
                    }

                }));
           
           this._doFileThread.IsBackground = true;
           this._doFileThread.Start();
           ThreadProtecter.getInstance(num).Start(_doFileThread);
           if (ThreadProtecter.getInstance(num).IsTimeout)
           {
               Logger.Error("自动关闭脚本");
               throw new TimeoutException("自动关闭脚本");
           }

           if (this._returnCode != null)
           {
               throw this._returnCode;
           }

           if (string.IsNullOrEmpty(this._throwMessage))
           {
               Logger.Info("执行完毕：" + luaFileName);
           }
           else
           {
               if (!string.IsNullOrEmpty(this._throwMessage))
               {
                   Logger.Info(this._throwMessage);
               }
           }

       }

      /// <summary>
      /// 开始执行
      /// </summary>
      /// <param name="projectName"></param>
       public void Start(string projectName)
       {
           _projectName = projectName;
           _scriptRoot = AppDomain.CurrentDomain.BaseDirectory + "Script\\";
           if (!Directory.Exists(_scriptRoot))
           {
               Logger.Exit("检测脚本路径不存在");
           }
       }

      /// <summary>
      /// 停止
      /// </summary>
       public void Stop()
       {
           try
           {
               Logger.Info("退出程序!");
           }
           catch
           {
               Logger.Warning("退出异常！");
           }
       }


    }
}
