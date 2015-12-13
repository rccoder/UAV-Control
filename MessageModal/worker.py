import hashlib
import json
import threading
import time
from Queue import Queue

from .config.SECRET_KEY import secret_key
from interface import interface_dict
from .network.receiving_controller import ReceivingController
from .network.sending_controller import SendingController
from .packet import packet
from .utils import utils

DEBUG = True


class Worker(threading.Thread):
    def __init__(self):
        super(Worker, self).__init__()
        self.__thread_running = True
        self.__sending_queue = Queue()
        self.__receiving_queue = Queue()
        self.__caching_dict = {}
        self.__receiving_controller = ReceivingController(self.__receiving_queue)
        self.__sending_controller = SendingController(self.__sending_queue)

    def stop(self):
        self.__thread_running = False

    def run(self):
        self.__thread_running = True
        self.__receiving_controller.setDaemon(True)
        self.__sending_controller.setDaemon(True)
        self.__receiving_controller.start()
        self.__sending_controller.start()
        while self.__thread_running:
            if not self.__receiving_queue.empty():
                route_address, now_packet = self.__receiving_queue.get()
                try:
                    self.__confirm_packet(now_packet)
                except packet.VersionException:
                    print '%s is not an available version' % utils.bytes2int(now_packet.version)
                except packet.CommandException:
                    print '%s is not an available command' % now_packet.get_command_string()
                except packet.ChecksumException:
                    if now_packet.get_command_string == 'DATA':
                        print 'checksum is not available, ready to resend a GETDATA packet'
                        self.__send_get_data_packet(route_address, now_packet.uuid.encode('hex'))
                    else:
                        print 'checksum is not available, drop the packet'
                else:
                    if now_packet.get_command_string() == 'DATA':
                        self.__send_ack_packet(route_address, now_packet.uuid.encode('hex'))
                    self.__handle_packet(route_address, now_packet)
            else:
                time.sleep(0.2)

    @staticmethod
    def __confirm_packet(confirmation_packet):
        if utils.bytes2int(confirmation_packet.version) != 0x0209:
            raise packet.VersionException
        if confirmation_packet.get_command_string() not in ['POST', 'GETDATA', 'DATA', 'ACK']:
            raise packet.CommandException
        if hashlib.sha256(confirmation_packet.payload).digest()[:8] != confirmation_packet.checksum:
            raise packet.ChecksumException
        return True

    def __send_get_data_packet(self, route_address, packet_uuid_hex):
        payload = {'uuid': packet_uuid_hex, 'timestamp': time.time() * 100}
        encrypt_data = utils.json_encrypt(secret_key, json.dumps(payload))
        get_data_packet = packet.Packet.gen_packet('GETDATA', encrypt_data)
        self.__sending_queue.put((route_address, get_data_packet))

    def send_data_packet(self, route_address, data_dict):
        encrypt_data = utils.json_encrypt(secret_key, json.dumps(data_dict))
        data_packet = packet.Packet.gen_packet('DATA', encrypt_data)
        if DEBUG:
            print '[LOG][%s]: create a DATA packet which uuid is %s' % (time.ctime(), data_packet.uuid.encode('hex'))
        self.__caching_dict[data_packet.uuid.encode('hex')] = data_packet
        self.__send_post_packet(route_address, data_packet.uuid.encode('hex'))

    def __send_post_packet(self, route_address, packet_uuid_hex):
        payload = {'uuid': packet_uuid_hex, 'timestamp': time.time() * 100}
        encrypt_data = utils.json_encrypt(secret_key, json.dumps(payload))
        post_packet = packet.Packet.gen_packet('POST', encrypt_data)
        self.__sending_queue.put((route_address, post_packet))

    def __send_ack_packet(self, route_address, packet_uuid_hex):
        payload = {'uuid': packet_uuid_hex, 'timestamp': time.time() * 100}
        encrypt_data = utils.json_encrypt(secret_key, json.dumps(payload))
        ack_packet = packet.Packet.gen_packet('ACK', encrypt_data)
        self.__sending_queue.put((route_address, ack_packet))

    def __handle_packet(self, route_address, now_packet):
        command = now_packet.get_command_string()
        decrypt_data = json.loads(utils.json_decrypt(secret_key, now_packet.payload))
        if command == 'POST':
            self.__send_get_data_packet(route_address, decrypt_data.get('uuid', '\0' * 16))
        elif command == 'ACK':
            if DEBUG:
                print '[LOG][%s]: the data packet %s has been received by %s!' % (
                    time.ctime(), decrypt_data.get('uuid'), route_address)
            if decrypt_data.get('uuid') in self.__caching_dict:
                del self.__caching_dict[decrypt_data.get('uuid')]
        elif command == 'GETDATA':
            self.__sending_queue.put((route_address, self.__caching_dict.get(decrypt_data.get('uuid'))))
        elif command == 'DATA':
            interface_dict.get(decrypt_data.get('command'))(worker=self, **decrypt_data.get('options', {}))
