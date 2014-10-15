﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using BasicTypes.Extensions;

namespace BasicTypes
{
    //Forgotten interfaces
    public interface IToString
    {
        string[] SupportedFormats { get; }
        string ToString(string format);
    }

    //[DebuggerDisplay("{DebuggerDisplay,nq}")]
    public interface IDebuggerDisplay
    {
        string DebuggerDisplay { get; }
    }

    public interface ICopySelf<out T> : ICloneable
        where T : class
    {
        T ShallowCopy(); //Utility for avoiding copying large trees.
        T DeepCopy(); //Make an equality by value test pass.
    }

    

    public interface IParse<T>
    {
        T Parse(string value);
        bool TryParse(string value, out T result);
    }


    //Hmm, extension methods work better unless it simplifies working wiht proxy classes
   // public interface ICanSerialize<TProxy>
   // {
   //     string ToXml(TProxy value);
   //     string FromXml(TProxy value);
   // }

}