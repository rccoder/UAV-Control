using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using System.Collections;
using System.Net;
using MessageModel.utils;
using System.Threading;
using System.Net.Sockets;
using MessageModel.packet;

namespace MessageModel.network
{
    class sending_controller
    {
        public static bool DEBUG = true;
        public Queue<Tuple<IPAddress, Packet>> sending_queue;
        bool threading_running = false;

        public sending_controller(Queue<Tuple<IPAddress, Packet>> queue)
        {
            this.sending_queue = queue;
            this.threading_running = true;
        }
        public void run()
        {
            while (this.threading_running)
            {
                if (sending_queue.Count != 0)
                {
                    Tuple<IPAddress, Packet> first = this.sending_queue.Dequeue();
                    IPAddress target_url = first.Item1;
                    Packet packet = first.Item2;
                    //Console.WriteLine(Encoding.UTF8.GetString(packet.Payload));
                    send_to(target_url, packet);
                }
                else
                    Thread.Sleep(100);
            }
        }

        public void stop()
        {
            this.threading_running = false;
        }

        public static void send_to(IPAddress target_url,Packet packet)
        {
            Socket s = new Socket(AddressFamily.InterNetwork, SocketType.Stream, ProtocolType.Tcp);
            s.Connect(new IPEndPoint(target_url, 2333));
            s.Send(packet.to_bytes());
            s.Close();
            if (DEBUG)
                Console.WriteLine("[LOG][{0}]: send a {1} packet which uuid is {2} to {3}",DateTime.Now.ToString(),Encoding.UTF8.GetString(packet.get_command_string()),Encoding.UTF8.GetString(packet.Packet_uuid) ,target_url.ToString());
        }

    }
}
