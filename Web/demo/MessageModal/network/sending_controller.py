import threading
import socket
import time

DEBUG = True


class SendingController(threading.Thread):
    def __init__(self, queue):
        super(SendingController, self).__init__(name='SendingController')
        self.sending_queue = queue
        self.thread_running = True

    def run(self):
        while self.thread_running:
            if not self.sending_queue.empty():
                target_url, packet = self.sending_queue.get()
                self.sent_to(target_url, packet)
            else:
                time.sleep(0.1)

    def stop(self):
        self.thread_running = False

    @staticmethod
    def sent_to(target_url, packet):
        s = socket.socket()
        s.setsockopt(socket.SOL_SOCKET, socket.SO_REUSEADDR, 1)
        s.connect((target_url, 2333))
        s.sendall(packet.to_bytes())
        s.close()
        if DEBUG:
            print '[LOG][%s]: send a %s packet which uuid is %s to %s' % (
                time.ctime(), packet.get_command_string(), packet.uuid.encode('hex'), target_url)
