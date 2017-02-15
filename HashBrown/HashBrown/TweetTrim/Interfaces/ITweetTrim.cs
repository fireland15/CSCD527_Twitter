using Shared;

namespace TweetTrim
{
   public interface ITweetTrim
    {
        /// <summary>
        /// Trims the tweet to remove stop words and misspelled words (?)
        /// </summary>
        /// <param name="tweeter"></param>
        /// <returns></returns>
        Tweeter Trim(Tweeter tweeter);
    }
}

