using System;
using Tweetinvi;
using Tweetinvi.Streaming;

namespace TwitterTest
{
    class Program
    {
        // DO NOT CHANGE THESE STRINGS!!!!!!! 
        // TWITTER WON'T LET YOU CONNECT IF YOU DO!
        private static readonly string consumerKey = "LcEGpecA6NONZTg7dXPoNiRwz";
        private static readonly string consumerSecret = "n0WnhprYUTBFexZl3xUG4tqa5AHgccqxGawtrh3m9D9YSdvrn9";
        private static readonly string userAccessToken = "826956744962174976-BIBQPRIY2UewtaJXIhEZudLVAqDute0";
        private static readonly string userAccessSecret = "J61blvOP4uyDTRzJifMKOdju4MTYdMD7JwOhfib4gF1zk";

        static void Main()
        {
            Auth.SetUserCredentials(consumerKey, consumerSecret, userAccessToken, userAccessSecret);

            ISampleStream stream = Stream.CreateSampleStream();
            stream.TweetReceived += (sender, args) =>
            {
                Console.WriteLine(args.Tweet);
            };
            stream.StartStream();
        }
    }
}
