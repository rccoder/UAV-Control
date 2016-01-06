$(function() {

    //websocket相关
    window.onload = function() {
        if (window.s) {
            window.s.close()
        }
        var s = new WebSocket("ws://" + window.location.host + "/echo");
        s.onopen = function() {
            console.log('WebSocket open');
        };
        s.onmessage = function(e) {
            console.log('message: ' + e.data);
            $('#messagecontainer').prepend('<p>' + e.data + '</p>');
            var eJson = JSON.parse(e.data);
            var _location = [];
            _location.push(eJson.plane_location_x);
            _location.push(eJson.plane_location_y);
            console.log(_location)
            console.log(2222)
            new AMap.Marker({
                map: map,
                icon: "http://webapi.amap.com/theme/v1.3/markers/n/mark_b2.png",
                position: _location,
                offset: new AMap.Pixel(-12, -36)
            });
        };
        window.s = s;
    };
    $('#test_websocket').click(function() {
        if (!window.s) {
            alert("Please connect server.");
        } else {
            window.s.send('WebSocket 连接成功');
        }
    });
    $('#close_websocket').click(function() {
        if (window.s) {
            window.s.close();
        }
    });

    //启动进程
    $('#start_progress').click(function() {
        $.ajax({
            method: 'GET',
            data: '',
            url: '/start',
            success: function(msg) {
                $('#messagecontainer').prepend('<p>' + '进程启动成功' + '</p>');
            }
        })
    });

    //测试进程连接
    $('#test_progress').click(function() {
        $.ajax({
            method: 'GET',
            data: '',
            url: '/action',
            success: function(msg) {

            }
        });
    });

    // 关闭进程
    $('#stop_progress').click(function() {
        $.ajax({
            method: 'GET',
            data: '',
            url: '/stop',
            success: function (msg) {
                $('#messagecontainer').prepend('<p>' + '进程关闭成功' + '</p>');
            }
        })
    });

    $("#get_location").click(function () {
        $.ajax({
            method: 'GET',
            data: '',
            url: '/get_location',
            success: function (msg) {
                $('#messagecontainer').prepend('<p>' + '进程关闭成功' + '</p>');
            }
        })
    });

});
