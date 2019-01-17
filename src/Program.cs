using System;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Linq;
using System.Web;
using static Utils;

namespace deletus_tweetus
{
    class Program
    {
        private static readonly HttpClient client = new HttpClient();
        private static ConsoleColor _cachedConsoleColor;

        static string consumerApiKey, consumerApiSecretKey, accessToken, accessTokenSecret;

        static void Main(string[] args)
        {
            // store the current console foreground color so we can reset when done
            _cachedConsoleColor = Console.ForegroundColor;

            // Show the welcome message for the program
            _showWelcomeMessage();

            // get the app keys and tokens for twitter app account
            _getConfigKeys();

            // need to wire up the HTTP GET and then the DELETE process here
            cLog(consumerApiKey);

            string x = _getBearerAuthToken();
            cLog("x: " + x);

            string msg = ProcessRepositories();
            cLog(msg);

            // Reset the console color to what it was in beginning
            Console.ForegroundColor = _cachedConsoleColor;
        }

        private static void _getConfigKeys()
        {
            logBlue("\nWhat is your Consumer API Key?");
            consumerApiKey = Console.ReadLine();

            logBlue("\nWhat is your Consumer API Secret Key?");
            consumerApiSecretKey = Console.ReadLine();

            logBlue("\nWhat is your Access Token?");
            accessToken = Console.ReadLine();

            logBlue("\nWhat is your Access Token Secret?");
            accessTokenSecret = Console.ReadLine();
        }

        private static string _getBearerAuthToken()
        {
            var encodedKey = HttpUtility.UrlEncode(consumerApiKey);
            var encodedSecret = HttpUtility.UrlEncode(consumerApiSecretKey);
            var concatKeySecret = $"Concatened Encoded Key: {encodedKey}:{encodedSecret}";
            cLog(concatKeySecret);

            client.DefaultRequestHeaders.Accept.Clear();
            client.DefaultRequestHeaders.Add("User-Agent", ".Brad Testing");
            client.DefaultRequestHeaders.Add("Content-Type", "application/x-www-form-urlencoded;charset=UTF-8");
            client.DefaultRequestHeaders.Add("Content-Length", "29");
            client.DefaultRequestHeaders.Add("Accept-Encoding", "gzip");
            client.DefaultRequestHeaders.Add("Authorization", $"Basic {concatKeySecret}");

            // var stringTask = client.GetStringAsync("https://api.github.com/orgs/dotnet/repos");
            string msg = client.GetStringAsync(TwitterEndpoints.OA_TOKEN).Result;
            return msg;
        }

        private static void _showWelcomeMessage()
        {
            Console.ForegroundColor = ConsoleColor.Blue;
            cLog("*************************************************************");
            cLog("*                        Genie                              *");
            cLog("*                                                           *");
            cLog("*                        _.---.__                           *");
            cLog("*                       .'        `-.                       *");
            cLog("*                     /      .--.   |                       *");
            cLog("*                  \\/  / /    | _ /                        *");
            cLog("*                  `\\/|/ _(_)                              *");
            cLog("*                ___ /| _.--'    `.   .                     *");
            cLog("*               \\  `--' .---.    \\ /|                     *");
            cLog("*                 )   `      \\     //|                     *");
            cLog("*                 | __    __ | '/||                         *");
            cLog("*                 |/ \\  / \\      / ||                     *");
            cLog("*                 ||  |  |  \\    \\  |                     *");
            cLog("*                \\|  |  |   /        |                     *");
            cLog("*                __\\@/  |@ | ___\\--'                      *");
            cLog("*               (     / ' `--'  __) |                       *");
            cLog("*              __ > (  .  .--' &'\\                         *");
            cLog("*             /   `--| _ / --'     &  |                     *");
            cLog("*             |                 #. |                        *");
            cLog("*             | q# |                                        *");
            cLog("*             \\              ,ad#'                         *");
            cLog("*               `.________.ad####'                          *");
            cLog("*                 `#####\"\"\"\"\"\"\'\'                    *");
            cLog("*                  `&#\"                                    *");
            cLog("*                   &# \" &                                 *");
            cLog("*                   \"#ba\" *                               *");
            cLog("*                                                           *");
            cLog("*************************************************************");
            Console.ForegroundColor = ConsoleColor.Green;
        }



        // private static async string fetchUserTimeline()
        // {
        //     cLog("Fetching timeline for user...");
        //     string queryUrl = $"{TwitterApiBaseUrl}statuses/home_timeline.json?count=200?since_id=";
        //     cLog($"HTTP GET = {queryUrl}");
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

        private static string ProcessRepositories()
        {
            client.DefaultRequestHeaders.Accept.Clear();
            client.DefaultRequestHeaders.Accept.Add(
                new MediaTypeWithQualityHeaderValue("application/vnd.github.v3+json"));
            client.DefaultRequestHeaders.Add("User-Agent", ".NET Foundation Repository Reporter");
            client.DefaultRequestHeaders.Add("Authorization", "OAuth oauth_consumer_key=" + consumerApiKey + ", oauth_nonce=" + RandomString(32) + ", oauth_signature=" + HttpUtility.UrlEncode(consumerApiSecretKey) + "&" + HttpUtility.UrlEncode(accessTokenSecret) + ", oauth_signature_method=HMAC-SHA1" + ", oauth_timestamp=" + DateTimeOffset.Now.ToUnixTimeSeconds() + ", oauth_token=" + accessToken + ", oauth_version=1.0"
            );

            // var stringTask = client.GetStringAsync("https://api.github.com/orgs/dotnet/repos");
            string stringTask = client.GetStringAsync(TwitterEndpoints.REST_ROOT_URL + "statuses/user_timeline.json?screen_name=__bradmartin__&count=2").Result;

            var msg = stringTask;
            return msg;
        }

        private static Random random = new Random();
        public static string RandomString(int length)
        {
            const string chars = "ABCDEFGHIJKLMNOPQRSTUVWXYZ0123456789";
            return new string(Enumerable.Repeat(chars, length)
              .Select(s => s[random.Next(s.Length)]).ToArray());
        }

    }
}

