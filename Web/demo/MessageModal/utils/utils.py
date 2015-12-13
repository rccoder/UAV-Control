import hashlib
import random
from Crypto.Cipher import AES


def int2bytes(number):
    if number < 0 or number > 2 ** 32 - 1:
        raise ValueError, 'the number must between 0 to 2 ** 32 - 1!'
    mask = 0x000000ff
    num_list = [(number >> x) & mask for x in [8 * y for y in xrange(4)]]
    return ('%02x%02x%02x%02x' % tuple(num_list)).decode('hex')


def bytes2int(bytes_int):
    if len(bytes_int) != 4:
        raise ValueError, 'the bytes list must be 4 bytes!'
    bytes_hex = bytes_int.encode('hex')
    num_list = map(lambda x: int(''.join(x), 16), zip(bytes_hex[::2], bytes_hex[1::2]))
    return num_list[0] + (num_list[1] << 8) + (num_list[2] << 16) + (num_list[3] << 24)


def padding_by_zero(bytes_array, length):
    if len(bytes_array) > length:
        raise ValueError, 'length is smaller than length of bytes array'
    return bytes_array + ('\0' * (length - len(bytes_array)))


def cut_tail_zero(bytes_array):
    i = 0
    for letter in bytes_array[::-1]:
        if letter is '\0':
            i += 1
        else:
            break
    return bytes_array[:len(bytes_array) - i]


def gen_command_bytes_array(command):
    command = command.upper()
    if command not in ['POST', 'GETDATA', 'DATA', 'ACK']:
        raise ValueError, 'the command must be POST, GETDATA, DATA or ACK'
    return padding_by_zero(command, 8)


def gen_aes_key_hex():
    result = ''
    m = hashlib.md5()
    for i in xrange(1):
        m.update(str(random.random()))
        result += m.digest()
    return result.encode('hex')


def json_encrypt(key_hex, json_string):
    obj = AES.new(key_hex.decode('hex'))
    length = len(json_string)
    last_length = length + (16 - (length % 16))
    return obj.encrypt(padding_by_zero(json_string, last_length))


def json_decrypt(key_hex, cipher_text):
    obj = AES.new(key_hex.decode('hex'))
    return cut_tail_zero(obj.decrypt(cipher_text))
