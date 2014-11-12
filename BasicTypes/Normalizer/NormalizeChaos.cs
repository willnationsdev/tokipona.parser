﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using BasicTypes.Parts;

namespace BasicTypes.Parser
{
    /// <summary>
    /// Assumes text is total chaos- mixed tp & foreign languages, HTML, unicode, inconsistent punctuation
    /// </summary>
    public class NormalizeChaos
    {
        public static string Normalize(string sentence)
        {
            string normalized = DetectWrongQuotes(sentence);
            normalized = DetectEntireForeignSentence(normalized);

            
            //Doesn't work!!!
            //normalized = DetectIndividualForeignWords(normalized);

            return normalized;
        }

        private static string DetectWrongQuotes(string normalized)
        {
            if (normalized.StartsWith(@"""") && normalized.EndsWith(@""""))
            {
                if (PercentTokiPona(normalized.Trim(new char[]
                {
                    '«', '»', '@', '.', '!', ' ', '?', '!', '\n', '\r', ';', ':', '<', '>', '/', '&', '$', '\'',
                    '"','(',')',
                })) > .80m)
                {
                    return "'" + normalized.Substring(1, normalized.Length - 2) + "'";
                }
            }
            if (normalized.StartsWith(@"""") )
            {
                if (PercentTokiPona(normalized.Trim(new char[]
                {
                    '«', '»', '@', '.', '!', ' ', '?', '!', '\n', '\r', ';', ':', '<', '>', '/', '&', '$', '\'',
                    '"','(',')',
                })) > .80m)
                {
                    return "'" + normalized.Substring(1, normalized.Length - 1);
                }
            }
            if (normalized.EndsWith(@""""))
            {
                if (PercentTokiPona(normalized.Trim(new char[]
                {
                    '«', '»', '@', '.', '!', ' ', '?', '!', '\n', '\r', ';', ':', '<', '>', '/', '&', '$', '\'',
                    '"','(',')',
                })) > .80m)
                {
                    return normalized.Substring(0, normalized.Length - 2) + "'";
                }
            }

            return normalized;
            
        }

        private static string DetectIndividualForeignWords(string normalized)
        {
            if (normalized.Contains(" "))
            {
                bool needFixing = false;
                List<string> normalizedTokens = new List<string>();
                string[] tokens = normalized.Split(new[] {' '});
                foreach (string token in tokens)
                {
                    //Already foreign enough.
                    string unpunctuated =
                        token.Trim(new char[]
                        {
                            '«', '»', '@', '.', '!', ' ', '?', '!', '\n', '\r', ';', ':', '<', '>', '/', '&', '$', '\'','(',')',
                            '"'
                        });
                    if (token.StartsWith(@"""") && token.EndsWith(@"""") && !token.Contains(" ")) continue;

                    var justLetters = Regex.Match(unpunctuated
                        , @"\p{L}+");
                    if (justLetters.Success)
                    {
                        if (!Token.CheckIsValidPhonology(justLetters.Value))
                        {
                            needFixing = true;
                            normalizedTokens.Add(@"""" + token.Trim()  + @"""");
                        }
                        else
                        {
                            normalizedTokens.Add(token);
                        }
                    }
                }
                if (needFixing)
                {
                    normalized = string.Join(" ", normalizedTokens);
                }
            }
            return normalized;
        }

        private static string DetectEntireForeignSentence(string sentence)
        {
            decimal percentTokipona = PercentTokiPona(sentence);
            if (percentTokipona < 0.20m)
            {
                if (sentence.Contains(" "))
                {
                    sentence = sentence.Replace(" ", "*");
                }
                if (!sentence.StartsWith(@""""))
                {
                    sentence = @"""" + sentence;
                }
                if (!sentence.EndsWith(@""""))
                {
                    sentence = sentence + @"""";
                }
                return sentence;
            }
            else
            {
                return sentence;
            }

            //if 25% or less tp, this is mostly foreign text.

            //if 75% or more tp, this is tp text with errors.

            //return "";
        }

        public static decimal PercentTokiPona(string sentence)
        {
            if (string.IsNullOrWhiteSpace(sentence)) return 1; //Blank is fine!

            TokenParserUtils pu = new TokenParserUtils();
            Token[]  tokens= pu.ValidTokens(sentence);
            Word w =new Word();
            int bad = 0;
            foreach (Token token in tokens)
            {
                string unpunctuated =
                        token.Text.Trim(new char[]
                        {
                            '«', '»', '@', '.', '!', ' ', '?', '!', '\n', '\r', ';', ':', '<', '>', '/', '&', '$', '\'','(',')'
                            '"'
                        });

                string[] errors= w.ValidateOnConstruction(unpunctuated, false);
                if (errors.Length > 0)
                {
                    bad++;
                }
            }
            return (((decimal)tokens.Length - (decimal)bad) / (decimal)tokens.Length);
        }
    }
}
