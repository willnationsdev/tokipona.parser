﻿using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using System.Xml;

namespace BasicTypes.Extensions
{
    public static class StringExtensions
    {
        [DebuggerStepThrough]
        public static bool TpLettersEqual(this string value, string comparsion)
        {
            StringBuilder sbValue = new StringBuilder(value.Length);
            StringBuilder sbComp = new StringBuilder(comparsion.Length);
            GetValue(value, sbValue);
            GetValue(comparsion, sbComp);
            return string.Equals(sbValue.ToString(), sbComp.ToString(), StringComparison.OrdinalIgnoreCase);
        }

        private static void GetValue(string comparsion, StringBuilder sb)
        {
            foreach (char c in comparsion)
            {
                if (Token.AlphabetEitherCase.Contains(c))
                {
                    sb.Append(c);
                }
            }
        }

        [DebuggerStepThrough]
        public static bool ContainsCheck(this string value, char middle)
        {
            //http://cc.davelozinski.com/c-sharp/fastest-way-to-check-if-a-string-occurs-within-a-string
            if (((value.Length - value.Replace(middle.ToString(), String.Empty).Length) / 1) > 0)
            {
                return true;
            }
            return false;
        
        }
        [DebuggerStepThrough]
        public static bool ContainsCheck(this string value, string middle, string orOther, string orThird)
        {
            return value.ContainsCheck(middle) || value.ContainsCheck(orOther) || value.ContainsCheck(orThird);
        }

        [DebuggerStepThrough]
        public static bool ContainsCheck(this string value, string middle, string orOther)
        {
            return value.ContainsCheck(middle) || value.ContainsCheck(orOther);
        }

        [DebuggerStepThrough]
        public static bool ContainsCheck(this string value, string middle)
        {
            //http://cc.davelozinski.com/c-sharp/fastest-way-to-check-if-a-string-occurs-within-a-string
            if(((value.Length - value.Replace(middle, String.Empty).Length) / middle.Length)>0)
            {
                return true;
            }
            return false;

        }

        [DebuggerStepThrough]
        public static bool StartCheck(this string value, string leadToken, string alternate)
        {
            return value.StartCheck(leadToken) || value.StartCheck(alternate);
        }

        [DebuggerStepThrough]
        public static bool StartCheck(this string value, string leadToken)
        {
            //Perf trace said this was surprisingly slow.

            //https://stackoverflow.com/questions/484796/more-efficient-way-to-determine-if-a-string-starts-with-a-token-from-a-set-of-to
            return value.StartsWith(leadToken, StringComparison.Ordinal);
        }

        [DebuggerStepThrough]
        public static bool EndCheck(this string value, string endToken, string endTokenOther)
        {
            return value.EndCheck(endToken) || value.EndCheck(endTokenOther);
        }

        [DebuggerStepThrough]
        public static bool EndCheck(this string value, string endToken, string endTokenOther, string endTokenYetAnother)
        {
            return value.EndCheck(endToken) || value.EndCheck(endTokenOther) || value.EndCheck(endTokenYetAnother);
        }

        [DebuggerStepThrough]
        public static bool EndCheck(this string value, string endToken)
        {
            //Perf trace said this was surprisingly slow.

            //https://stackoverflow.com/questions/484796/more-efficient-way-to-determine-if-a-string-starts-with-a-token-from-a-set-of-to
            return value.EndsWith(endToken, StringComparison.Ordinal);
        }

        [DebuggerStepThrough]
        public static string Remove(this string value, string letters)
        {
            StringBuilder sb = new StringBuilder(value.Length);
            foreach (char c in value)
            {
                if(letters.ContainsCheck(c) ) continue;
                sb.Append(c);
            }
            return sb.ToString();
        }

        [DebuggerStepThrough]
        public static bool ContainsWholeWord(this string value, string word)
        {
            return Regex.IsMatch(value,@"\b" + word + @"\b");
        }

        [DebuggerStepThrough]
        public static bool ContainsLetter(this string value, string letters)
        {
            return value != null && letters.Any(value.Contains);
        }

        [DebuggerStepThrough]
        public static bool ContainsOnlyAtoZLetters(this string value)
        {
            return value != null && value.ToLower().All(x=>"abcdefghijklmnopqrstuvwxyz".Contains(x));
        }
        [DebuggerStepThrough]
        public static bool ContainsOnlyPunctuation(this string value)
        {
            return value != null && value.ToLower().All(x => "!@#$%^&*()_+-={}[]|:,<>,/~`".Contains(x));
        }

        /// <summary>
        /// This is for normalizing arbitrary text, so it is a very broad definition of what a number is.
        /// It would include dates, 1/1/1900, negatives, enumeration titles, 1. milk 2. bread, explicit numbers #23 and more. 
        /// </summary>
        [DebuggerStepThrough]
        public static bool ContainsOnlyDigitsAndNumberLikeCruft(this string value)
        {
            return value != null && value.ToLower().All(x => ",/#-.1234567890".Contains(x));
        }
        [DebuggerStepThrough]
        public static bool IsFirstUpperCased(this string value)
        {
            if (string.IsNullOrWhiteSpace(value)) return false;
            if (value.Length == 0) return false;
            string toTest = value.Trim(new[] {'~', '#','\''});
            if (toTest.Length == 0) return false;
            return toTest[0].ToString().ToUpperInvariant() == value[0].ToString();
        }
        [DebuggerStepThrough]
        public static string RemoveLeadingWholeWord(this string value, string word)
        {
            if (!value.ContainsCheck(word)) return value;//short circuit

            //TODO: Maybe use regex instead?
            foreach (var white in new String[]{" ","\n"})
            {
                if (value.ContainsCheck(white))
                {
                    string wordWithWhite = word + white;
                    value = value.StartCheck(wordWithWhite) ? value.Substring(wordWithWhite.Length) : value;    
                }
            }
            return value;
        }
        [DebuggerStepThrough]
        public static bool StartsOrContainsOrEnds(this string value,string target)
        {
            if (value == target)
            {
                return true;
            }
            foreach (var white in new string[] {" ","\n" })
            {
                if (value.StartCheck(target + white))
                {
                    return true;
                }
                if (value.EndCheck(white + target))
                {
                    return true;
                }
                if (value.ContainsCheck(white + target + white))
                {
                    return true;
                }    
            }
            
            return false;
        }

        //http://stackoverflow.com/questions/2961656/generic-tryparse
        [DebuggerStepThrough]
        public static T Convert<T>(this string input)
        {
            var converter = TypeDescriptor.GetConverter(typeof(T));
            if (converter != null)
            {
                //Cast ConvertFromString(string text) : object to (T)
                return (T)converter.ConvertFromString(input);
            }
            return default(T);
        }

        [DebuggerStepThrough]
        public static String PrintXml(this String xml)
        {
            String result = "";

            MemoryStream mStream = new MemoryStream();
            XmlTextWriter writer = new XmlTextWriter(mStream, Encoding.Unicode);
            XmlDocument document = new XmlDocument();

            try
            {
                // Load the XmlDocument with the XML.
                document.LoadXml(xml);

                writer.Formatting = Formatting.Indented;

                // Write the XML into a formatting XmlTextWriter
                document.WriteContentTo(writer);
                writer.Flush();
                mStream.Flush();

                // Have to rewind the MemoryStream in order to read
                // its contents.
                mStream.Position = 0;

                // Read MemoryStream contents into a StreamReader.
                StreamReader sReader = new StreamReader(mStream);

                // Extract the text from the StreamReader.
                String formattedXml = sReader.ReadToEnd();

                result = formattedXml;
            }
            catch (XmlException)
            {
            }

            mStream.Close();
            writer.Close();

            return result;
        }

    }
}
