using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using MessageModel.config;
using MessageModel.utils;
using System.Net;
using System.Security.Cryptography;
namespace MessageModel
{
    
    class Program
    {
        static void Main(string[] args)
        {
            worker my_worker = new worker();
            Thread work = new Thread(my_worker.run);
            work.Start();
            /*
            JsonDataObject js = new JsonDataObject();
            js.command = "plane_number";
            Dictionary < string, string> d = new Dictionary<string, string>();
            d.Add("plane_state", "all");
            d.Add("plane_number","5");
            js.options = d;
            IPAddress ip = IPAddress.Parse(ROUTE_ADDRESS.route_address);
            my_worker.send_data_packet(ip, js);
            */
            
        }
        
    }
}
