using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Security.Cryptography;
using System.Collections;
using MessageModel.network;
using MessageModel.packet;
using MessageModel.utils;
using System.Threading;
using System.Net.Sockets;
using System.Net;
using Newtonsoft.Json;
using MessageModel.config;

namespace MessageModel
{
    class worker
    {
        public static bool DEBUG;
        private bool __thread_running;
        private Queue<Tuple<IPAddress, Packet>> __sending_queue;
        private Queue<Tuple<IPAddress, Packet>> __receiving_queue;
        private Dictionary<byte[], Packet> __caching_dict;
        private Receiving_Controller __receiving_controller;
        private sending_controller __sending_controller;

        public static string[] command_list = new string[] { "POST","GETDATA","DATA","ACK"};
        public worker()
        {
            __thread_running = true;
            __sending_queue = new Queue<Tuple<IPAddress, Packet>>();
            __receiving_queue = new Queue<Tuple<IPAddress, Packet>>();
            __caching_dict = new Dictionary<byte[], Packet>();
            __receiving_controller = new Receiving_Controller(__receiving_queue);
            __sending_controller = new sending_controller(__sending_queue);
        }
        public void stop()
        {
            this.__thread_running = false;
        }
        public void run()
        {
            Thread rc = new Thread(this.__receiving_controller.run);
            Thread sc = new Thread(this.__sending_controller.run);
            rc.Start();
            sc.Start();
            while (this.__thread_running)
            {
                if (this.__receiving_queue.Count != 0)
                {
                    Tuple<IPAddress, Packet> tup = this.__receiving_queue.Dequeue();
                    IPAddress route_address = tup.Item1;
                    Packet now_packet = tup.Item2;
                    try
                    {
                        __confirm_packet(now_packet);
                    }
                    catch(packet.VersionExcepion e)
                    {
                        Console.WriteLine("{0} is not an available version",utils.Utils.bytes2int(now_packet.Version));
                    }
                    catch(packet.CommandException e)
                    {
                        Console.WriteLine("{0} is not an available command", now_packet.get_command_string());
                    }
                    catch(packet.ChecksumExcepiton e)
                    {
                        if (now_packet.get_command_string().Equals(Encoding.UTF8.GetBytes("DATA")))
                        {
                            Console.WriteLine("checksum is not available, ready to resend a GETDATA packet");
                            this.__send_get_data_packet(route_address, now_packet.Packet_uuid);
                        }
                        else
                            Console.WriteLine("checksum is not available, drop the packet");
                    }
                    if (now_packet.get_command_string().Equals(Encoding.UTF8.GetBytes("DATA")))
                        this.__send_ack_packet(route_address,now_packet.Packet_uuid);
                    this.__handle_packet(route_address,now_packet);
                }
                else
                    Thread.Sleep(100);
            }
        }

        public static bool __confirm_packet(Packet confirmation_packet)
        {
            if (utils.Utils.bytes2int(confirmation_packet.Version) != 0x0209)
                throw new packet.VersionExcepion();
            if (command_list.Contains(Encoding.UTF8.GetString(confirmation_packet.get_command_string())))
                throw new packet.CommandException();
            byte[] check_bytes = Encoding.UTF8.GetBytes(utils.HashHelper.Hash_SHA_256(Encoding.UTF8.GetString(confirmation_packet.Payload)));
            byte[] check_bytes_8 = new byte[8];
            Buffer.BlockCopy(check_bytes, 0, check_bytes_8, 0, 8);
            if (check_bytes_8.Equals(confirmation_packet.Checksum))
                throw new packet.ChecksumExcepiton();
            return true;
        }

        public void __send_get_data_packet(IPAddress route_address, byte[] packet_uuid_hex)
        {
            JsonObject pay_load = new JsonObject();
            pay_load.uuid = packet_uuid_hex;
            pay_load.timestamp = DateTime.Now.ToString();
            byte[] encrypt_data = utils.Utils.json_encrypt(config.SECRET_KEY.secret_key, Encoding.UTF8.GetBytes(JsonConvert.SerializeObject(pay_load)));
            Packet get_data_packet = Packet.gen_packet(Encoding.UTF8.GetBytes("GETDATA"), encrypt_data);
            __sending_queue.Enqueue(Tuple.Create<IPAddress, Packet>(route_address, get_data_packet));
        }
        public void send_data_packet(IPAddress route_address, JsonDataObject data_dict)
        {
            byte[] encrypt_data = utils.Utils.json_encrypt(config.SECRET_KEY.secret_key, Encoding.UTF8.GetBytes(JsonConvert.SerializeObject(data_dict)));
            Packet data_packet = Packet.gen_packet(Encoding.UTF8.GetBytes("DATA"), encrypt_data);
            if(DEBUG)
            {
                Console.WriteLine("[LOG][{0}]: create a DATA packet which uuid is {1}",DateTime.Now.ToString(),Encoding.UTF8.GetString(data_packet.Packet_uuid));
            }
            this.__caching_dict.Add(data_packet.Packet_uuid,data_packet);
            this.__send_post_packet(route_address,data_packet.Packet_uuid);
        }
        public void __send_post_packet(IPAddress route_address,byte[] packet_uuid_hex)
        {
            JsonObject pay_load = new JsonObject();
            pay_load.uuid = packet_uuid_hex;
            pay_load.timestamp = DateTime.Now.ToString();
            byte[] encrypt_data = utils.Utils.json_encrypt(config.SECRET_KEY.secret_key, Encoding.UTF8.GetBytes(JsonConvert.SerializeObject(pay_load)));
            Packet post_packet = Packet.gen_packet(Encoding.UTF8.GetBytes("POST"), encrypt_data);
            __sending_queue.Enqueue(Tuple.Create<IPAddress, Packet>(route_address, post_packet));
        }
        public void __send_ack_packet(IPAddress route_address, byte[] packet_uuid_hex)
        {
            JsonObject pay_load = new JsonObject();
            pay_load.uuid = packet_uuid_hex;
            pay_load.timestamp = DateTime.Now.ToString();
            byte[] encrypt_data = utils.Utils.json_encrypt(config.SECRET_KEY.secret_key, Encoding.UTF8.GetBytes(JsonConvert.SerializeObject(pay_load)));
            Packet ack_packet = Packet.gen_packet(Encoding.UTF8.GetBytes("ACK"), encrypt_data);
            __sending_queue.Enqueue(Tuple.Create<IPAddress, Packet>(route_address, ack_packet));
        }
        public void __handle_packet(IPAddress route_address,Packet now_packet)
        {
            byte[] command = now_packet.get_command_string();
            if (command.Equals(Encoding.UTF8.GetBytes("POST")))
            {
                JsonObject decrypt_post = JsonConvert.DeserializeObject<JsonObject>(Encoding.UTF8.GetString(Utils.json_decrypt(SECRET_KEY.secret_key, now_packet.Payload)));
                this.__send_get_data_packet(route_address, decrypt_post.uuid);
            }
            else if (command.Equals(Encoding.UTF8.GetBytes("ACK")))
            {
                JsonObject decrypt_ack = JsonConvert.DeserializeObject<JsonObject>(Encoding.UTF8.GetString(Utils.json_decrypt(SECRET_KEY.secret_key, now_packet.Payload)));
                if (DEBUG)
                {
                    Console.WriteLine("[LOG][{0}]: the data packet {1} has been received by {2}!", DateTime.Now.ToString(), decrypt_ack.uuid, route_address);
                }
                if (__caching_dict.ContainsKey(decrypt_ack.uuid))
                {
                    this.__caching_dict.Remove(decrypt_ack.uuid);
                }
            }
            
            else if (command.Equals(Encoding.UTF8.GetBytes("GETDATA")))
            {
                JsonObject decrypt_getdata = JsonConvert.DeserializeObject<JsonObject>(Encoding.UTF8.GetString(Utils.json_decrypt(SECRET_KEY.secret_key, now_packet.Payload)));
                this.__sending_queue.Enqueue(Tuple.Create<IPAddress, Packet>(route_address, this.__caching_dict[decrypt_getdata.uuid]));
            }
            else if (command.Equals(Encoding.UTF8.GetBytes("DATA")))
            {
                JsonDataObject decrypt_data = JsonConvert.DeserializeObject<JsonDataObject>(Encoding.UTF8.GetString(Utils.json_decrypt(SECRET_KEY.secret_key, now_packet.Payload)));
                //
            }
        }
        
    }
}
