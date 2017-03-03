using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Shared.Naive;

namespace NaiveHashtag
{
    interface INaiveHashtag
    {
        /// <summary>
        /// From Naive Hashtag I will send a ICollection of words and then will receive a collection of hashtags. 
        /// </summary>
        /// <param name="word">
        /// The words are a collection of words from the tweet and they should be run though the database to see what hashtags are associated with them
        /// and the count for each hashtag
        /// </param>
        /// <returns>
        /// a NaiveList that is a collection fo the words and the hashtags and the count fo the hashtags.  
        /// </returns>
         NaiveList HashtagPairs(ICollection<string> words);
    }
}
