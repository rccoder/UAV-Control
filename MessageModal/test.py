from worker import Worker
from config.ROUTE_ADDRESS import route_address

my_worker = Worker()


def main():
    global my_worker
    my_worker.start()
    query_plane_number()


def query_plane_number():
    global my_worker
    data_dict = {
        'command': 'get_plane_number',
        'options': {
            'state': 'all'
        }
    }
    my_worker.send_data_packet(route_address, data_dict)


if __name__ == '__main__':
    main()
