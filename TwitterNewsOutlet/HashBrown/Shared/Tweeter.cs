﻿using System.Collections.Generic;

namespace Shared
{
    public class Tweeter
    {
        public ICollection<string> WordSet { get; set; }
        public ICollection<string> HashtagSet { get; set; }
    }
}