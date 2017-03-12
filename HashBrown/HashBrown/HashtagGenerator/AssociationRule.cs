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

        public override string ToString()
        {
            StringBuilder b = new StringBuilder();

            b.Append(string.Join(", ", Antecedents));
            b.Append(" => ");
            b.Append(string.Join(", ", Consequents));
            b.Append($" -- Confidence={Confidence:0.0}, Support={Support:0.0000}");

            return b.ToString();
        }
    }
}
