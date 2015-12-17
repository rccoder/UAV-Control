from django.shortcuts import render
from django.http import HttpResponse
# Create your views here.
from MessageModal.worker import *
from MessageModal.config.ROUTE_ADDRESS import *

my_worker = Worker()

def base_view(request):
    return render(request, 'app/index.html', {})


def start(request):
    my_worker.start()
    return HttpResponse('ok')

def stop(request):
    my_worker.stop()
    return HttpResponse('ok')

def action(request):
    data_dict = {
        'command': 'get_plane_number',
        'options': {
            'state': 'all'
        }
    }
    my_worker.send_data_packet(route_address, data_dict)
