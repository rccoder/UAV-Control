using System;
using System.Collections.Generic;
using System.Collections;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using System.Threading;
using System.Net.Sockets;  
using System.Net;
using MessageModel.utils;
using MessageModel.packet;

namespace MessageModel.network
{
    class Receiving_Controller
    {
        bool DEBUG = true;
        public Queue<Tuple<IPAddress,Packet>> receiving_queue;
        bool threading_running = false;
        Socket s,conn,route;

        public Receiving_Controller(Queue<Tuple<IPAddress,Packet>>  queue)
        {
            this.receiving_queue = queue;
            this.threading_running = true;
            this.s = new Socket(AddressFamily.InterNetwork, SocketType.Stream, ProtocolType.Tcp);
        }

        public void run()
        {
            IPAddress this_com_ip = IPAddress.Parse("192.168.17.137");
            this.threading_running = true;
            this.s.Bind(new IPEndPoint(this_com_ip, 2333));
            this.s.Listen(5);
            while (this.threading_running)
            {
                conn = this.s.Accept();
                IPEndPoint clientipe = (IPEndPoint)conn.RemoteEndPoint;
                IPAddress route_address = clientipe.Address;

                if (DEBUG)
                    Console.WriteLine("[LOG]{0}: connect by {1}",DateTime.Now.ToString(), route_address.ToString());
                //string datastr = "";
                byte[] data = new byte[10*1024];
                int bytes;
                bytes = conn.Receive(data, data.Length, 0);//从客户端接受信息 
                //datastr += Encoding.ASCII.GetString(recvBytes, 0, bytes);
                conn.Close();
                Packet now_packet = Packet.from_bytes(data);

                string Str = "";
                for(int i=0;i<data.Length;i++)
                {
                    Str += data[i].ToString("X2");
                }
                //Console.WriteLine("Checksum is {0}",Str);
                string Str1 = "";
                for (int i = 0; i < now_packet.Checksum.Length; i++)
                {
                    Str1 += now_packet.Checksum[i].ToString("X2");
                }
                Console.WriteLine("Checksum is {0}", Str1);

                this.receiving_queue.Enqueue(Tuple.Create<IPAddress,Packet>(route_address,now_packet));
                Console.WriteLine("[LOG]{0}: receive a {1} packet which uuid is {2} from {3}", DateTime.Now.ToString(), Encoding.UTF8.GetString(now_packet.get_command_string()), Encoding.UTF8.GetString(now_packet.Packet_uuid), route_address.ToString());
            }
        }

        public void stop()
        {
            this.threading_running = false;
        }

    }
}
