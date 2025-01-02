using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace ControlRigging
{
    public static class EnumeratorExtensions
    {
        public static IEnumerable<T> Recurse<T>(this T t, 
            bool includeRoot = true) 
            where T : IEnumerable
        {
            if(includeRoot)
                yield return t;
            
            foreach (T child in t.Recurse_Internal())
                yield return child;
        }
        
        static IEnumerable<T> Recurse_Internal<T>(this T t)
            where T : IEnumerable
        {
            foreach (T child in t)
            {
                yield return child;
                foreach (T child2 in child.Recurse_Internal())
                    yield return child2;
            }
        }
    }
}