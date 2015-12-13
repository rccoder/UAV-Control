from dwebsocket.decorators import accept_websocket

clients = []

@accept_websocket
def echo(request):
    if request.is_websocket:
        print 2222
        try:
            clients.append(request.websocket)
            print '1111'
            for message in request.websocket:
                print message
                if not message:
                    break
                for client in clients:
                    print client
                    client.send(message)
        finally:
            clients.remove(request.websocket)

def send_message(msg):
    for client in clients:
        client.send(msg)