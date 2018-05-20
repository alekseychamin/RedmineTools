using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace NotificationRedmine
{
    class Program
    {
        static void Main(string[] args)
        {
            Manage manage = new Manage("188.242.201.77", "70b1a875928636d8d3895248309344ea2bca6a5f");
            TimeSpan dt;

            while (true)
            {
                dt = manage.GetTimeStart();
                Task.Delay(dt).ContinueWith((x) => manage.MakeNotificationToTime());
                Thread.Sleep(dt);
            }            
        }
    }
}
