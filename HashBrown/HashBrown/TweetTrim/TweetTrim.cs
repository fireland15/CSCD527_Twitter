using System;
using Shared;
using System.Collections.Generic;
using System.IO;
using System.Linq;

namespace TweetTrim
{
    public class TweetTrim : ITweetTrim
    {

        public TweetTrim(string stopListFileName)
        {
            //TODO: read in the stopList file as an IEnum 
            StopList = CreateStopList(stopListFileName);
            //TODO: make a list of valid words to test that words are good.
            ValidWords = null;
        }

        public IEnumerable<string> StopList { set; get; }
        public IEnumerable<string> ValidWords { set; get; }

        public IEnumerable<string> CreateStopList(string filename)
        {
            List<string> readArray = new List<string>();
            string temp;
            using (StreamReader reader = new StreamReader(filename))
            {
                temp = reader.ReadLine();
                while (temp != null)
                {
                    if (temp != "")
                    {
                        readArray.Add(temp.ToLower());
                    }
                    temp = reader.ReadLine();
                }
            }
            return readArray;
        }

        /// <summary>
        /// This takes the tweet and starts to check to see if each string input has any useless words. 
        /// </summary>
        /// <param name="tweeter">
        /// tweeter;
        /// ICollection<string> wordSet;
        /// ICollection<string> HashTagSet;
        /// </param>
        /// <returns>
        /// Tweeter: with the removed words;
        /// </returns>
        public Tweeter Trim(Tweeter tweeter)
        {
            Tweeter nuTweeter = new Tweeter
            {
                WordSet = Remove(tweeter.WordSet, StopList),
                HashtagSet = tweeter.HashtagSet
            };
            return nuTweeter;
        }

        /// <summary>
        /// This is where I send the original Tweeter's WordSet to be removed and returned. 
        /// </summary>
        /// <param name="wordsToRemove">
        /// This is the collection of   words Where I will remove from
        /// </param>
        /// <param name="wordSet">
        /// This is the set of words that I will compare too. 
        /// </param>
        /// <returns></returns>
        private ICollection<string> Remove(ICollection<string> tweetWords, IEnumerable<string> stopList)
        {
            return tweetWords.Where(t => !stopList.Contains(t)).ToList();
        }

        /// <summary>
        /// This method is make sure that it is words we can keep. 
        /// </summary>
        /// <param name="wordsToKeep">
        /// This is the collection of words that are valide to keep or remove.
        /// </param>
        /// <param name="wordSet">
        /// This is the wordSet I am processing of valid words. Use english words from an english dictionary.  
        /// </param>
        /// <returns></returns>
        private ICollection<string> Validate(ICollection<string> wordsToKeep, IEnumerable<string> wordSet)
        {
            throw new NotImplementedException();
        }
    }
}
