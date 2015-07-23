using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Runtime.InteropServices;
namespace ControlCenter.Control
{
   internal class SystemTimer
    {
       [DllImport("Kernal32.dll")]
       public static extern bool SetLocalTime(ref SYSTEMTIME sysTime);
       [DllImport("Kernal32.dll")]
       public static extern void GetLocalTime(ref SYSTEMTIME sysTime);


       public static string GetTime()
       {
           SYSTEMTIME    sYSTETIME = default(SYSTEMTIME);
           SystemTimer.GetLocalTime(ref sYSTETIME);
           return sYSTETIME.wHour.ToString() + ":" + sYSTETIME.wMinute.ToString();
       }


       public  static string   SetTime(ushort hour,ushort minute)
       {
           string result;
           try
           {
               SYSTEMTIME sYSTEMTIME  =default(SYSTEMTIME);
               SystemTimer.GetLocalTime(ref  sYSTEMTIME);
               sYSTEMTIME.wHour = hour;
               sYSTEMTIME.wMinute = minute;
               SystemTimer.SetLocalTime(ref sYSTEMTIME);
               result ="sucess";
           }
           catch
           {
               result = "failed";
           }
           return result;
       }

    }
}
