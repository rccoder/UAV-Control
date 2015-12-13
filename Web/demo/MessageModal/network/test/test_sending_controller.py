import unittest
import json
import Queue
import socket
import threading
import time
from network import sending_controller
from packet import packet


class SocketServer(threading.Thread):
    def __init__(self):
        super(SocketServer, self).__init__()
        self.thread_running = False
        self.data = []
        self.s = socket.socket()

    def run(self):
        self.thread_running = True
        self.s = socket.socket()
        self.s.setsockopt(socket.SOL_SOCKET, socket.SO_REUSEADDR, 1)
        self.s.bind(('127.0.0.1', 2333))
        self.s.listen(3)
        while self.thread_running:
            conn, route_address = self.s.accept()
            print '%s : connect by %s' % (time.ctime(), route_address)
            self.data.append(conn.recv(1024 * 10))
            conn.close()

    def stop(self):
        self.thread_running = False
        self.s.close()


class TestSendingController(unittest.TestCase):
    def setUp(self):
        self.data_packet = packet.Packet.gen_packet('DATA', json.dumps({'command': 'test'}))
        self.post_packet = packet.Packet.gen_packet('POST', json.dumps({'uuid': '2e0b47c8d88f430d9305c757385f0147'}))
        self.queue = Queue.Queue()
        self.test_socket_server = SocketServer()
        self.test_sending_controller = sending_controller.SendingController(self.queue)
        self.test_socket_server.setDaemon(True)
        self.test_socket_server.start()
        self.test_sending_controller.start()

    def test_send_to(self):
        self.queue.put(('127.0.0.1', self.data_packet))
        self.queue.put(('127.0.0.1', self.post_packet))
        time.sleep(1)
        self.assertEqual(self.test_socket_server.data[0], self.data_packet.to_bytes(),
                         'test send to fail, the message is %s, but received %s' % (
                             self.data_packet.to_bytes().encode('hex'), self.test_socket_server.data[0].encode('hex')))
        self.assertEqual(self.test_socket_server.data[1], self.post_packet.to_bytes(),
                         'test send to fail, the message is %s, but received %s' % (
                             self.post_packet.to_bytes().encode('hex'), self.test_socket_server.data[1].encode('hex')))
        self.test_sending_controller.stop()
        self.test_socket_server.stop()


if __name__ == '__main__':
    unittest.main()
