using System;
using System.IO;
using System.Net.Http;
using LinqToTwitter;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Configuration.FileExtensions;
using Microsoft.Extensions.Configuration.Json;
using System.Threading.Tasks;
using System.Net.Http.Headers;

namespace deletus_tweetus
{
    class Program
    {
        private static readonly HttpClient client = new HttpClient();
        private string TwitterApiBaseUrl = TwitterEndpoints.REST_ROOT_URL;
        static string consumerApiKey, consumerApiSecretKey, accessToken, accessTokenSecret;

        static void Main(string[] args)
        {
            DateTime start = DateTime.Now;

            // get the app keys and tokens for twitter app account
            promptUserForConfigKeys();

            Console.WriteLine(consumerApiKey);

            ProcessRepositories();
            // Console.WriteLine(t);
        }

        private static void promptUserForConfigKeys()
        {
            Console.WriteLine("\nWhat is your Consumer API Key?");
            consumerApiKey = Console.ReadLine();

            Console.WriteLine("\nWhat is your Consumer API Secret Key?");
            consumerApiSecretKey = Console.ReadLine();

            Console.WriteLine("\nWhat is your Access Token?");
            accessToken = Console.ReadLine();

            Console.WriteLine("\nWhat is your Access Token Secret?");
            accessTokenSecret = Console.ReadLine();
        }


        // private static async string fetchUserTimeline()
        // {
        //     Console.WriteLine("Fetching timeline for user...");
        //     string queryUrl = $"{TwitterApiBaseUrl}statuses/home_timeline.json?count=200?since_id=";
        //     Console.WriteLine($"HTTP GET = {queryUrl}");
        //     // Add a new Request Message
        //     HttpRequestMessage requestMessage = new HttpRequestMessage(HttpMethod.Get, queryUrl);

        //     requestMessage.Headers.Add("Accept", "application/vnd.github.v3+json");
        //     requestMessage.Headers.Add("User-Agent", "HttpClientFactory-Sample");


        //     // Send the request to the server
        //     HttpResponseMessage response = await httpClient.SendAsync(requestMessage);

        //     // Get the response
        //     var responseString = await response.Content.ReadAsStringAsync();
        //     // var data = new object[0];

        //     return responseString;
        // }

        private static async void ProcessRepositories()
        {
            client.DefaultRequestHeaders.Accept.Clear();
            client.DefaultRequestHeaders.Accept.Add(
                new MediaTypeWithQualityHeaderValue("application/vnd.github.v3+json"));
            client.DefaultRequestHeaders.Add("User-Agent", ".NET Foundation Repository Reporter");

            var stringTask = client.GetStringAsync("https://api.github.com/orgs/dotnet/repos");

            var msg = await stringTask;
            Console.Write(msg);
        }

    }
}

