using System;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Linq;
using System.Web;
using static Utils;
using static Silly;
using System.Collections.Generic;
using System.Net;
using System.Text;
using System.Threading.Tasks;
using System.IO;
using LitJson;
using Microsoft.Extensions.Configuration;
using Newtonsoft.Json;
using System.Threading;
using Newtonsoft.Json.Linq;
using System.Diagnostics;

namespace deletus_tweetus
{
    class Program
    {
        private static ConsoleColor _cachedConsoleColor;
        private static readonly HttpClient client = new HttpClient();
        private static Stopwatch sw = new Stopwatch();
        private static string consumerApiKey, consumerApiSecretKey, accessToken, accessTokenSecret, TwitterApiBearerToken, fileReadTime, fileWriteTime;

        static void Main(string[] args)
        {
            sw.Start();

            // store the current console foreground color so we can reset when done
            _cachedConsoleColor = Console.ForegroundColor;

            // Show the welcome message for the program
            showWelcomeMessage();

            // get the app keys and tokens for twitter app account
            _getConfigKeys();

            // need to wire up the HTTP GET and then the DELETE process here
            _getBearerAuthToken();
            cLog("TwitterApiBearerToken =  " + TwitterApiBearerToken);

            _getTimeline();

            sw.Stop();
            // show exit log print
            printExitMessage();

            // Reset the console color to what it was in beginning
            Console.ForegroundColor = _cachedConsoleColor;
        }



        /// <summary>
        /// Will check for Twitter Keys and Secrets in appsettings.json file, if not found, user is prompted for input in the terminal.
        /// </summary>
        private static void _getConfigKeys()
        {
            logBlue("\nReading Configuration values from appsettings.json ...");

            // see if the user has input the values into appsettings.json and use those as defaults
            consumerApiKey = ConfigValueProvider.Get("ConsumerApiKey");
            consumerApiSecretKey = ConfigValueProvider.Get("ConsumerSecretKey");
            accessToken = ConfigValueProvider.Get("AccessToken");
            accessTokenSecret = ConfigValueProvider.Get("AccessTokenSecret");

            if (String.IsNullOrEmpty(consumerApiKey))
            {
                logBlue("\nWhat is your Consumer API Key?");
                consumerApiKey = Console.ReadLine();
            }

            if (String.IsNullOrEmpty(consumerApiSecretKey))
            {
                logBlue("\nWhat is your Consumer API Secret Key?");
                consumerApiSecretKey = Console.ReadLine();
            }


            if (String.IsNullOrEmpty(accessToken))
            {
                logBlue("\nWhat is your Access Token?");
                accessToken = Console.ReadLine();
            }

            if (String.IsNullOrEmpty(accessTokenSecret))
            {
                logBlue("\nWhat is your Access Token Secret?");
                accessTokenSecret = Console.ReadLine();
            }

            cLog("\nThe values for your Twitter configuration are:");
            cLog("Consumer Api Key = " + consumerApiKey);
            cLog("Consumer Api Secret Key = " + consumerApiSecretKey);
            cLog("Access Token = " + accessToken);
            cLog("Access Token Secret = " + accessTokenSecret);
            cLog("\n");

            // if any value is null, print message and exit since they are required to use the Twitter API.
            if (
                String.IsNullOrEmpty(consumerApiKey) ||
                String.IsNullOrEmpty(consumerApiSecretKey) ||
                String.IsNullOrEmpty(accessToken) ||
                String.IsNullOrEmpty(accessTokenSecret)
                )
            {
                logRed("\nAll of the Twitter Api Keys and Secrets are required to communicate with the Twitter API. \nDeletus-Tweetus is now exiting.");
                Environment.Exit(0);
                return;
            }

        }

        private static void _getBearerAuthToken()
        {
            string encodedCreds = _encodeCredentials();

            HttpRequestMessage req = new HttpRequestMessage(HttpMethod.Post, TwitterEndpoints.OA_TOKEN);

            req.Headers.Clear();
            req.Headers.ExpectContinue = false;
            req.Headers.Add("User-Agent", "Deletus-Tweetus");
            req.Headers.Add("Authorization", $"Basic {encodedCreds}");
            req.Content = new StringContent("grant_type=client_credentials", Encoding.UTF8, "application/x-www-form-urlencoded");

            // cLog("request: " + req);
            // cLog(req.Content.ToString());

            HttpClientHandler handler = new HttpClientHandler();
            if (handler.SupportsAutomaticDecompression)
                handler.AutomaticDecompression = DecompressionMethods.GZip;

            string response = "";
            using (HttpClient client = new HttpClient(handler))
            {
                HttpContent content = client.SendAsync(req).Result.Content;
                response = content.ReadAsStringAsync().Result;

                //  JSON string into an object so we can get the bearer_token
                TwitterAuthResponse x = JsonConvert.DeserializeObject<TwitterAuthResponse>(response);
                TwitterApiBearerToken = x.access_token;
            }
        }

        private static void _getTimeline()
        {
            client.DefaultRequestHeaders.Add("User-Agent", "Deletus-Tweetus");
            client.DefaultRequestHeaders.Add("Authorization", "Bearer " + TwitterApiBearerToken);

            HttpContent x = client.GetAsync("https://api.twitter.com/1.1/statuses/user_timeline.json?screen_name=realDonaldTrump&count=3000").Result.Content;
            string timelineData = x.ReadAsStringAsync().Result;

            // write the json from the response to a file for now
            long startW = sw.ElapsedMilliseconds;
            File.WriteAllText(@"timeline.json", timelineData);
            long endW = sw.ElapsedMilliseconds;
            fileWriteTime = (endW - startW).ToString();

            long startR = sw.ElapsedMilliseconds;
            string timelineJson = File.ReadAllText(@"timeline.json");
            long endR = sw.ElapsedMilliseconds;
            fileReadTime = (endR - startR).ToString();


            // List<int> idList = new List<int>();
            // JObject z;

            // // TODO: read the entire array returned from the timeline.json
            // // then put all the ids of the tweets into an array
            // // loop the array and send DELETE requests for those tweet ids    
            // using (StreamReader sr = File.OpenText(@"timeline.json"))
            // {
            //     string s = String.Empty;
            //     while ((s = sr.ReadLine()) != null)
            //     {
            //         //we're just testing read speeds // 1243ms 1157ms 1131ms 1162ms 1232ms (times)
            //         // JObject o = JObject.Parse(s);
            //         cLog("\no: " + s);
            //     }
            // }
        }

        private static string _encodeCredentials()
        {
            // encode the key and secret
            string encodedConsumerKey = Uri.EscapeDataString(consumerApiKey);
            string encodedConsumerSecret = Uri.EscapeDataString(consumerApiSecretKey);
            // concat the key and secret
            string concatenatedCredentials = encodedConsumerKey + ":" + encodedConsumerSecret;
            // encoding the concatenated string
            byte[] credBytes = Encoding.UTF8.GetBytes(concatenatedCredentials);
            // converting to base64
            string base64Credentials = Convert.ToBase64String(credBytes);
            return base64Credentials;
        }


        private static void printExitMessage()
        {
            // print file write and read times
            logBlue($"\nFile Write Time: {fileWriteTime} \nFile Read Time: {fileReadTime}");
            // print elapsed time
            logBlue("\nProgram took " + sw.ElapsedMilliseconds.ToString() + " milliseconds.");
        }

    }
}

