using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using SFLib;
using System.Net;
using System.IO;
namespace ControlCenter.Control
{
   internal class ScriptServer : HttpImplanter
    {

        private ScriptEngineer _scriptEngineer = new ScriptEngineer();
        private string _projectName;
        public ScriptEngineer ScriptEngineer
        {
            get
            {
                return this._scriptEngineer;
            }
            set
            {
                this._scriptEngineer = value;
            }
        }

        public ScriptServer(string projectName)
        {
            this._projectName = projectName;
        }

        public void Start()
        {
            this._scriptEngineer.Start(this._projectName);
        }

        public void MakeHttpPrefix(HttpListener server)
        {
            server.Prefixes.Clear();
            server.Prefixes.Add("http://*:80/");
        }

        public SFReturnCode ProcessRequest(HttpListenerContext context)
        {
            UrlHelper urlHelper = new UrlHelper(context.Request.Url);
            CommandResult parseResult = urlHelper.ParseResult;
            SFReturnCode result;
            if (urlHelper.ParseResult == CommandResult.Success)
            {
                try
                {
                    this._scriptEngineer.ExecuteScript(urlHelper.ScriptName, urlHelper.Parameters);
                    result = new SFReturnCode(1, EnumHelper.GetEnumDescription(CommandResult.Success));
                    return result;
                }
                catch (FileNotFoundException var_2_5D)
                {
                    result = new SFReturnCode(5, EnumHelper.GetEnumDescription(CommandResult.NoExistsMethod));
                    return result;
                }
                catch (TimeoutException var_3_74)
                {
                    result = new SFReturnCode(9, EnumHelper.GetEnumDescription(CommandResult.DoFunctionTooLongTimeProtect));
                    return result;
                }
                catch (SFReturnCode sFReturnCode)
                {
                    result = sFReturnCode;
                    return result;
                }
                catch (Exception var_5_96)
                {
                    result = new SFReturnCode(7, EnumHelper.GetEnumDescription(CommandResult.ExcuteFunctionFailed));
                    return result;
                }
            }
            result = new SFReturnCode((int)parseResult, EnumHelper.GetEnumDescription(parseResult));
            return result;
        }

        public byte[] CreateReturnResult(HttpListenerContext context, SFReturnCode result)
        {
            string s = string.Format("msg={0}", result.Message);
            return Encoding.UTF8.GetBytes(s);
        }

        public void Stop()
        {
            this._scriptEngineer.Stop();
        }
    }
}
