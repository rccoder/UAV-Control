import threading
import socket
import time
from ..packet.packet import Packet

DEBUG = True

class ReceivingController(threading.Thread):
    def __init__(self, queue):
        super(ReceivingController, self).__init__()
        self.receiving_queue = queue
        self.thread_running = True
        self.s = socket.socket()
        self.s.setsockopt(socket.SOL_SOCKET, socket.SO_REUSEADDR, 1)

    def run(self):
        self.thread_running = True
        self.s.bind(('', 2333))
        self.s.listen(5)
        while self.thread_running:
            conn, route_address = self.s.accept()
            if DEBUG:
                print '[LOG][%s]: connect by %s' % (time.ctime(), route_address)
            data = conn.recv(10 * 1024)
            conn.close()
            now_packet = Packet.from_bytes(data)
            self.receiving_queue.put((route_address[0], now_packet))
            print '[LOG][%s]: receive a %s packet which uuid is %s from %s' % (
                time.ctime(), now_packet.get_command_string(), now_packet.uuid.encode('hex'), route_address)

    def stop(self):
        self.thread_running = False
