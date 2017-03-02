using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Shared.Naive;
using Shared.Interfaces;

namespace NaiveHashtag
{
    public class NaiveHashtag : INaiveHashtag
    {
        private readonly INaiveRepository _repo;

        public NaiveHashtag(INaiveRepository repo)
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
        public ICollection<string> getHashtags(IEnumerable<string> tweet, int numberOfHashtags)
        {
            NaiveList list = _repo.GetNaiveList(tweet);
            ICollection<HashtagAndCount> topHashtags = Score(list, numberOfHashtags);

            return MakeStringCollection(topHashtags);
        }//end of getHashtags

        private ICollection<string> MakeStringCollection(ICollection<HashtagAndCount> list)
        {
            ICollection<string> newList = new List<string>();
            foreach(HashtagAndCount word in list)
            {
                newList.Add(word.Hashtag);
            }//end of foreac
            return newList;
        }//end of MakeStringArray.

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
            while(collisionList.Count > numberOfHashtags)
            {
                ICollection<HashtagAndCount> newList = new List<HashtagAndCount>();
                foreach(HashtagAndCount tag in collisionList)
                {
                    if(tag.Count > threashold)
                    {
                        newList.Add(tag);     
                    }//end of if
                }//end of foreach
                collisionList = newList;
                threashold++;
            }//end of while loop
            ICollection<HashtagAndCount> listWithCount = RecreateCount(naiveList, collisionList);
            return listWithCount.OrderBy(tag => tag.Count).ToList();
        }//end of score


        /// <summary>
        /// This recreates the hashtagList with the original Count and organized by how often the count appeared. 
        /// </summary>
        /// <param name="naiveList"></param>
        /// <param name="collisionList"></param>
        /// <returns></returns>
        private ICollection<HashtagAndCount> RecreateCount(NaiveList naiveList, ICollection<HashtagAndCount> collisionList)
        {
            ICollection<HashtagAndCount> recreated = new List<HashtagAndCount>();
            foreach (HashtagAndCount tag in collisionList)
            {
                tag.Count = FindCount(naiveList, tag.Hashtag);
            }
            return recreated;
        }//end of recreatecount

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
                    }//end of if
                }//end of inner foreach
            }//end of outer foreach
            return 0;
        }//end of FindCount

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
            foreach(NaiveWord tag in list.Words)
            {
               
                foreach(HashtagAndCount tagAndCount in tag.Hashtags)
                {
                    if(!hashList.Contains(tagAndCount.Hashtag))
                    {
                        hashList.Add(tagAndCount.Hashtag);
                    }//end of if
                    allHashtags.Add(tagAndCount.Hashtag);
                }//end of inner foreach
            }//end of foreach

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
                    }//end of if
                }//end of inner foreach
                nuTag.Count = count;
                count = 0;
                collisionList.Add(nuTag);
            }//end of outter for each
            

            return collisionList;
        }//end of CreateCollis


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
            }//end of foreach
            return list;
        }
    }//end of NaiveHastag class
}//end of name space
