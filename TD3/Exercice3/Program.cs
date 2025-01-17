﻿using System;
using System.Text.Json;

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

    internal class Program
    {
        static readonly HttpClient client = new HttpClient();

        static async Task Main(string[] args)
        {
            string api_key = "e0beee5c2f1a0468494223125c4c35b1761aaffc";
            HttpResponseMessage response = await client.GetAsync($"https://api.jcdecaux.com/vls/v1/stations/{args[1]}?contract={args[0]}&apiKey={api_key}");
            response.EnsureSuccessStatusCode();
            string responseBody = await response.Content.ReadAsStringAsync();

            Root jsonData = JsonSerializer.Deserialize<Root>(responseBody);

            Console.WriteLine(jsonData.number + "\n" +
                jsonData.number + "\n" +
                jsonData.contract_name + "\n" +
                jsonData.name + "\n" +
                jsonData.address + "\n" +
                jsonData.position + "\n" +
                jsonData.banking + "\n" +
                jsonData.bonus + "\n" +
                jsonData.bike_stands + "\n" +
                jsonData.available_bike_stands + "\n" +
                jsonData.available_bikes + "\n" +
                jsonData.status + "\n" +
                jsonData.last_update + "\n");
        }
    }
}