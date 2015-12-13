import unittest
from packet import packet
from utils import utils
import hashlib


class TestUtils(unittest.TestCase):
    def setUp(self):
        self.post_packet = packet.Packet.gen_packet('POST', '{}')
        self.getdata_packet = packet.Packet.gen_packet('GETDATA', '{}')
        self.data_packet = packet.Packet.gen_packet('DATA', '{}')
        self.ack_packet = packet.Packet.gen_packet('ACK', '{}')

    def test_to_bytes(self):
        self.assertEqual(self.post_packet.to_bytes(), ''.join(
            [utils.int2bytes(0x0209), utils.gen_command_bytes_array('POST'), utils.int2bytes(2), self.post_packet.uuid,
             hashlib.sha256('{}').digest()[:8], '{}']))
        self.assertEqual(self.getdata_packet.to_bytes(), ''.join(
            [utils.int2bytes(0x0209), utils.gen_command_bytes_array('GETDATA'), utils.int2bytes(2),
             self.getdata_packet.uuid, hashlib.sha256('{}').digest()[:8], '{}']))
        self.assertEqual(self.data_packet.to_bytes(), ''.join(
            [utils.int2bytes(0x0209), utils.gen_command_bytes_array('DATA'), utils.int2bytes(2), self.data_packet.uuid,
             hashlib.sha256('{}').digest()[:8], '{}']))
        self.assertEqual(self.ack_packet.to_bytes(), ''.join(
            [utils.int2bytes(0x0209), utils.gen_command_bytes_array('ACK'), utils.int2bytes(2), self.ack_packet.uuid,
             hashlib.sha256('{}').digest()[:8], '{}']))

    def test_from_bytes(self):
        self.assertEqual(packet.Packet.from_bytes(self.post_packet.to_bytes()), self.post_packet,
                         'test packet from bytes fail')

    def test_get_command_string(self):
        self.assertEqual(self.post_packet.get_command_string(), 'POST', 'test get command string fail')


if __name__ == '__main__':
    unittest.main()
