using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Shared.Naive;
using Shared.Interfaces;

namespace NaiveHashtag
{
    public class Naive : INaiveHashtag
    {
        private readonly INaiveRepository _repo;

        public Naive(INaiveRepository repo)
        {
            if (repo == null)
            {
                throw new ArgumentNullException();
            }

            _repo = repo;
        }

        /// <summary>
        /// This gets the top hashtags from the group of words in the tweet. 
        /// </summary>
        /// <param name="tweet"></param>
        /// <param name="numberOfHashtags"></param>
        /// <returns></returns>
        public IEnumerable<string> getHashtags(IEnumerable<string> tweet, int numberOfHashtags)
        {
            NaiveList list = _repo.GetNaiveList(tweet);

            IEnumerable<HashtagAndCount> topHashtags = Score(list, numberOfHashtags);

            return topHashtags.Select(x => x.Hashtag);
        }

        [Obsolete("Don't use", true)]
        private ICollection<string> MakeStringCollection(ICollection<HashtagAndCount> list)
        {
            ICollection<string> newList = new List<string>();

            foreach(HashtagAndCount word in list)
            {
                newList.Add(word.Hashtag);
            }

            return newList;
        }

        /// <summary>
        /// This scores all the hashtags by counting how many times they appear in the otherhashtag lists, then it removes
        /// them until there is enough for the amount of hashtags the user requested. 
        /// </summary>
        /// <param name="naiveList"></param>
        /// <param name="numberOfHashtags"></param>
        /// <returns></returns>
        private ICollection<HashtagAndCount> Score(NaiveList naiveList, int numberOfHashtags)
        {
            int threashold  = 0;
            ICollection<HashtagAndCount> collisionList = CreateCollisionList(naiveList);

            // group by hashtag and sum the counts for each one
            //IEnumerable<HashtagAndCount> collisionList = naiveList.Words
            //    .SelectMany(x => x.Hashtags)
            //    .GroupBy(x => x.Hashtag)
            //    .Select(x => new HashtagAndCount
            //    {
            //        Hashtag = x.Key,
            //        Count = x.Sum(y => y.Count)
            //    });


            //IEnumerable<HashtagAndCount> rankedHashtags = collisionList.OrderByDescending(x => x.Count).Take(numberOfHashtags);


            // "chop" off the hashtags with the lowest count to meet a threshold
            var topCollisionList = collisionList.Take(numberOfHashtags);
            //while (collisionList.Count() > numberOfHashtags)
            //{
            //    ICollection<HashtagAndCount> newList = new List<HashtagAndCount>();
            //    foreach(HashtagAndCount tag in collisionList)
            //    {
            //        // filter hashtags if they don't meet a threshold count
            //        if(tag.Count > threashold)
            //        {
            //            newList.Add(tag);     
            //        }
            //    }
            //    collisionList = newList;

            //    // increase threshold until the collisiong list is large enough
            //    threashold++;
            //}

            ICollection<HashtagAndCount> listWithCount = RecreateCount(naiveList, topCollisionList.ToList());

            return listWithCount.OrderBy(tag => tag.Count).ToList();
        }


        /// <summary>
        /// This recreates the hashtagList with the original Count and organized by how often the count appeared. 
        /// </summary>
        /// <param name="naiveList"></param>
        /// <param name="collisionList"></param>
        /// <returns></returns>
        private ICollection<HashtagAndCount> RecreateCount(NaiveList naiveList, ICollection<HashtagAndCount> collisionList)
        {
            foreach (HashtagAndCount tag in collisionList)
            {
                tag.Count = FindCount(naiveList, tag.Hashtag);
            }
            return collisionList;
        }

        /// <summary>
        /// This method finds the original count of hashtags with how often they appear in the database
        /// This way The hashtags can be organized based off of how often they appear in the database. 
        /// </summary>
        /// <param name="list"></param>
        /// <param name="tag"></param>
        /// <returns>
        /// the count of teh word 
        /// </returns>
        private int FindCount(NaiveList list, string tag)
        {
            foreach(NaiveWord word in list.Words)
            {
                foreach(HashtagAndCount hash in word.Hashtags)
                {
                    if(hash.Hashtag == tag)
                    {
                        return hash.Count;
                    }
                }
            }
            return 0;
        }

        /// <summary>
        /// I want to create a list of all the hashtags with count 0 then,
        /// as I go through I want to keep track of how man times the hashtag appeared in the group associated with each word. 
        /// </summary>
        /// <param name="list"></param>
        /// <returns>
        /// return the Collection of words and weight with how many times it appears in the out come. 
        /// </returns>
        private ICollection<HashtagAndCount> CreateCollisionList(NaiveList list)
        {

            ICollection<HashtagAndCount> collisionList = new List<HashtagAndCount>();
            ICollection<string> hashList = new List<string>();
            ICollection<string> allHashtags = new List<string>();

            // Make unique list of hashtags, and a global list of hashtags
            foreach(NaiveWord tag in list.Words)
            {               
                foreach(HashtagAndCount tagAndCount in tag.Hashtags)
                {
                    if(!hashList.Contains(tagAndCount.Hashtag))
                    {
                        hashList.Add(tagAndCount.Hashtag);
                    }

                    allHashtags.Add(tagAndCount.Hashtag);
                }
            }

            // counting the number of occurences of each hashtag
            foreach(string word in hashList)
            {
                int count = 0;
                HashtagAndCount nuTag = new HashtagAndCount();
                nuTag.Hashtag = word;
                foreach(string compare in allHashtags)
                {
                    if(compare == word)
                    {
                        count++;
                    }
                }
                nuTag.Count = count;
                count = 0;
                collisionList.Add(nuTag);
            }
            
            // aggregate count of each hashtag
            return collisionList;
        }


        /// <summary>
        /// This methdo is only for testing. It should call this method in a different class to 
        /// get the list
        /// </summary>
        /// <param name="words"></param>
        /// <returns>
        /// NaiveList of all the words in the text and all the hashtags that are accosiated with it. 
        /// </returns>
        public NaiveList HashtagPairs(ICollection<string> words)
        {
            //TODO: create it so it goes to correct Method. Right now I am going to use this to create a list
            // for testing later it will actually do the thing.
            Random rnd = new Random();
            NaiveList list = new NaiveList();
            foreach(string word in words )
            {
                ICollection<HashtagAndCount> hashList = new List<HashtagAndCount>();
                HashtagAndCount hashAndCount = new HashtagAndCount();
                hashAndCount.Count = rnd.Next(1, 50);
                hashAndCount.Hashtag = "#" + word;
                hashList.Add(hashAndCount);
                list.Add(word, hashList);
            }
            return list;
        }
    }
}
