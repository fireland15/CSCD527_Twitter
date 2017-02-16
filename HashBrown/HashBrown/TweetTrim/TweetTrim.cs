using System;
using Shared;
using System.Collections.Generic;

namespace TweetTrim
{
    public class TweetTrim : ITweetTrim
    {
        public Tweeter Trim(Tweeter tweeter)
        {
            //throw new NotImplementedException();
            return tweeter;
        }

        private ICollection<string> Remove(ICollection<string> wordsToRemove, IEnumerable<string> wordSet)
        {
            throw new NotImplementedException();
        }

        private ICollection<string> Validate(ICollection<string> wordsToKeep, IEnumerable<string> wordSet)
        {
            throw new NotImplementedException();
        }
    }
}
