using Shared.Naive;
using System.Collections.Generic;

namespace Shared.Interfaces
{
    public interface INaiveRepository
    {
        NaiveList GetNaiveList(IEnumerable<string> words);
    }
}
