import unittest
import json

from utils import utils


class TestUtils(unittest.TestCase):
    def setUp(self):
        pass

    def tearDown(self):
        pass

    def test_int2bytes(self):
        self.assertEqual(utils.int2bytes(0x12345678), '78563412'.decode('hex'), 'test int2bytes fail')
        self.assertEqual(utils.int2bytes(0xabcdef12), '12efcdab'.decode('hex'), 'test int2bytes fail')
        self.assertEqual(utils.int2bytes(0x0209), '09020000'.decode('hex'), 'test int2bytes fail')

    def test_bytes2int(self):
        self.assertEqual(utils.bytes2int('78563412'.decode('hex')), 0x12345678, 'test bytes2int fail')
        self.assertEqual(utils.bytes2int('12efcdab'.decode('hex')), 0xabcdef12, 'test bytes2int fail')
        self.assertEqual(utils.bytes2int('7b000000'.decode('hex')), 123, 'test bytes2int fail')

    def test_padding_by_zero(self):
        self.assertEqual(utils.padding_by_zero('POST', 8), 'POST\0\0\0\0', "test padding by zero fail")
        self.assertEqual(utils.padding_by_zero('GETDATA', 8), 'GETDATA\0', "test padding by zero fail")
        self.assertEqual(utils.padding_by_zero('DATA', 8), 'DATA\0\0\0\0', "test padding by zero fail")
        self.assertEqual(utils.padding_by_zero('ACK', 8), 'ACK\0\0\0\0\0', "test padding by zero fail")

    def test_cut_tail_zero(self):
        self.assertEqual(utils.cut_tail_zero('POST\0\0\0\0'), 'POST', "test cut tail zero fail")
        self.assertEqual(utils.cut_tail_zero('GETDATA\0'), 'GETDATA', "test cut tail zero fail")
        self.assertEqual(utils.cut_tail_zero('DATA\0\0\0\0'), 'DATA', "test cut tail zero fail")
        self.assertEqual(utils.cut_tail_zero('ACK\0\0\0\0\0'), 'ACK', "test cut tail zero fail")

    def test_gen_command_bytes_array(self):
        self.assertEqual(utils.gen_command_bytes_array('POST'), 'POST\0\0\0\0', "test gen command bytes array fail")
        self.assertEqual(utils.gen_command_bytes_array('GETDATA'), 'GETDATA\0', "test gen command bytes array fail")
        self.assertEqual(utils.gen_command_bytes_array('DATA'), 'DATA\0\0\0\0', "test gen command bytes array fail")
        self.assertEqual(utils.gen_command_bytes_array('ACK'), 'ACK\0\0\0\0\0', "test gen command bytes array fail")

    def test_gen_aes_key(self):
        self.assertEqual(len(utils.gen_aes_key_hex()), 32,
                         'test gen command bytes array fail, the length is %d' % len(utils.gen_aes_key_hex()))

    def test_encrypt_and_decrypt(self):
        key_hex = utils.gen_aes_key_hex()
        content = json.dumps({'command': 'test'})
        cipher_string = utils.json_encrypt(key_hex, content)
        after = utils.json_decrypt(key_hex, cipher_string)
        self.assertEqual(after, content, 'test encrypt and decrypt fail')


if __name__ == '__main__':
    unittest.main()
