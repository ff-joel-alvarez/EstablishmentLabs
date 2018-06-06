using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Configuration;
namespace ServerService
{
    public static class Library
    {
        public static void WriteErrorLog(Exception ex)
        {
            StreamWriter sw = null;
            try
            {
                Library.WriteLine(ConfigurationManager.AppSettings["logPath"]);
                sw = new StreamWriter(ConfigurationManager.AppSettings["logPath"], true);
                sw.WriteLine($"{DateTime.Now.ToString()} : {ex.Source.ToString().Trim()} ; {ex.Message.ToString().Trim()}");
                sw.Flush();
                sw.Close();

            }
            catch { }
        }
        public static void WriteLine(String message)
        {
            StreamWriter sw = null;
            try
            {
                sw = new StreamWriter("C:\\TestSS\\LogFile.txt", true);
                sw.WriteLine($"{DateTime.Now.ToString()} : { message}\n");
                sw.Flush();
                sw.Close();

            }
            catch { }
        }
    }
}
