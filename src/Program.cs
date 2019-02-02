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
        private static HttpClient client;
        private static Stopwatch sw = new Stopwatch();
        private static string consumerApiKey, consumerApiSecretKey, accessToken, accessTokenSecret, TwitterApiBearerToken, fileReadTime, fileWriteTime;
        public static List<ulong> _tweetIdArray = new List<ulong>();
        private static HttpClientHandler handler = new HttpClientHandler();

        static void Main(string[] args)
        {
            _setupMainProgram();

            sw.Start();
            // store the current console foreground color so we can reset when done
            _cachedConsoleColor = Console.ForegroundColor;
            // Show the welcome message for the program
            showWelcomeMessage();
            // get the app keys and tokens for twitter app account
            _getConfigKeys();
            // need to wire up the HTTP GET and then the DELETE process here
            _getBearerAuthToken();
            // get the twitter timeline
            _getTimeline();

            // delete tweets
            if (_tweetIdArray.Count >= 1)
            {
                _tweetIdArray.ForEach(tweetId =>
                {
                    _deleteTweet(tweetId);
                });
            }

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

            string response = "";

            HttpContent content = client.SendAsync(req).Result.Content;
            response = content.ReadAsStringAsync().Result;

            //  JSON string into an object so we can get the bearer_token
            TwitterAuthResponse x = JsonConvert.DeserializeObject<TwitterAuthResponse>(response);
            TwitterApiBearerToken = x.access_token;
            cLog("TwitterApiBearerToken =  " + TwitterApiBearerToken + "\n");
        }

        private static void _getTimeline()
        {
            client.DefaultRequestHeaders.Add("User-Agent", "Deletus-Tweetus");
            client.DefaultRequestHeaders.Add("Authorization", "Bearer " + TwitterApiBearerToken);
            HttpContent x = client.GetAsync("https://api.twitter.com/1.1/statuses/user_timeline.json?screen_name=_bradmartin_&count=3000").Result.Content;
            string timelineData = x.ReadAsStringAsync().Result;

            // parse the JSON data from twitter API and put the tweet ids into a List
            JArray tweets = JArray.Parse(timelineData);
            foreach (JObject t in tweets.Children())
            {
                foreach (JProperty prop in t.Properties())
                {
                    if (prop.Name == "id")
                    {
                        cLog($"Tweet ID: {prop.Value}");
                        _tweetIdArray.Add(prop.Value.ToObject<ulong>());
                    }
                }
            }

            // write the json from the response to a file for now
            long startW = sw.ElapsedMilliseconds;
            File.WriteAllText(@"timeline.json", timelineData);
            long endW = sw.ElapsedMilliseconds;
            fileWriteTime = (endW - startW).ToString();

            long startR = sw.ElapsedMilliseconds;
            string timelineJson = File.ReadAllText(@"timeline.json");
            long endR = sw.ElapsedMilliseconds;
            fileReadTime = (endR - startR).ToString();
        }

        private static string _deleteTweet(ulong id)
        {
            // POST https://api.twitter.com/1.1/statuses/destroy/240854986559455234.json

            string encodedCreds = _encodeCredentials();

            HttpRequestMessage req = new HttpRequestMessage(HttpMethod.Post, TwitterEndpoints.REST_ROOT_URL + $"statuses/destroy/{id}");

            req.Headers.Clear();
            req.Headers.ExpectContinue = false;
            req.Headers.Add("User-Agent", "Deletus-Tweetus");
            req.Headers.Add("Authorization", $"Basic {encodedCreds}");
            req.Content = new StringContent("grant_type=client_credentials", Encoding.UTF8, "application/x-www-form-urlencoded");

            string response = "";
            HttpContent content = client.SendAsync(req).Result.Content;
            response = content.ReadAsStringAsync().Result;

            return response;
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


        /// <summary>
        /// Set up for the program.
        /// Configures the HttpClient handler and compression if possible.
        /// </summary>
        private static void _setupMainProgram()
        {
            if (handler.SupportsAutomaticDecompression)
            {
                handler.AutomaticDecompression = DecompressionMethods.GZip;
                client = new HttpClient(handler);
            }
        }

    }
}

