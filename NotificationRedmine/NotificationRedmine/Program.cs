using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace NotificationRedmine
{
    public static class SaveError
    {
        static StreamWriter logOut;
        public static void CreateLog(string filename)
        {
            try
            {
                logOut = new StreamWriter(filename);
                Console.SetError(logOut);
            }
            catch (Exception ex)
            {
                Console.WriteLine("Error while create log: {0}", ex);
            }
        }
        public static void SaveMessage(string message)
        {
            if (logOut != null)
            {
                Console.Error.WriteLine(DateTime.Now.ToString() + " Error in " + message);
                logOut.Flush();
            }
        }

        public static void Close()
        {
            logOut.Close();
        }
    }

    class Program
    {        
        static void Main(string[] args)
        {
            SaveError.CreateLog("error.log");
            Manage manage = new Manage("188.242.201.77", "70b1a875928636d8d3895248309344ea2bca6a5f");
                        
            manage.StartMakeNotification(2);
            //manage.MakeNotificationToTime();       

            SaveError.Close();
            Console.WriteLine(DateTime.Now.ToString() + " program stop working!");
            Console.ReadLine();
        }
    }
}
