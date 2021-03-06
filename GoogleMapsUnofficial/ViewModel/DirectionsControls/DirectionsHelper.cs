﻿using GoogleMapsUnofficial.ViewModel.VoiceNavigation;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Windows.Devices.Geolocation;
using Windows.UI;
using Windows.UI.Xaml.Controls.Maps;
using Windows.Web.Http;

namespace GoogleMapsUnofficial.ViewModel.DirectionsControls
{
    public class DirectionsHelper
    {
        /// <summary>
        /// Available modes for Direction
        /// </summary>
        public enum DirectionModes
        {
            driving, walking, bicycling, transit
        }
        /// <summary>
        /// Get Directions from a Origin to a Destination
        /// </summary>
        /// <param name="Origin">The Origin BasicGeoposition</param>
        /// <param name="Destination">The Destination BasicGeoposition</param>
        /// <param name="Mode">Mode for example Driving, walking or etc.</param>
        /// <returns></returns>
        public static async Task<Rootobject> GetDirections(BasicGeoposition Origin, BasicGeoposition Destination, DirectionModes Mode = DirectionModes.driving)
        {
            try
            {
                var m = Mode.ToString();
                var requestUrl = String.Format("http://maps.google.com/maps/api/directions/json?origin=" + Origin.Latitude + "," + Origin.Longitude + "&destination=" + Destination.Latitude + "," + Destination.Longitude + "&units=metric&mode=" + Mode);

                var http = new HttpClient();
                http.DefaultRequestHeaders.UserAgent.ParseAdd(AppCore.HttpUserAgent);
                var s = await http.GetStringAsync(new Uri(requestUrl, UriKind.RelativeOrAbsolute));
                return JsonConvert.DeserializeObject<Rootobject>(s);
            }
            catch (Exception)
            {
                return null;
            }
        }
        /// <summary>
        /// Create a MapPolyLine from RootObject and select FirstOrDefault route to show on map control
        /// </summary>
        /// <param name="FuncResp">Convert first or default route to mappolyline to show on map</param>
        /// <returns></returns>
        public static MapPolyline GetDirectionAsRoute(Rootobject FuncResp, Color ResultColor)
        {
            var loclist = new List<BasicGeoposition>();
            foreach (var leg in FuncResp.routes.FirstOrDefault().legs)
            {
                foreach (var step in leg.steps)
                {
                    loclist.Add(
                        new BasicGeoposition()
                        { Latitude = step.start_location.lat, Longitude = step.start_location.lng });
                    loclist.Add(
                        new BasicGeoposition()
                        { Latitude = step.end_location.lat, Longitude = step.end_location.lng });
                }
            }
            MapPolyline line = new MapPolyline()
            {
                StrokeThickness = 5,
                StrokeColor = Colors.Purple,
                Path = new Geopath(loclist)
            };
            var voice = new VoiceHelper(FuncResp.routes.FirstOrDefault());
            return line;
        }
        /// <summary>
        /// Create a MapPolyLine from Route to show on map control
        /// </summary>
        /// <param name="Route">Selected route for converting to mappolyline</param>
        /// <returns></returns>
        public static MapPolyline GetDirectionAsRoute(Route Route, Color ResultColor)
        {
            var loclist = new List<BasicGeoposition>();
            foreach (var leg in Route.legs)
            {
                foreach (var step in leg.steps)
                {
                    loclist.Add(
                        new BasicGeoposition()
                        { Latitude = step.start_location.lat, Longitude = step.start_location.lng });
                    loclist.Add(
                        new BasicGeoposition()
                        { Latitude = step.end_location.lat, Longitude = step.end_location.lng });
                }
            }
            MapPolyline line = new MapPolyline()
            {
                StrokeThickness = 5,
                StrokeColor = ResultColor,
                Path = new Geopath(loclist)
            };
            var voice = new VoiceHelper(Route);
            return line;
        }

        public static string GetDistance(Route Route)
        {
            var Distance = 0;
            foreach (var item in Route.legs)
            {
                Distance += item.distance.value;
            }
            return $"{Distance} meters";
        }

        public static string GetTotalEstimatedTime(Route Route)
        {
            var EstimatedTime = 0;
            foreach (var item in Route.legs)
            {
                EstimatedTime += item.duration.value;
            }
            return $"{Convert.ToInt32((EstimatedTime / 60))} minutes";
        }

        //public static async Task<List<GoogleMapPlaceAutoCompleteModel.Prediction>> GetAutoCompleteResults(string Input, string Lat, string Lng)
        //{
        //    var http = new HttpClient();
        //    var r = await http.GetStringAsync(new Uri($"https://maps.googleapis.com/maps/api/place/autocomplete/json?input={Input.Replace(" ", "+")}&location={Lat},{Lng}&radius=50000&key=AIzaSyCS5gpejHZIpgK7StAfFCcTqZ8cQsuHVnw"));
        //    var res = JsonConvert.DeserializeObject<GoogleMapPlaceAutoCompleteModel.Rootobject>(r);
        //    return res.predictions.ToList();
        //}

        public class Rootobject
        {
            public Geocoded_Waypoints[] geocoded_waypoints { get; set; }
            public Route[] routes { get; set; }
            public string status { get; set; }
        }

        public class Geocoded_Waypoints
        {
            public string geocoder_status { get; set; }
            public string place_id { get; set; }
            public string[] types { get; set; }
        }

        public class Route
        {
            public Bounds bounds { get; set; }
            public string copyrights { get; set; }
            public Leg[] legs { get; set; }
            public Overview_Polyline overview_polyline { get; set; }
            public string summary { get; set; }
            public string[] warnings { get; set; }
            public object[] waypoint_order { get; set; }
        }

        public class Bounds
        {
            public Northeast northeast { get; set; }
            public Southwest southwest { get; set; }
        }

        public class Northeast
        {
            public float lat { get; set; }
            public float lng { get; set; }
        }

        public class Southwest
        {
            public float lat { get; set; }
            public float lng { get; set; }
        }

        public class Overview_Polyline
        {
            public string points { get; set; }
        }

        public class Leg
        {
            public Distance distance { get; set; }
            public Duration duration { get; set; }
            public string end_address { get; set; }
            public End_Location end_location { get; set; }
            public string start_address { get; set; }
            public Start_Location start_location { get; set; }
            public Step[] steps { get; set; }
            public object[] traffic_speed_entry { get; set; }
            public object[] via_waypoint { get; set; }
        }

        public class Distance
        {
            public string text { get; set; }
            public int value { get; set; }
        }

        public class Duration
        {
            public string text { get; set; }
            public int value { get; set; }
        }

        public class End_Location
        {
            public float lat { get; set; }
            public float lng { get; set; }
        }

        public class Start_Location
        {
            public float lat { get; set; }
            public float lng { get; set; }
        }

        public class Step
        {
            public Distance1 distance { get; set; }
            public Duration1 duration { get; set; }
            public End_Location1 end_location { get; set; }
            public string html_instructions { get; set; }
            public Polyline polyline { get; set; }
            public Start_Location1 start_location { get; set; }
            public string travel_mode { get; set; }
            public string maneuver { get; set; }
        }

        public class Distance1
        {
            public string text { get; set; }
            public int value { get; set; }
        }

        public class Duration1
        {
            public string text { get; set; }
            public int value { get; set; }
        }

        public class End_Location1
        {
            public float lat { get; set; }
            public float lng { get; set; }
        }

        public class Polyline
        {
            public string points { get; set; }
        }

        public class Start_Location1
        {
            public float lat { get; set; }
            public float lng { get; set; }
        }

    }
}
