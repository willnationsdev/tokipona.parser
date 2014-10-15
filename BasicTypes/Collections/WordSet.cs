﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization;
using System.Text;
using System.Threading.Tasks;

namespace BasicTypes
{
    //Unordered words.-- uh oh, this may be same concept as headed phrase.
    [Serializable]
    public class WordSet : HashSet<Word>, IContainsWord
    {
        public WordSet():base()
        {
        }
        public WordSet(SerializationInfo info, StreamingContext context)
            : base(info, context)
        {
        }

        public WordSet(IEnumerable<string> strings)
            : base()
        {
            foreach (string s in strings)
            {
                this.Add(new Word(s));
            }
        }

        //Same if same jumble of words. E.g. the set of words following a noun.

        public override string ToString()
        {
            if (this.Count == 0) return "";
            return string.Join(" ", this.Select(x=>x.Text).ToArray());
        }

        public static WordSet Parse(string value)
        {
            return WordSetTypeConverter.Parse(value);
        }

        public static void TryParse(string value, out WordSet result)
        {
            try
            {
                result = WordSetTypeConverter.Parse(value);
            }
            catch (Exception)
            {
                result = null;
            }
        }
    }

    
}