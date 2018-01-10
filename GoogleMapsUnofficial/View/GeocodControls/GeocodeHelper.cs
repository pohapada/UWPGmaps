﻿using System;
using System.Collections.Generic;
using System.Linq;
using Windows.Web.Http;
using System.Text;
using System.Threading.Tasks;
using Newtonsoft.Json;

namespace GoogleMapsUnofficial.View.GeocodControls
{
    public class GeocodeHelper
    {
        public static async Task<string> GetAddress()
        {
            try
            {
                var cn = MapView.MapControl.Center;
                var http = new HttpClient();
                http.DefaultRequestHeaders.UserAgent.ParseAdd(AppCore.GoogleMapAPIKey);
                var r = await http.GetStringAsync(new Uri($"https://maps.googleapis.com/maps/api/geocode/json?latlng={cn.Position.Latitude},{cn.Position.Longitude}&sensor=false&key={AppCore.GoogleMapAPIKey}", UriKind.RelativeOrAbsolute));
                var res = JsonConvert.DeserializeObject<Rootobject>(r);
                return res.results.FirstOrDefault().formatted_address;
            }
            catch { return "Earth :D"; }
        }


        public class Rootobject
        {
            public Result[] results { get; set; }
            public string status { get; set; }
        }

        public class Result
        {
            public Address_Components[] address_components { get; set; }
            public string formatted_address { get; set; }
            public Geometry geometry { get; set; }
            public string place_id { get; set; }
            public string[] types { get; set; }
        }

        public class Geometry
        {
            public Location location { get; set; }
            public string location_type { get; set; }
            public Viewport viewport { get; set; }
            public Bounds bounds { get; set; }
        }

        public class Location
        {
            public float lat { get; set; }
            public float lng { get; set; }
        }

        public class Viewport
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

        public class Bounds
        {
            public Northeast1 northeast { get; set; }
            public Southwest1 southwest { get; set; }
        }

        public class Northeast1
        {
            public float lat { get; set; }
            public float lng { get; set; }
        }

        public class Southwest1
        {
            public float lat { get; set; }
            public float lng { get; set; }
        }

        public class Address_Components
        {
            public string long_name { get; set; }
            public string short_name { get; set; }
            public string[] types { get; set; }
        }

    }
}
