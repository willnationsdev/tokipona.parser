﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization;
using System.Text;
using System.Threading.Tasks;
using BasicTypes.Extensions;

namespace BasicTypes.Collections
{
    //Stuff after the 1st li, never itself contains li.
    [DataContract]
    [Serializable]
    public class TpPredicate : IContainsWord, IFormattable
    {
        [DataMember]
        private readonly HeadedPhrase verbPhrases;
        [DataMember]
        private readonly Chain directs;
        [DataMember]
        private readonly Chain prepositionals;

        public TpPredicate(HeadedPhrase verbPhrases, Chain directs, Chain prepositionals)
        {
            //TODO: Validate. 
            this.verbPhrases = verbPhrases; //only pi, en
            this.directs = directs;//only e, pi, en
            this.prepositionals = prepositionals;//only ~prop, pi, en
        }
        public HeadedPhrase VerbPhrases { get { return verbPhrases; } }
        public Chain Directs { get { return directs; } }
        public Chain Prepositionals { get { return prepositionals; } }
        public bool Contains(Word word)
        {
            List<IContainsWord> chains = new List<IContainsWord>() { verbPhrases, directs, prepositionals };
            return chains.Any(x => x != null && x.Contains(word));
        }

        public string ToString(string format)
        {
            return this.ToString(format, System.Globalization.CultureInfo.CurrentCulture);
        }

        public override string ToString()
        {
            return this.ToString(null, System.Globalization.CultureInfo.CurrentCulture);
        }

        public string ToString(string format, IFormatProvider formatProvider)
        {
            //Mixture of adding words, phrases adn brackets. Ugly.
            List<string> sb = new List<string>();

            sb.Add(verbPhrases.Head.ToString());
            sb.AddRange(verbPhrases.Modifiers.Select(x=>x.ToString()));


            foreach (Chain chain in new []{directs,Prepositionals})
            {
                if(chain==null)continue;
                
                sb.Add("{");
                if (chain.HeadedPhrases.Any())
                {
                    sb.Add(chain.Particle.ToString());
                }
                foreach (HeadedPhrase headedPhrase in chain.HeadedPhrases)
                {
                    sb.Add(headedPhrase.ToString(format));
                }
                sb.Add("}");
            }
            return sb.SpaceJoin(format);
        }
    }

    public class TpPredicateByValue : EqualityComparer<TpPredicate>
    {
        private static readonly TpPredicateByValue instance = new TpPredicateByValue();
        public static TpPredicateByValue Instance { get { return instance; } }

        static TpPredicateByValue() { }
        private TpPredicateByValue() { }

        public override bool Equals(TpPredicate x, TpPredicate y)
        {
            return ChainByValue.Instance.Equals(x.Directs, y.Directs)
                   && ChainByValue.Instance.Equals(x.Prepositionals, y.Prepositionals)
                   && HeadPhraseByValue.Instance.Equals(x.VerbPhrases, y.VerbPhrases);
        }

        public override int GetHashCode(TpPredicate obj)
        {
            throw new NotImplementedException();
        }
    }
}