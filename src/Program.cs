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

namespace deletus_tweetus
{
    class Program
    {
        private static ConsoleColor _cachedConsoleColor;
        private static readonly HttpClient client = new HttpClient();
        static string consumerApiKey, consumerApiSecretKey, accessToken, accessTokenSecret;

        static void Main(string[] args)
        {
            // store the current console foreground color so we can reset when done
            _cachedConsoleColor = Console.ForegroundColor;

            // Show the welcome message for the program
            showWelcomeMessage();

            // get the app keys and tokens for twitter app account
            _getConfigKeys();

            // need to wire up the HTTP GET and then the DELETE process here
            var result = _getBearerAuthToken();
            // cLog("StatusCode: " + result.StatusCode);
            // cLog("Headers: " + result.Headers);
            // cLog("Content: " + result.Content);
            // cLog("ReasonPhrase: " + result.ReasonPhrase);
            // cLog("RequestMessage: " + result.RequestMessage);
            // cLog("Content.Headers: " + result.Content.Headers);

            // Reset the console color to what it was in beginning
            Console.ForegroundColor = _cachedConsoleColor;
        }


        /// <summary>
        /// Will check for Twitter Keys and Secrets in appsettings.json file, if not found, user is prompted for input in the terminal.
        /// </summary>
        private static void _getConfigKeys()
        {
            logBlue("Reading Configuration values from appsettings.json ...");

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
                logRed("All of the Twitter Api Keys and Secrets are required to communicate with the Twitter API. \nDeletus-Tweetus is now exiting.");
                Environment.Exit(0);
                return;
            }

        }

        private static string _getBearerAuthToken()
        {
            string encodedCreds = _encodeCredentials();

            // FormUrlEncodedContent stringContent = new FormUrlEncodedContent(new[]
            //     {
            //         new KeyValuePair<string, string>("User-Agent", "Deletus-Tweetus"),
            //         new KeyValuePair<string, string>("Content-Type", "application/x-www-form-urlencoded;charset=UTF-8"),
            //         new KeyValuePair<string, string>("Authorization", $"Basic {plainText}")
            //     });
            // Dictionary<string, string> postParams = new Dictionary<string, string> {
            //     { "grant_type", "client_credentials" }
            // };

            HttpRequestMessage req = new HttpRequestMessage(HttpMethod.Post, TwitterEndpoints.OA_TOKEN);

            req.Headers.Clear();
            req.Headers.ExpectContinue = false;
            req.Headers.Add("User-Agent", "Deletus-Tweetus");
            // req.Headers.Add("Content-Type", "application/x-www-form-urlencoded;charset=UTF-8");
            req.Headers.Add("Authorization", $"Basic {encodedCreds}");
            req.Content = new StringContent("grant_type=client_credentials", Encoding.UTF8, "application/x-www-form-urlencoded");

            cLog("request: " + req);
            cLog(req.Content.ToString());

            HttpClientHandler handler = new HttpClientHandler();
            if (handler.SupportsAutomaticDecompression)
                handler.AutomaticDecompression = DecompressionMethods.GZip;

            string response = "";
            using (HttpClient client = new HttpClient(handler))
            {
                HttpContent content = client.SendAsync(req).Result.Content;

                response = content.ReadAsStringAsync().Result;

                cLog("response: " + response);
            }

            return response;
        }

        private static string _encodeCredentials()
        {
            string encodedConsumerKey = Uri.EscapeDataString(consumerApiKey);
            string encodedConsumerSecret = Uri.EscapeDataString(consumerApiSecretKey);

            string concatenatedCredentials = encodedConsumerKey + ":" + encodedConsumerSecret;

            byte[] credBytes = Encoding.UTF8.GetBytes(concatenatedCredentials);

            string base64Credentials = Convert.ToBase64String(credBytes);
            cLog("base64Credentials: " + base64Credentials);
            return base64Credentials;
        }


    }
}

