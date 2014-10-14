﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using NUnit.Framework;

namespace BasicTypes.Glosser
{
    //Can't delegate glossing to parts because you need to know position to gloss!
    [TestFixture]
    public class GlossMakerTests
    {
        [Test]
        public void NounPhrase()
        {
            GlossMaker gm = new GlossMaker();
            Console.WriteLine(gm.Gloss("jan suli"));
        }

        [Test]
        public void SimplePredicate()
        {
            GlossMaker gm = new GlossMaker();
            Console.WriteLine(gm.Gloss("jan li suli"));
        }


        [Test]
        public void SimplePredicateConjunction()
        {
            GlossMaker gm = new GlossMaker();
            Console.WriteLine(gm.Gloss("jan li suli li wawa"));
        }

        [Test]
        public void TransitiveVerb()
        {
            GlossMaker gm = new GlossMaker();
            Console.WriteLine(gm.Gloss("jan li moku e kili"));
        }

        [Test]
        public void IntransitiveVerb()
        {
            GlossMaker gm = new GlossMaker();
            Console.WriteLine(gm.Gloss("jan li moku kepeken ilo moku"));
        }
    }
}
