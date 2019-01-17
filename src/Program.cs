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
            string encodedKey = Uri.EscapeDataString(consumerApiKey);
            string encodedSecret = Uri.EscapeDataString(consumerApiSecretKey);
            string concatenatedCredentials = encodedKey + ":" + encodedSecret;
            byte[] credBytes = Encoding.UTF8.GetBytes(concatenatedCredentials);
            string base64Credentials = Convert.ToBase64String(credBytes);
            Console.WriteLine("base64Credentials : {0}", base64Credentials);

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
            req.Headers.Add("Authorization", $"Basic {concatenatedCredentials}");
            req.Content = new StringContent("grant_type=client_credentials", Encoding.UTF8, "application/x-www-form-urlencoded");

            cLog("request: " + req);
            cLog(req.Content.ToString());

            var handler = new HttpClientHandler();
            if (handler.SupportsAutomaticDecompression)
                handler.AutomaticDecompression = DecompressionMethods.GZip;

            string response = "";
            using (var client = new HttpClient(handler))
            {
                var content = client.SendAsync(req).Result.Content;

                response = content.ReadAsStringAsync().Result;

                cLog("response: " + response);
            }

            // HttpResponseMessage response = client.SendAsync(req).Result;
            // // HttpResponseMessage response = client.PostAsync(TwitterEndpoints.OA_TOKEN, stringContent).Result;

            // var receiveStream = response.Content.ReadAsStringAsync().Result;
            // StreamReader readStream = new StreamReader(receiveStream, Encoding.UTF8);
            // var sText = readStream.ReadToEnd();
            // cLog("sText: " + sText);

            // var sText = await response.Content.ReadAsStringAsync().ConfigureAwait(false);
            // cLog("sText: " + sText);

            return response;
        }

        internal string EncodeCredentials()
        {
            string encodedConsumerKey = Uri.EscapeDataString("hWc3Cqc4QOAAjOzkTZ9z14yI4");
            string encodedConsumerSecret = Uri.EscapeDataString("2gSqFAPSZqfslUvgBbDGUz5VTlkd3JX9lzEy68sV85AzECjIDN");

            string concatenatedCredentials = encodedConsumerKey + ":" + encodedConsumerSecret;

            byte[] credBytes = Encoding.UTF8.GetBytes(concatenatedCredentials);

            string base64Credentials = Convert.ToBase64String(credBytes);
            cLog("base64Credentials: " + base64Credentials);
            return base64Credentials;
        }

        // private static string ProcessRepositories()
        // {
        //     client.DefaultRequestHeaders.Accept.Clear();
        //     client.DefaultRequestHeaders.Accept.Add(
        //         new MediaTypeWithQualityHeaderValue("application/vnd.github.v3+json"));
        //     client.DefaultRequestHeaders.Add("User-Agent", ".NET Foundation Repository Reporter");
        //     client.DefaultRequestHeaders.Add("Authorization", "OAuth oauth_consumer_key=" + consumerApiKey + ", oauth_nonce=" + RandomString(32) + ", oauth_signature=" + HttpUtility.UrlEncode(consumerApiSecretKey) + "&" + HttpUtility.UrlEncode(accessTokenSecret) + ", oauth_signature_method=HMAC-SHA1" + ", oauth_timestamp=" + DateTimeOffset.Now.ToUnixTimeSeconds() + ", oauth_token=" + accessToken + ", oauth_version=1.0"
        //     );

        //     // var stringTask = client.GetStringAsync("https://api.github.com/orgs/dotnet/repos");
        //     string stringTask = client.GetStringAsync(TwitterEndpoints.REST_ROOT_URL + "statuses/user_timeline.json?screen_name=__bradmartin__&count=2").Result;

        //     var msg = stringTask;
        //     return msg;
        // }

    }
}

