using Microsoft.VisualStudio.TestTools.UnitTesting;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Test.HashtagGenerator;

namespace HashtagGenerator.Test
{
    [TestClass]
    public class AssociateRulesTest
    {
        [TestMethod]
        public void TestMatchAntecedentReturnsTrue()
        {
            var ar = new AssociationRule
            {
                Antecedents = new List<string>
                {
                    "woman",
                    "man"
                },
                Consequents = new List<string>
                {
                    "babies"
                },
                Confidence = 12.00,
                Support = 13.00
            };

            if (!ar.MatchAntecedents(new List<string> { "woman", "banana", "phone", "man" }))
            {
                throw new Exception();
            }
        }
    }
}
