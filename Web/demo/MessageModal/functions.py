from extern_code import display_plane_number
from app.websockettest import *

import json
def plane_number_function(worker, plane_state, plane_number):
    msg =  'the %s plane number is %s' % (plane_state, plane_number)
    send_message(msg.encode('utf-8'))
    # display_plane_number(plane_state, plane_number)

def plane_location_function(worker, plane_id, plane_location_x, plane_location_y, plane_location_z):
    msg = 'id %s %s %s %s' % (plane_id, plane_location_x, plane_location_y, plane_location_z)
    print msg
    msg = {}
    msg['plane_id'] = plane_id
    msg['plane_location_x'] = plane_location_x
    msg['plane_location_y'] = plane_location_y
    msg['plane_location_z'] = plane_location_z
    send_message(json.dumps(msg).encode('utf-8'))
    # send_message(msg.encode('utf-8'))