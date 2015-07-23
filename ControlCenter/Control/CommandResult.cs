using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace ControlCenter.Control
{
    public enum CommandResult
    {
        Success = 1,
        UrlTooLong,
        UrlInvalidChar,
        FileNameInvalidChar,
        NoExistsMethod,
        FileNotExists,
        ExcuteFunctionFailed,
        ServerIsBusy,
        DoFunctionTooLongTimeProtect
    }
}
