from django.conf.urls import *
from django.shortcuts import render_to_response
from django.template import RequestContext
from dwebsocket.decorators import accept_websocket
#from conf.settings import worker
# Uncomment the next two lines to enable the admin:
# from django.contrib import admin
# admin.autodiscover()

def base_view(request):
    return render_to_response('index.html', {

    }, context_instance=RequestContext(request))


clients = []

@accept_websocket
def echo(request):
    if request.is_websocket:
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

def test(request):
    for client in clients:
        client.send('test')

urlpatterns = patterns('',
    # Example:
    url(r'^$', base_view),
    url(r'^test$', test),
    url(r'^echo$', echo),

    # Uncomment the admin/doc line below and add 'django.contrib.admindocs' 
    # to INSTALLED_APPS to enable admin documentation:
    # (r'^admin/doc/', include('django.contrib.admindocs.urls')),

    # Uncomment the next line to enable the admin:
    # (r'^admin/', include(admin.site.urls)),
)
