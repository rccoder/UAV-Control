using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Security.Cryptography;
using MessageModel.utils;
namespace MessageModel.packet
{
    class Packet
    {
        private byte[] version;
        private byte[] command;
        private byte[] length;
        private byte[] packet_uuid;
        private byte[] checksum;
        private byte[] payload;

        public byte[] Version
        {
            get
            {
                return version;
            }
        }

        public byte[] Command
        {
            get
            {
                return command;
            }
        }

        public byte[] Length
        {
            get
            {
                return length;
            }
        }

        public byte[] Packet_uuid
        {
            get
            {
                return packet_uuid;
            }
        }

        public byte[] Checksum
        {
            get
            {
                return checksum;
            }
        }

        public byte[] Payload
        {
            get
            {
                return payload;
            }
        }

        public Packet(byte[] version, byte[] command, byte[] length, byte[] packet_uuid, byte[] checksum, byte[] payload)
        {
            this.version = version;
            this.command = command;
            this.length = length;
            this.packet_uuid = packet_uuid;
            this.checksum = checksum;
            this.payload = payload;
        }
        public static Packet from_bytes(byte[] bytes_array)
        {
            byte[] version = new byte[4];
            byte[] command = new byte[8];
            byte[] length = new byte[4];
            byte[] packet_uuid = new byte[16];
            byte[] checksum = new byte[8];
            
            Buffer.BlockCopy(bytes_array, 0, version, 0, 4);
            Buffer.BlockCopy(bytes_array, 4, command, 0, 8);
            Buffer.BlockCopy(bytes_array, 12, length, 0, 4);
            Buffer.BlockCopy(bytes_array, 16, packet_uuid, 0, 16);
            Buffer.BlockCopy(bytes_array, 32, checksum, 0, 8);
            byte[] payload = new byte[Utils.bytes2int(length)];
            Buffer.BlockCopy(bytes_array, 40, payload, 0, Utils.bytes2int(length));
            Packet p = new Packet(version,command,length,packet_uuid,checksum,payload);
            return p;
        }
        public static Packet gen_packet(byte[] com,byte[] payl)
        {
            byte[] version = utils.Utils.int2bytes(0x0209);
            byte[] command = utils.Utils.gen_command_bytes_array(com);
            byte[] length = utils.Utils.int2bytes(payl.Length);
            byte[] packet_uuid = Guid.NewGuid().ToByteArray();
            byte[] checksum_temp = Encoding.UTF8.GetBytes(utils.HashHelper.Hash_SHA_256(Encoding.UTF8.GetString(payl)));
            byte[] checksum = new byte[8];
            Buffer.BlockCopy(checksum_temp, 0, checksum, 0, 8);
            Console.WriteLine(Encoding.UTF8.GetString(checksum));
            Console.WriteLine(Encoding.UTF8.GetString(payl));
            Console.WriteLine(payl.Length);
            Packet p = new Packet(version, command, length, packet_uuid, checksum, payl);
            return p;
        }
        public byte[] to_bytes()
        {
            byte[] ret = new byte[40+this.payload.Length];
            Buffer.BlockCopy(this.version, 0, ret, 0, 4);
            Buffer.BlockCopy(this.command, 0, ret, 4, 8);
            Buffer.BlockCopy(this.length, 0, ret, 12, 4);
            Buffer.BlockCopy(this.packet_uuid, 0, ret, 16, 16);
            Buffer.BlockCopy(this.checksum, 0, ret, 32, 8);
            Buffer.BlockCopy(this.payload, 0, ret, 40, this.payload.Length);
            return ret;
        }
        public byte[] get_command_string()
        {
            return utils.Utils.cut_tail_zero(this.command);
        }
        public bool __eq__(Packet p)
        {
            if (this.version != p.version)
                return false;
            if (this.command != p.command)
                return false;
            if (this.length != p.length)
                return false;
            if (this.packet_uuid != p.packet_uuid)
                return false;
            if (this.checksum != p.checksum)
                return false;
            if (this.payload != p.payload)
                return false;
            return true;
        }

    }

    class VersionExcepion:Exception
    {
    }
    class CommandException:Exception
    {
    }
    class LengthException:Exception
    {
    }
    class ChecksumExcepiton:Exception
    {
    }
}
