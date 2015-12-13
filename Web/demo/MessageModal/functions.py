from extern_code import display_plane_number
from app.websockettest import *


def plane_number_function(worker, plane_state, plane_number):
    msg =  'the %s plane number is %s' % (plane_state, plane_number)
    send_message(msg.encode('utf-8'))
    # display_plane_number(plane_state, plane_number)
