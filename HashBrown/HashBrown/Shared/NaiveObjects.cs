using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Shared.Naive
{
    /// <summary>
    /// This class is the creation of the objects that I need for the naive class
    /// </summary>
    public class HashtagAndCount
    {
        public int Count { set; get; }
        public string Hashtag { set; get; }
    }

    public class NaiveWord
    {
        public string Word { get; set; }
        public ICollection<HashtagAndCount> Hashtags { get; set; }
    }


    public class NaiveList
    {
        public ICollection<NaiveWord> Words { get; set; } = new List<NaiveWord>();

        public void Add(string word, ICollection<HashtagAndCount> hashtags)
        {

            Words.Add(new NaiveWord
            {
                Word = word,
                Hashtags = hashtags
            });
        }
    }
}
