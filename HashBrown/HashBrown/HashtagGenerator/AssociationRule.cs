using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace HashtagGenerator
{
    public class AssociationRule
    {
        public IEnumerable<string> Antecedents { get; set; }
        public IEnumerable<string> Consequents { get; set; }
        public double Confidence { get; set; }
        public double Support { get; set; }

        public bool MatchAntecedents(IEnumerable<string> words)
        {
            bool result = true; 

            foreach(var antecedent in Antecedents)
            {
                result = words.Contains(antecedent) ? result && true : result && false;
            }

            return result;
        }
    }
}
