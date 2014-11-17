﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization;
using System.Text.RegularExpressions;
using BasicTypes.Collections;
using BasicTypes.CollectionsDegenerate;
using BasicTypes.Exceptions;
using BasicTypes.Extensions;

namespace BasicTypes
{
    [DataContract]
    [Serializable]
    public partial class Sentence : IContainsWord, IFormattable, IToString
    {
        [DataMember]
        private readonly Sentence conclusion;

        [DataMember]
        private readonly Sentence[] preconditions;

        //Breaks immutability :-(
        [DataMember]
        public List<Fragment> LaFragment { get; set; }

        [DataMember]
        private readonly ComplexChain fragments; //not used????

        [DataMember]
        private readonly ComplexChain subjects;
        [DataMember]
        private readonly PredicateList predicates;
        [DataMember]
        private readonly Punctuation punctuation;

        [DataMember]
        private readonly Particle conjunction;

        [DataMember]
        private readonly Vocative vocative; //Degenerate sentence, maybe should be a subclass?

        [DataMember]
        private readonly Exclamation exclamation;//Degenerate sentence, maybe should be a subclass?


        [DataMember]
        private readonly Fragment fragment;

        [DataMember]
        private readonly bool isHortative; //o mi mute li moku e kili.

        [DataMember]
        private readonly string original;

        [DataMember]
        private readonly string normalized;

        public Sentence(Sentence[] preconditions = null, Sentence conclusion = null, string original = null, string normalized = null)
        {
            LaFragment = new List<Fragment>();
            if (preconditions != null && preconditions.Length > 0 && conclusion == null)
            {
                throw new TpSyntaxException("There must be a head sentence (conclusions) if there are preconditions.");
            }
            this.conclusion = conclusion;
            
            this.preconditions = preconditions;//Entire sentences.       


            if (conclusion != null && conclusion.punctuation == null)
            {
                throw new InvalidOperationException("Conclusions require punctuation, if only through normalization");
            }
            if (preconditions != null)
            {
                foreach (Sentence precondition in preconditions)
                {
                    precondition.HeadSentence = conclusion;

                    if (precondition.punctuation != null)
                    {
                        throw new InvalidOperationException("Preconditions should have no punctuation.");
                    }
                }
            }
            this.original = original;
            this.normalized = normalized;
        }

        public Sentence(Exclamation exclamation, Punctuation punctuation, string original = null, string normalized = null)
        {
            this.exclamation = exclamation;
            this.punctuation = punctuation;

            this.original = original;
            this.normalized = normalized;
        }

        //Suggest that vocatives don't chain.  o jan o meli o soweli => o jan! o meli! o soweli!
        public Sentence(Vocative vocative, Punctuation punctuation, string original = null, string normalized = null)
        {
            this.vocative = vocative;
            this.punctuation = punctuation;

            this.original = original;
            this.normalized = normalized;
        }

        public Sentence(Fragment fragment, Punctuation punctuation, string original = null, string normalized = null)
        {
            this.fragment = fragment;
            this.punctuation = punctuation;

            this.original = original;
            this.normalized = normalized;
        }

        //Preconditions
        //public Sentence(Chain subjects, PredicateList predicates, string original = null, string normalized = null)
        //{
        //    LaFragment = new List<Chain>();
        //    this.subjects =  subjects ; //only (*), o, en
        //    this.predicates = predicates; //only li, pi, en
        //
        //    this.original = original;
        //    this.normalized = normalized;
        //}
        
        //Simple Sentences
        public Sentence(ComplexChain subjects, PredicateList predicates, SentenceOptionalParts parts=null, string original = null, string normalized = null)
        {
            LaFragment = new List<Fragment>();
            this.subjects =  subjects ; //only (*), o, en
            this.predicates = predicates; //only li, pi, en
            if (parts != null)
            {
                punctuation = parts.Punctuation;
                conjunction = parts.Conjunction;
            }
            
            this.original = original;
            this.normalized = normalized;
        }

        public Sentence BindSeme(Sentence question)
        {
            //Strong bind, all words match except seme
            //Weak bind, all head words match except seme.
            if (SentenceByValue.Instance.Equals(this, question))
            {
                //unless this is a question too!
                return this; //Echo! Better to return yes!
            }

            if (question.Contains(Words.seme))
            {
                //if (SentenceSemeEqual.Instance.Equals(this, question))
                //{
                //    //We have a match!
                //    //Compress & return.
                //}
            }

            //Null bind, replace seme phrase with jan ala
            //Or don't know.
            return null;
        }

        public bool Contains(Word word)
        {
            if (subjects != null)
            {
                if (subjects.Contains(word)) return true;
            }
            if (predicates != null)
            {
                if (predicates.Contains(word)) return true;
            }
            return false;
        }

        public bool IsTrue()
        {
            return false;
        }


        //If we have the following 2, we don't have the others (except punct).
        public Sentence[] Preconditions { get { return preconditions; } }
        public Sentence Conclusion { get { return conclusion; } } //Only preconditions have these.
        public Sentence HeadSentence { get; private set; } //Only preconditions have these.

        //Also an odd ball.
        public Vocative Vocative { get { return vocative; } }
        public Fragment Fragment { get { return fragment; } }

        public Particle Conjunction { get { return conjunction; } } //Anu, taso
        public ComplexChain Subjects { get { return subjects; } } //jan 
        public PredicateList Predicates { get { return predicates; } }//li verb li noun li prep phrase
        public Punctuation Punctuation { get { return punctuation; } } //.?!

        public Sentence EquivallencyGenerator()
        {
            return (Sentence)MemberwiseClone();
        }

        public IContainsWord[] Segments()
        {
            List<IContainsWord> w = new List<IContainsWord>();
            w.AddRange(Predicates);
            w.Add(Subjects);
            return w.ToArray();
        }

        public string[] SupportedsStringFormats
        {
            get
            {
                return new[] { "g", "b", "bs" };
            }
        }

        public string ToString(string format)
        {
            if (format == null)
            {
                format = "g";
            }
            return ToString(format, Config.CurrentDialect);
        }

        public override string ToString()
        {
            return ToString("g", Config.CurrentDialect);
        }

        public string ToString(string format, IFormatProvider formatProvider)
        {
            if (format == null)
            {
                format = "g";
            }
            List<string> sb = new List<string>();
            string spaceJoined;
            if (preconditions != null)
            {
                foreach (Sentence precondition in preconditions)
                {
                    sb.AddRange(precondition.ToTokenList(format, formatProvider));
                }
                sb.Add(Particles.la.ToString(format, formatProvider));
                sb.AddRange(conclusion.ToTokenList(format, formatProvider));

                if (sb[sb.Count() - 1] == "li")
                {
                    throw new InvalidOperationException("Something went wrong, sentence ends in li");
                }
                spaceJoined = sb.SpaceJoin(format);
                if (conclusion.punctuation != null)
                {
                    spaceJoined = spaceJoined + conclusion.punctuation;//format, formatProvider
                }
            }
            else
            {
                //Simple sentence
                sb = ToTokenList(format, formatProvider);

                if (sb[sb.Count() - 1] == "li")
                {
                    throw new InvalidOperationException("Something went wrong, sentence ends in li");
                }
                spaceJoined = sb.SpaceJoin(format);
                
                if (punctuation != null)
                {
                    spaceJoined = spaceJoined + punctuation;//format, formatProvider
                }
            }

            

            if (format != "bs")
            {
                string result = Denormalize(spaceJoined, format);
                while (result.Contains(" , "))
                {
                    result = result.Replace(" , ", ", ");
                }
                return result;
            }
            return spaceJoined;
        }

        public List<string> ToTokenList(string format, IFormatProvider formatProvider)
        {
            List<string> sb = new List<string>();

            if (conjunction != null)
            {
                sb.AddIfNeeded("|", format);
                sb.Add(conjunction.Text);
                sb.AddIfNeeded("|", format);
            }

            //TODO Vocative sentences
            //[chain]o[!.?]
            if (vocative != null)
            {
                sb.AddRange(vocative.ToTokenList(format,formatProvider));
            }
            else if (fragment != null)
            {
                sb.AddRange(fragment.ToTokenList(format, formatProvider));
            }
            else if (exclamation!= null)
            {
                sb.AddRange(exclamation.ToTokenList(format, formatProvider));
            }
            else
            {
                if (LaFragment != null)
                {
                    foreach (Fragment chain in LaFragment)
                    {
                        sb.AddIfNeeded("{",format);
                        sb.AddRange(chain.ToTokenList(format, formatProvider));
                        sb.Add(Particles.la.ToString(format, formatProvider));
                        sb.AddIfNeeded("}",format);   
                    }
                }

                //Unless it is an array, delegate to member ToString();
                if (subjects != null)
                {
                    //Should only happen for imperatives
                    sb.AddIfNeeded("[",format);
                    sb.Add(subjects == null ? "[NULL]" : subjects.ToString(format, formatProvider));
                        //subjects.Select(x => x == null ? "[NULL]" : x.ToString(format, formatProvider)), format, formatProvider, false);
                    sb.AddIfNeeded("]",format);
                }
                else
                {
                    //Not surprising if it is an imperative.
                    if (Predicates.All(x => x.Particle.Text != Particles.o.Text))
                    {
                        Console.WriteLine("This was surprising.. no subjects and AFAIK, this isn't an imperative of any sort.");
                    }
                }

                sb.AddIfNeeded("<",format);
                sb.AddRange(Predicates.ToTokenList(format, formatProvider));
                sb.AddIfNeeded(">",format);
            }


            return sb;
        }

        private bool NeedToReplace(string value, string pronoun)
        {
            bool startsWith = value.Contains(pronoun+ " li") && value.StartsWith(pronoun+ " ");
            if(startsWith) return true;

            bool followsConditional = value.Contains(pronoun + " li") && value.Contains(" la "+pronoun+" li ");
            if(followsConditional) return true;

            bool followsPunctuation = value.Contains(pronoun + " li") && value.Contains(". " + pronoun + " li ");
            if (followsPunctuation) return true;

            bool followsColon = value.Contains(pronoun + " li") && value.Contains(": " + pronoun + " li ");
            if (followsColon) return true;

            return false;
        }
    

        private string Denormalize(string value, string format)
        {
            if (format == null)
            {
                format = "g";
            }
            //tenpo kama la jan lili mi li toki e ni
            if (NeedToReplace(value,"mi"))
            {
                Regex r = new Regex(@"\bmi li\b");
                value = r.Replace(value, "mi");
            }

            if (NeedToReplace(value, "sina"))
            {
                Regex r = new Regex(@"\bsina li li\b");
                value = r.Replace(value, "sina");
            }

            if (value.Contains("~"))
            {
                value = value.Replace("~", ", ");
            }
            if (value.Contains("li ijo Nanunanuwakawakawawa."))
            {
                value = value.Replace("li ijo Nanunanuwakawakawawa.", format.StartsWith("b")?"[NULL TOKEN]":"");
            }
            if (value.Contains("[NULL]") && !format.StartsWith("b"))
            {
                value = value.Replace("[NULL]","");
            }
            return value;
        }


        

        public bool HasPunctuation()
        {
            return Punctuation != null;
        }
        public bool HasConjunction()
        {
            return Conjunction != null;
        }
    }



}

