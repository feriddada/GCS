using Microsoft.Web.WebView2.Core;
using System;
using System.Globalization;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;

namespace GCS.Views;

public partial class MapView : UserControl
{
    private bool _isMapInitialized = false;
    private double _lastLatitude = 40.4093;
    private double _lastLongitude = 49.8671;
    private GCS.ViewModels.MissionViewModel? _missionVm;

    public MapView()
    {
        InitializeComponent();
        DataContextChanged += OnDataContextChanged;
        Loaded += OnLoaded;
    }

    private void OnLoaded(object sender, RoutedEventArgs e)
    {
        var window = Window.GetWindow(this);
        if (window?.DataContext is GCS.ViewModels.MainViewModel mainVm)
        {
            _missionVm = mainVm.Mission;
            _missionVm.WaypointsCleared += OnWaypointsCleared;
            _missionVm.WaypointAdded += OnWaypointAdded;
            _missionVm.WaypointUpdated += OnWaypointUpdated;
            _missionVm.WaypointsRebuilt += OnWaypointsRebuilt;
        }
    }

    private void OnWaypointsCleared() => ExecuteScript("clearWaypoints();");

    private void OnWaypointAdded(GCS.ViewModels.MissionItemViewModel wp)
    {
        string type = wp.CommandName;
        string script = string.Format(CultureInfo.InvariantCulture,
            "addWaypoint({0:F7}, {1:F7}, {2}, '{3}', {4:F1});",
            wp.Latitude, wp.Longitude, wp.Sequence, type, wp.Radius);
        ExecuteScript(script);
    }

    private void OnWaypointUpdated(GCS.ViewModels.MissionItemViewModel wp)
    {
        string type = wp.CommandName;
        string script = string.Format(CultureInfo.InvariantCulture,
            "updateWaypoint({0}, {1:F7}, {2:F7}, '{3}', {4:F1});",
            wp.Sequence, wp.Latitude, wp.Longitude, type, wp.Radius);
        ExecuteScript(script);
    }

    private void OnWaypointsRebuilt() => ExecuteScript("updatePathLine();");

    private async void ExecuteScript(string script)
    {
        if (!_isMapInitialized || MapWebView?.CoreWebView2 == null) return;
        try { await MapWebView.CoreWebView2.ExecuteScriptAsync(script); }
        catch (Exception ex) { System.Diagnostics.Debug.WriteLine($"[MapView] Script error: {ex.Message}"); }
    }

    private async void MapWebView_Loaded(object sender, RoutedEventArgs e)
    {
        try
        {
            await InitializeWebView();
        }
        catch (Exception ex)
        {
            MessageBox.Show($"Map initialization failed: {ex.Message}", "Error", MessageBoxButton.OK, MessageBoxImage.Error);
        }
    }

    private async Task InitializeWebView()
    {
        await MapWebView.EnsureCoreWebView2Async(null);
        MapWebView.CoreWebView2.WebMessageReceived += OnWebMessageReceived;
        MapWebView.NavigateToString(GetMapHtml());
        MapWebView.CoreWebView2.NavigationCompleted += OnNavigationCompleted;
    }

    private void OnWebMessageReceived(object? sender, CoreWebView2WebMessageReceivedEventArgs e)
    {
        try
        {
            string message = e.TryGetWebMessageAsString();

            if (message.StartsWith("click:"))
            {
                var coords = message.Substring(6).Split(',');
                if (coords.Length == 2 &&
                    double.TryParse(coords[0], NumberStyles.Float, CultureInfo.InvariantCulture, out double lat) &&
                    double.TryParse(coords[1], NumberStyles.Float, CultureInfo.InvariantCulture, out double lon))
                {
                    _missionVm?.AddWaypoint(lat, lon);
                }
            }
            else if (message.StartsWith("drag:"))
            {
                // Format: drag:index,lat,lon
                var parts = message.Substring(5).Split(',');
                if (parts.Length == 3 &&
                    int.TryParse(parts[0], out int index) &&
                    double.TryParse(parts[1], NumberStyles.Float, CultureInfo.InvariantCulture, out double lat) &&
                    double.TryParse(parts[2], NumberStyles.Float, CultureInfo.InvariantCulture, out double lon))
                {
                    _missionVm?.UpdateWaypointPosition(index, lat, lon);
                }
            }
        }
        catch (Exception ex)
        {
            System.Diagnostics.Debug.WriteLine($"[MapView] WebMessage error: {ex.Message}");
        }
    }

    private void OnNavigationCompleted(object? sender, CoreWebView2NavigationCompletedEventArgs e)
    {
        if (e.IsSuccess)
        {
            _isMapInitialized = true;
            if (LoadingOverlay != null) LoadingOverlay.Visibility = Visibility.Collapsed;

            if (DataContext is GCS.ViewModels.TelemetryViewModel vm)
                UpdateUAVPosition(vm.Latitude, vm.Longitude, vm.Altitude, vm.Groundspeed, vm.Airspeed);
        }
    }

    private void OnDataContextChanged(object sender, DependencyPropertyChangedEventArgs e)
    {
        if (e.OldValue is GCS.ViewModels.TelemetryViewModel oldVm)
            oldVm.PropertyChanged -= OnTelemetryPropertyChanged;
        if (e.NewValue is GCS.ViewModels.TelemetryViewModel newVm)
            newVm.PropertyChanged += OnTelemetryPropertyChanged;
    }

    private void OnTelemetryPropertyChanged(object? sender, System.ComponentModel.PropertyChangedEventArgs e)
    {
        if (e.PropertyName == nameof(GCS.ViewModels.TelemetryViewModel.Latitude) ||
            e.PropertyName == nameof(GCS.ViewModels.TelemetryViewModel.Longitude))
        {
            if (DataContext is GCS.ViewModels.TelemetryViewModel vm)
                UpdateUAVPosition(vm.Latitude, vm.Longitude, vm.Altitude, vm.Groundspeed, vm.Airspeed);
        }
    }

    public void UpdateUAVPosition(double lat, double lon, double alt, double groundSpeed, double airSpeed)
    {
        if (!_isMapInitialized || MapWebView?.CoreWebView2 == null) return;
        if (Math.Abs(lat - _lastLatitude) < 0.00001 && Math.Abs(lon - _lastLongitude) < 0.00001) return;

        _lastLatitude = lat;
        _lastLongitude = lon;

        string script = string.Format(CultureInfo.InvariantCulture,
            "updateUAV({0:F7}, {1:F7}, {2:F2}, {3:F2}, {4:F2});",
            lat, lon, alt, groundSpeed, airSpeed);
        ExecuteScript(script);
    }

    private string GetMapHtml()
    {
        return @"<!DOCTYPE html>
<html>
<head>
    <meta charset='utf-8'>
    <link rel='stylesheet' href='https://unpkg.com/leaflet@1.9.4/dist/leaflet.css' />
    <script src='https://unpkg.com/leaflet@1.9.4/dist/leaflet.js'></script>
    <style>
        html, body, #map { width: 100%; height: 100%; margin: 0; padding: 0; }
        .distance-label {
            background: rgba(0,0,0,0.7);
            border: none;
            border-radius: 3px;
            color: #58A6FF;
            font-size: 10px;
            font-weight: bold;
            padding: 2px 5px;
            white-space: nowrap;
        }
    </style>
</head>
<body>
    <div id='map'></div>
    <script>
        var map, uavMarker, followUAV = true, userMovedMap = false;
        var waypointMarkers = [], waypointCircles = [], waypointLine = null, distanceLabels = [];

        function initMap() {
            map = L.map('map', { center: [40.4093, 49.8671], zoom: 15 });
            L.tileLayer('https://{s}.tile.openstreetmap.org/{z}/{x}/{y}.png', {
                attribution: '© OpenStreetMap', maxZoom: 19
            }).addTo(map);

            var uavIcon = L.divIcon({
                className: 'uav-icon',
                html: '<div id=""uav-arrow"" style=""width:0;height:0;border-left:12px solid transparent;border-right:12px solid transparent;border-bottom:24px solid #FF9500;filter:drop-shadow(0 2px 4px rgba(0,0,0,0.3));""></div>',
                iconSize: [24, 24], iconAnchor: [12, 12]
            });
            uavMarker = L.marker([40.4093, 49.8671], { icon: uavIcon }).addTo(map);

            map.on('dragstart', function() { userMovedMap = true; followUAV = false; updateFollowButton(); });
            map.on('click', function(e) {
                window.chrome.webview.postMessage('click:' + e.latlng.lat + ',' + e.latlng.lng);
            });

            createFollowButton();
        }

        function getColor(type) {
            switch(type) {
                case 'HOME': return '#39D0D8';
                case 'TKOF': return '#3FB950';
                case 'LAND': return '#F85149';
                case 'LOIT': return '#FF9500';
                case 'RTL':  return '#A371F7';
                default:     return '#58A6FF';
            }
        }

        function addWaypoint(lat, lon, index, type, radius) {
            var color = getColor(type);
            
            // Circle marker (draggable)
            var svg = '<svg width=""32"" height=""32"" viewBox=""0 0 32 32""><circle cx=""16"" cy=""16"" r=""14"" fill=""' + color + '"" stroke=""white"" stroke-width=""3""/><text x=""16"" y=""21"" text-anchor=""middle"" fill=""white"" font-size=""12"" font-weight=""bold"">' + index + '</text></svg>';
            var icon = L.divIcon({ className: 'wp-marker', html: svg, iconSize: [32, 32], iconAnchor: [16, 16] });
            
            var marker = L.marker([lat, lon], { icon: icon, draggable: true }).addTo(map);
            marker.wpIndex = index;
            marker.bindPopup('<b>' + type + ' #' + index + '</b><br>Lat: ' + lat.toFixed(6) + '<br>Lon: ' + lon.toFixed(6) + '<br>Radius: ' + radius + 'm');
            
            marker.on('dragend', function(e) {
                var pos = e.target.getLatLng();
                window.chrome.webview.postMessage('drag:' + marker.wpIndex + ',' + pos.lat + ',' + pos.lng);
            });
            
            waypointMarkers.push(marker);
            
            // Radius circle
            var circle = L.circle([lat, lon], { radius: radius, color: color, fillColor: color, fillOpacity: 0.1, weight: 1, dashArray: '5,5' }).addTo(map);
            waypointCircles.push(circle);
            
            updatePathLine();
        }

        function updateWaypoint(index, lat, lon, type, radius) {
            if (index < waypointMarkers.length) {
                var color = getColor(type);
                waypointMarkers[index].setLatLng([lat, lon]);
                waypointCircles[index].setLatLng([lat, lon]);
                waypointCircles[index].setRadius(radius);
                waypointCircles[index].setStyle({ color: color, fillColor: color });
            }
            updatePathLine();
        }

        function clearWaypoints() {
            waypointMarkers.forEach(m => map.removeLayer(m));
            waypointCircles.forEach(c => map.removeLayer(c));
            distanceLabels.forEach(l => map.removeLayer(l));
            if (waypointLine) map.removeLayer(waypointLine);
            waypointMarkers = []; waypointCircles = []; distanceLabels = []; waypointLine = null;
        }

        function updatePathLine() {
            if (waypointLine) map.removeLayer(waypointLine);
            distanceLabels.forEach(l => map.removeLayer(l));
            distanceLabels = [];

            if (waypointMarkers.length >= 2) {
                var latlngs = waypointMarkers.map(m => m.getLatLng());
                waypointLine = L.polyline(latlngs, { color: '#58A6FF', weight: 3, opacity: 0.8, dashArray: '10,10' }).addTo(map);
                
                // Add distance/bearing labels
                for (var i = 0; i < latlngs.length - 1; i++) {
                    var p1 = latlngs[i], p2 = latlngs[i + 1];
                    var dist = map.distance(p1, p2);
                    var bearing = getBearing(p1.lat, p1.lng, p2.lat, p2.lng);
                    var midLat = (p1.lat + p2.lat) / 2;
                    var midLng = (p1.lng + p2.lng) / 2;
                    
                    var distText = dist > 1000 ? (dist/1000).toFixed(2) + ' km' : dist.toFixed(0) + ' m';
                    var labelHtml = '<div class=""distance-label"">' + distText + ' | ' + bearing.toFixed(0) + '°</div>';
                    var label = L.marker([midLat, midLng], {
                        icon: L.divIcon({ className: '', html: labelHtml, iconSize: [80, 20], iconAnchor: [40, 10] }),
                        interactive: false
                    }).addTo(map);
                    distanceLabels.push(label);
                }
            }
        }

        function getBearing(lat1, lon1, lat2, lon2) {
            var dLon = (lon2 - lon1) * Math.PI / 180;
            var y = Math.sin(dLon) * Math.cos(lat2 * Math.PI / 180);
            var x = Math.cos(lat1 * Math.PI / 180) * Math.sin(lat2 * Math.PI / 180) -
                    Math.sin(lat1 * Math.PI / 180) * Math.cos(lat2 * Math.PI / 180) * Math.cos(dLon);
            return (Math.atan2(y, x) * 180 / Math.PI + 360) % 360;
        }

        var followButton;
        function createFollowButton() {
            followButton = document.createElement('button');
            followButton.innerHTML = '📍 Follow UAV';
            followButton.style.cssText = 'position:absolute;top:10px;left:10px;z-index:1000;padding:10px 15px;background:#3FB950;color:white;border:none;border-radius:6px;cursor:pointer;font-weight:bold;font-size:14px;box-shadow:0 2px 6px rgba(0,0,0,0.3);';
            document.body.appendChild(followButton);
            followButton.onclick = function(e) {
                e.stopPropagation();
                followUAV = !followUAV;
                userMovedMap = !followUAV;
                updateFollowButton();
                if (followUAV && uavMarker) map.setView(uavMarker.getLatLng(), map.getZoom());
            };
        }

        function updateFollowButton() {
            followButton.innerHTML = followUAV ? '📍 Follow UAV' : '❌ Free mode';
            followButton.style.background = followUAV ? '#3FB950' : '#F85149';
        }

        function updateUAV(lat, lon, alt, gs, as) {
            if (!map || !uavMarker) return;
            uavMarker.setLatLng([lat, lon]);
            if (followUAV && !userMovedMap) map.panTo([lat, lon]);
        }

        window.onload = initMap;
    </script>
</body>
</html>";
    }
}