using System;
using System.Collections.Generic;
using System.Collections.Specialized;
using System.Linq;
using System.Text;
using System.Web;
using SFLib;

namespace ControlCenter.Control
{
   public  class UrlHelper
    {
       private const int MAX_URI_LENGTH = 512;
       private string _scriptName = string.Empty;
       private CommandResult _parseResult = CommandResult.Success;
       private NameValueCollection _parameters = new NameValueCollection();

       private char[] _uriInvalidChar = new char[]
		{
			'\\'
		};

       private char[] _pathInvalidChar = new char[]
		{
			'\\',
			':',
			'*',
			'?',
			'"',
			'<',
			'>',
			'|'
		};

       public Uri _uri = null;

       public string ScriptName
       {
           get
           {
               return this._scriptName;
           }
       }

       public NameValueCollection Parameters
       {
           get
           {
               return this._parameters;
           }
       }

       public CommandResult ParseResult
       {
           get
           {
               return this._parseResult;
           }
       }

       public UrlHelper(Uri originalUri)
       {
           this._uri = originalUri;
           if (!this.IsUriLengthError())
           {
               if (this.CheckPathAndQuery())
               {
                   this.ParsePathAndQuery();
               }
           }
       }

       private bool IsUriLengthError()
       {
           bool result;
           if (this._uri == null || this._uri.ToString().Length > 512)
           {
               this._parseResult = CommandResult.UrlTooLong;
               result = true;
           }
           else
           {
               result = false;
           }
           return result;
       }

       private bool CheckPathAndQuery()
       {
           string text = this._uri.PathAndQuery.Substring(1);
           bool result;
           if (this.IsUrlInvalidChar(text))
           {
               result = false;
           }
           else
           {
               if (text.IndexOfAny(this._uriInvalidChar) >= 0)
               {
                   this._parseResult = CommandResult.UrlInvalidChar;
                   result = false;
               }
               else
               {
                   if (text.Length == 0)
                   {
                       this._parseResult = CommandResult.NoExistsMethod;
                       result = false;
                   }
                   else
                   {
                       string[] splitPathAndQuery = new string[0];
                       result = !this.IsFileNameInvalidChar(text, splitPathAndQuery);
                   }
               }
           }
           return result;
       }

       private bool IsFileNameInvalidChar(string pathAndQuery, string[] splitPathAndQuery)
       {
           splitPathAndQuery = pathAndQuery.Split(new char[]
			{
				'?'
			}, StringSplitOptions.RemoveEmptyEntries);
           bool result;
           if (splitPathAndQuery[0].IndexOfAny(this._pathInvalidChar) >= 0)
           {
               this._parseResult = CommandResult.FileNameInvalidChar;
               result = true;
           }
           else
           {
               result = false;
           }
           return result;
       }

       private bool IsUrlInvalidChar(string pathAndQuery)
       {
           bool result;
           if (pathAndQuery.IndexOfAny(this._uriInvalidChar) >= 0)
           {
               this._parseResult = CommandResult.UrlInvalidChar;
               result = true;
           }
           else
           {
               result = false;
           }
           return result;
       }

       private void ParsePathAndQuery()
       {
           string[] scriptNameAndParameters = this._uri.PathAndQuery.Substring(1).Split(new char[]
			{
				'?'
			}, StringSplitOptions.RemoveEmptyEntries);
           this.SetScriptNameAndParameters(scriptNameAndParameters);
       }

       private void SetScriptNameAndParameters(string[] splitPathAndQuery)
       {
           this._scriptName = splitPathAndQuery[0];
           if (splitPathAndQuery.Length > 1)
           {
               this._parameters = HttpUtility.ParseQueryString(splitPathAndQuery[1], Encoding.UTF8);
           }
       }
    }
}
