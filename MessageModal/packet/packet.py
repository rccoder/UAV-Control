from ..utils import utils
import hashlib
import uuid


class Packet(object):
    def __init__(self, version, command, length, packet_uuid, checksum, payload):
        self.version = version
        self.command = command
        self.length = length
        self.uuid = packet_uuid
        self.checksum = checksum
        self.payload = payload

    @classmethod
    def from_bytes(cls, bytes_array):
        version = bytes_array[:4]
        command = bytes_array[4:12]
        length = bytes_array[12:16]
        packet_uuid = bytes_array[16:32]
        checksum = bytes_array[32:40]
        payload = bytes_array[40:]
        return cls(version, command, length, packet_uuid, checksum, payload)

    @classmethod
    def gen_packet(cls, command, payload):
        version = utils.int2bytes(0x0209)
        command = utils.gen_command_bytes_array(command)
        length = utils.int2bytes(len(payload))
        packet_uuid = uuid.uuid4().bytes
        checksum = hashlib.sha256(payload).digest()[:8]
        return cls(version, command, length, packet_uuid, checksum, payload)

    def to_bytes(self):
        return ''.join([self.version, self.command, self.length, self.uuid, self.checksum, self.payload])

    def get_command_string(self):
        return utils.cut_tail_zero(self.command)

    def __eq__(self, other):
        if self.version != other.version:
            return False
        if self.command != other.command:
            return False
        if self.length != other.length:
            return False
        if self.uuid != other.uuid:
            return False
        if self.checksum != other.checksum:
            return False
        if self.payload != other.payload:
            return False
        return True


class VersionException(Exception):
    pass


class CommandException(Exception):
    pass


class LengthException(Exception):
    pass


class ChecksumException(Exception):
    pass
