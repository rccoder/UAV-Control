import unittest
import json
import Queue
import socket
import time
from network import receiving_controller
from packet import packet


def send_packet(sending_packet):
    s = socket.socket()
    s.setsockopt(socket.SOL_SOCKET, socket.SO_REUSEADDR, 1)
    s.connect(('127.0.0.1', 2333))
    s.sendall(sending_packet.to_bytes())
    s.close()


class TestReceivingController(unittest.TestCase):
    def setUp(self):
        self.data_packet = packet.Packet.gen_packet('DATA', json.dumps({'command': 'test'}))
        self.post_packet = packet.Packet.gen_packet('POST', json.dumps({'uuid': '2e0b47c8d88f430d9305c757385f0147'}))
        self.queue = Queue.Queue()
        self.test_receiving_controller = receiving_controller.ReceivingController(self.queue)
        self.test_receiving_controller.setDaemon(True)
        self.test_receiving_controller.start()

    def tearDown(self):
        self.test_receiving_controller.stop()

    def test_run(self):
        send_packet(self.post_packet)
        time.sleep(1)
        temp_packet = self.queue.get()
        self.assertEqual(temp_packet[0], '127.0.0.1', 'test run fail, the address is %s' % temp_packet[0])
        self.assertEqual(temp_packet[1].to_bytes(), self.post_packet.to_bytes(),
                         'test run fail, the packet should be %s, but %s received' % (
                             temp_packet[1].to_bytes().encode('hex'), self.post_packet.to_bytes().encode('hex')))

        send_packet(self.data_packet)
        time.sleep(1)
        temp_packet = self.queue.get()
        self.assertEqual(temp_packet[0], '127.0.0.1', 'test run fail, the address is %s' % temp_packet[0])
        self.assertEqual(temp_packet[1].to_bytes(), self.data_packet.to_bytes(),
                         'test run fail, the packet should be %s, but %s received' % (
                             temp_packet[1].to_bytes().encode('hex'), self.data_packet.to_bytes().encode('hex')))


if __name__ == '__main__':
    unittest.main()
