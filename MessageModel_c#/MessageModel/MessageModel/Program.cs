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

        }
        
    }
}
