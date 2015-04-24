using Microsoft.ServiceBus.Messaging;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Tweetinvi;
using Tweetinvi.Core.Interfaces;
using Tweetinvi.Core.Interfaces.oAuth;

namespace SJKP.AzureBootCamp2015.TwitterIngest
{
    class Program
    {
        static string EventHubName = "TweetHub";
        static StreamWriter fs = new StreamWriter(File.Open("tweetoutput.json", FileMode.OpenOrCreate));
        static EventHubClient tweetHubClient;
        static void Main(string[] args)
        {

            //Create the event hub
            var manager = Microsoft.ServiceBus.NamespaceManager.CreateFromConnectionString(ConfigurationManager.AppSettings["Microsoft.ServiceBus.ConnectionString"]);
            var tweethub = manager.CreateEventHubIfNotExists("TweetHubSimple");
            tweetHubClient = EventHubClient.Create(tweethub.Path);

            var oauth = CredentialsCreator_CreateFromRedirectedCallbackURL_SingleStep(ConfigurationManager.AppSettings["twitterkey"], ConfigurationManager.AppSettings["twittersecret"]);
            // Setup your credentials
            TwitterCredentials.SetCredentials(oauth.AccessToken, oauth.AccessTokenSecret, oauth.ConsumerKey, oauth.ConsumerSecret);

            
            // Access the filtered stream
            var filteredStream = Tweetinvi.Stream.CreateFilteredStream();
            filteredStream.AddTrack("globalazurebootcamp");
            filteredStream.AddTrack("azure");
            filteredStream.AddTrack("microsoft");
            filteredStream.MatchingTweetReceived += (sender, a) => {
                Console.WriteLine(a.Tweet.Text);
                var str = JsonConvert.SerializeObject(new
                {
                    Tweet = a.Tweet.Text,
                    Lang = a.Tweet.Language,
                    Created_At = a.Tweet.CreatedAt
                });
                tweetHubClient.Send(new EventData(System.Text.Encoding.UTF8.GetBytes(str)));

            };
            //filteredStream.JsonObjectReceived += (sender, json) =>
            //{
            //    ProcessTweet(json.Json);
            //};
            filteredStream.StartStreamMatchingAllConditions();
        }

        private static void ProcessTweet(string tweet)
        {
            //fs.WriteLine(tweet);
            tweetHubClient.Send(new EventData(System.Text.Encoding.UTF8.GetBytes(tweet)));
        }

        private static IOAuthCredentials CredentialsCreator_CreateFromRedirectedCallbackURL_SingleStep(string consumerKey, string consumerSecret)
        {
            //if (string.IsNullOrEmpty(User.Default.TwitterAccessKey) || string.IsNullOrEmpty(User.Default.TwitterAccessToken))
            //    {
                Func<string, string> retrieveCallbackURL = url =>
                {
                    Console.WriteLine("Go on : {0}", url);
                    Console.WriteLine("When redirected to your website copy and paste the URL: ");

                    // Enter a value like: https://tweeetinvi.codeplex.com?oauth_token={tokenValue}&oauth_verifier={verifierValue}

                    var callbackURL = Console.ReadLine();
                    return callbackURL;
                };

                // Here we provide the entire URL where the user has been redirected
                var newCredentials = CredentialsCreator.GetCredentialsFromCallbackURL_UsingRedirectedCallbackURL(retrieveCallbackURL, consumerKey, consumerSecret, "https://tweetinvi.codeplex.com");
                Console.WriteLine("Access Token = {0}", newCredentials.AccessToken);
                Console.WriteLine("Access Token Secret = {0}", newCredentials.AccessTokenSecret);
                User.Default.TwitterAccessKey = newCredentials.AccessToken;
                User.Default.TwitterAccessToken = newCredentials.AccessTokenSecret;
                User.Default.Save();
                return newCredentials;
            //}
            return TwitterCredentials.CreateCredentials(User.Default.TwitterAccessToken, User.Default.TwitterAccessKey, consumerKey, consumerSecret);
        }
    }
}
