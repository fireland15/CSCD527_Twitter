using System.Collections.Generic;
using System.Linq;
using Tweetinvi.Models.Entities;

namespace Shared
{
    public class Tweeter
    {
		public Tweeter()
		{
			
		}
		
        public Tweeter(string text, ICollection<IHashtagEntity> hashTagSet)
        {
            WordSet = SplitText(text);
            HashtagSet = hashTagSet.Select(h => h.Text).ToList();
        }

        public ICollection<string> WordSet { get; set; }
        public ICollection<string> HashtagSet { get; set; }

        public bool IsValid()
        {
            return !(WordSet.Count == 0 || HashtagSet.Count == 0);
        }

        /// <summary>
        /// Splits a text string on the following characters
        /// whitespace.,/!@$%^&*()\\:;\"'<>?
        /// </summary>
        /// <param name="text"></param>
        /// <returns></returns>
        private ICollection<string> SplitText(string text)
        {
            return text.Split(" .,/!@$%^&*()\\:;\"'<>?".ToCharArray())
                .Where(s => s.Length != 0)
                .Select(s => s.ToLower())
                .ToList();
        }
    }
}
