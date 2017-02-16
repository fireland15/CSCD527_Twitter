using System.Collections.Generic;

namespace Shared
{
    public class Tweeter
    {
        private object WordSet1;
        private object HashTagSet;

        public Tweeter(object wordSet, object hashTagSet)
        {
            this.WordSet = WordSet;
            this.HashTagSet = hashTagSet;
        }

        public ICollection<string> WordSet { get; set; }
        public ICollection<string> HashtagSet { get; set; }

        public bool IsValid()
        {
            return !(WordSet.Count == 0 || HashtagSet.Count == 0);
        }
    }
}
