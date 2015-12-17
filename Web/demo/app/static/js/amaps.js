 var map = new AMap.Map('container', {
        resizeEnable: true,
        center: [116.397428, 39.90923],
        zoom: 13
    });    
    map.clearMap();  // 清除地图覆盖物
    var markers = [{
        icon: 'http://webapi.amap.com/theme/v1.3/markers/n/mark_b1.png',
        position: [116.205467, 39.907761]
    }, {
        icon: 'http://webapi.amap.com/theme/v1.3/markers/n/mark_b1.png',
        position: [116.368904, 39.913423]
    }, {
        icon: 'http://webapi.amap.com/theme/v1.3/markers/n/mark_b1.png',
        position: [116.305467, 39.807761]
    }];
    // 添加一些分布不均的点到地图上,地图上添加三个点标记，作为参照
    markers.forEach(function(marker) {
        new AMap.Marker({
            map: map,
            icon: marker.icon,
            position: [marker.position[0], marker.position[1]],
            offset: new AMap.Pixel(-12, -36)
        });
    });
    var center = map.getCenter();
    var centerText = '当前中心点坐标：' + center.getLng() + ',' + center.getLat();
    document.getElementById('centerCoord').innerHTML = centerText;
    document.getElementById('tips').innerHTML = '成功添加三个点标记，其中有两个在当前地图视野外！';

    // 添加事件监听, 使地图自适应显示到合适的范围
    AMap.event.addDomListener(document.getElementById('setFitView'), 'click', function() {
        var newCenter = map.setFitView();
        document.getElementById('centerCoord').innerHTML = '当前中心点坐标：' + newCenter.getCenter();
        document.getElementById('tips').innerHTML = '通过setFitView，地图自适应显示到合适的范围内,点标记已全部显示在视野中！';
    });