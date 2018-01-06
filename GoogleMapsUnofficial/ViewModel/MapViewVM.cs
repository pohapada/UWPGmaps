﻿using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Windows.Devices.Geolocation;
using Windows.Foundation;
using Windows.UI.Core;
using Windows.UI.Xaml.Controls.Maps;

namespace GoogleMapsUnofficial.ViewModel
{
    public class ViewModel : INotifyPropertyChanged
    {
        private string attractionname;
        private Geopoint location;
        public Geopoint Location
        {
            get { return location; }
            set
            {
                location = value;
                PropertyChanged?.Invoke(this, new PropertyChangedEventArgs("Location"));
            }
        }
        public static Geopoint UserLocation { get; set; }
        public string AttractionName
        {
            get { return attractionname; }
            set
            {
                attractionname = value;
                PropertyChanged?.Invoke(this, new PropertyChangedEventArgs("AttractionName"));
            }
        }
        public event PropertyChangedEventHandler PropertyChanged;
    }
    class MapViewVM : INotifyPropertyChanged
    {
        public event PropertyChangedEventHandler PropertyChanged;
        private MapControl Map;
        private CoreWindow CoreWindow;
        public ViewModel UserLocation { get; set; }
        Geolocator geolocator = new Geolocator();
        public MapViewVM()
        {
            CoreWindow = CoreWindow.GetForCurrentThread();
            LoadPage();
            UserLocation = new ViewModel() { AttractionName = "My Location" };
        }
        ~MapViewVM()
        {
            geolocator.PositionChanged -= Geolocator_PositionChanged;
            geolocator = null;
        }
        async void LoadPage()
        {
            await CoreWindow.Dispatcher.RunAsync(CoreDispatcherPriority.Normal, async delegate
            {
                var accessStatus = await Geolocator.RequestAccessAsync();
                if (accessStatus == GeolocationAccessStatus.Allowed)
                {
                    if(ClassInfo.DeviceType() != ClassInfo.DeviceTypeEnum.Phone)
                    {
                        geolocator.DesiredAccuracy = PositionAccuracy.High;
                    }
                    // Subscribe to the StatusChanged event to get updates of location status changes.
                    //geolocator.StatusChanged += Geolocator_StatusChanged;
                    geolocator.PositionChanged += Geolocator_PositionChanged;
                    // Carry out the operation.
                    Geoposition pos = await geolocator.GetGeopositionAsync();

                    var MyLandmarks = new List<MapElement>();

                    BasicGeoposition snPosition = new BasicGeoposition { Latitude = pos.Coordinate.Point.Position.Latitude, Longitude = pos.Coordinate.Point.Position.Longitude };
                    Geopoint snPoint = new Geopoint(snPosition);
                    UserLocation.Location = snPoint;
                    //UserLoction = new MapIcon
                    //{
                    //    CollisionBehaviorDesired = MapElementCollisionBehavior.RemainVisible,
                    //    Location = snPoint,
                    //    NormalizedAnchorPoint = new Point(0.5, 1.0),
                    //    ZIndex = 0,
                    //    Title = "Your Location"
                    //};
                    //MyLandmarks.Add(UserLoction);
                    await Task.Delay(10);
                    Map = View.MapView.MapControl;
                    //Map.MapElements.Add(UserLoction);
                    Map.Center = snPoint;
                    Map.ZoomLevel = 16;

                }
            });
        }

        private async void Geolocator_PositionChanged(Geolocator sender, PositionChangedEventArgs args)
        {
            try
            {
                await CoreWindow.Dispatcher.RunAsync(CoreDispatcherPriority.Normal, delegate
                {
                    if (Map == null || UserLocation == null) return;
                    UserLocation.Location = new Geopoint(args.Position.Coordinate.Point.Position);
                });
            }
            catch { }
        }
    }
}