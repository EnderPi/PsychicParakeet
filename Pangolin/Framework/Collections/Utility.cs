using System;
using System.Collections.Generic;
using System.Text;

namespace EnderPi.Framework.Collections
{
    public static class Utility
    {
        /// <summary>
        /// Counts  the number of elements that appear more than once.  Assumes input array is sorted.
        /// </summary>
        /// <remarks>
        /// The set 1, 2, 2, 2, 2, 3 will return a 1, because only one element (2) appears more than once.
        /// </remarks>
        /// <param name="array">input array, assumed to be sorted.</param>
        /// <returns></returns>
        public static int CountNumberOfDuplicatedElements(ulong[] array)
        {
            int k = 0;
            int duplicates = 0;
            UInt64 current;
            while (k < array.Length - 1)
            {
                current = array[k];
                if (array[k] == array[k + 1])
                {
                    duplicates++;
                }
                while (k < array.Length && array[k] == current)
                {
                    k++;
                }
            }
            return duplicates;
        }
    }
}
