using System;
using System.Text.Json;
using System.Device.Location;

namespace MyApp // Note: actual namespace depends on the project name.
{
    public class Position
    {
        public double lat { get; set; }
        public double lng { get; set; }
    }

    public class Root
    {
        public int number { get; set; }
        public string contract_name { get; set; }
        public string name { get; set; }
        public string address { get; set; }
        public Position position { get; set; }
        public bool banking { get; set; }
        public bool bonus { get; set; }
        public int bike_stands { get; set; }
        public int available_bike_stands { get; set; }
        public int available_bikes { get; set; }
        public string status { get; set; }
        public object last_update { get; set; }
    }

    public class Error
    {
        public string error { get; set; }
    }



    internal class Program
    {
        static readonly HttpClient client = new HttpClient();

        static async Task Main(string[] args)
        {
            GeoCoordinate userPos;

            try
            {
                userPos = new GeoCoordinate(Double.Parse(args[1]), Double.Parse(args[2]));
                string api_key = "e0beee5c2f1a0468494223125c4c35b1761aaffc";
                HttpResponseMessage response = await client.GetAsync($"https://api.jcdecaux.com/vls/v1/stations?contract={args[0]}&apiKey={api_key}");

                try
                {
                    response.EnsureSuccessStatusCode();
                    string responseBody = await response.Content.ReadAsStringAsync();

                    List<Root> jsonData = JsonSerializer.Deserialize<List<Root>>(responseBody);

                    Root bestDistanceElement = null;
                    double bestDistance = -1;

                    foreach (Root element in jsonData)
                    {
                        GeoCoordinate element_coords = new GeoCoordinate(element.position.lat, element.position.lng);
                        if (bestDistance < 0 || element_coords.GetDistanceTo(userPos) < bestDistance)
                        {
                            bestDistance = element_coords.GetDistanceTo(userPos);
                            bestDistanceElement = element;
                        }
                    }

                    Console.WriteLine($"The closest station is {bestDistanceElement.name} at {Math.Round(bestDistance)}m (GPS coordinates : ({bestDistanceElement.position.lat}, {bestDistanceElement.position.lng})).");
                }
                catch (HttpRequestException ex)
                {
                    string responseBody = await response.Content.ReadAsStringAsync();
                    Error errorData = JsonSerializer.Deserialize<Error>(responseBody);

                    Console.WriteLine($"Error : {errorData.error}");
                }
            }
            catch (FormatException ex)
            {
                Console.WriteLine("Error when parsing an argument to a double : please make sure the format is correct.");
            }
        }
    }
}