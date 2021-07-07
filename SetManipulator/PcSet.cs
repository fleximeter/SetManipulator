/* File: PcSet.cs
** Project: MusicTheory
** Author: Jeffrey Martin
**
** This file contains the implementation of the PcSet class.
**
** Copyright © 2021 by Jeffrey Martin. All rights reserved.
** Email: jmartin@jeffreymartincomposer.com
** Website: https://jeffreymartincomposer.com
**
******************************************************************************
**
** This program is free software: you can redistribute it and/or modify
** it under the terms of the GNU General Public License as published by
** the Free Software Foundation, either version 3 of the License, or
** (at your option) any later version.
**
** This program is distributed in the hope that it will be useful,
** but WITHOUT ANY WARRANTY; without even the implied warranty of
** MERCHANTABILITY or FITNESS FOR A PARTICULAR PURPOSE.  See the
** GNU General Public License for more details.
**
** You should have received a copy of the GNU General Public License
** along with this program.  If not, see <https://www.gnu.org/licenses/>.
*/

using System;
using System.Collections.Generic;

namespace MusicTheory
{
    /// <summary>
    /// Functions for working with pitch-class sets
    /// </summary>
    public static class PcSet
    {
        /// <summary>
        /// Copies a pcset
        /// </summary>
        /// <param name="set">A pcset</param>
        /// <returns>A copy of the pcset</returns>
        public static HashSet<PitchClass> Copy(HashSet<PitchClass> set)
        {
            HashSet<PitchClass> copy = new HashSet<PitchClass>();
            foreach (PitchClass pc in set)
                copy.Add(new PitchClass(pc));
            return copy;
        }

        /// <summary>
        /// Gets a collection of secondary forms that contain a provided collection of pitches
        /// </summary>
        /// <param name="pitchClasses">The pitch classes to search for</param>
        /// <returns>The secondary forms</returns>
        public static List<Pair<string, HashSet<PitchClass>>> GetSecondaryForms(HashSet<PitchClass> pcset, List<PitchClass> pitchClasses)
        {
            List<Pair<string, HashSet<PitchClass>>> secondaryForms = new List<Pair<string, HashSet<PitchClass>>>();

            if (pitchClasses.Count <= pcset.Count)
            {
                for (int i = 0; i < 12; i++)
                {
                    HashSet<PitchClass> transpose = Transpose(pcset, i);
                    HashSet<PitchClass> invert = Transpose(Invert(pcset), i);
                    bool isTranspose = true;
                    bool isInvert = true;

                    foreach (PitchClass pc in pitchClasses)
                    {
                        if (!transpose.Contains(pc))
                        {
                            isTranspose = false;
                            break;
                        }
                    }
                    foreach (PitchClass pc in pitchClasses)
                    {
                        if (!invert.Contains(pc))
                        {
                            isInvert = false;
                            break;
                        }
                    }

                    if (isTranspose)
                        secondaryForms.Add(new Pair<string, HashSet<PitchClass>>('T' + i.ToString(), transpose));
                    if (isInvert)
                        secondaryForms.Add(new Pair<string, HashSet<PitchClass>>('T' + i.ToString() + 'I', invert));
                }
            }

            else
                throw new System.ArgumentOutOfRangeException("The provided list of pitch classes is too big.");

            return secondaryForms;
        }

        /// <summary>
        /// Intersects two PcSets
        /// </summary>
        /// <param name="set1">A PcSet</param>
        /// <param name="set2">A PcSet</param>
        /// <returns>A new, intersected pcset</returns>
        public static HashSet<PitchClass> Intersect(HashSet<PitchClass> set1, HashSet<PitchClass> set2)
        {
            HashSet<PitchClass> pcSet = Copy(set1);
            pcSet.IntersectWith(set2);
            return pcSet;
        }

        /// <summary>
        /// Produces the maximum intersection of two pcsets
        /// </summary>
        /// <param name="set1">A pcset</param>
        /// <param name="set2">A pcset</param>
        /// <returns>The maximum intersection</returns>
        public static HashSet<PitchClass> IntersectMax(HashSet<PitchClass> set1, HashSet<PitchClass> set2)
        {
            HashSet<PitchClass> intersect = new HashSet<PitchClass>();
            int max = 0;
            for (int i = 0; i < 12; i++)
            {
                HashSet<PitchClass> t = Transpose(set2, i);
                HashSet<PitchClass> inv = Transpose(Invert(set2), i);
                HashSet<PitchClass> i1 = Intersect(set1, t);
                HashSet<PitchClass> i2 = Intersect(set1, inv);
                if (i1.Count > max)
                {
                    max = i1.Count;
                    intersect = i1;
                }
                if (i2.Count > max)
                {
                    max = i2.Count;
                    intersect = i2;
                }
            }
            return intersect;
        }

        /// <summary>
        /// Inverts a set of pitch classes. This is a TTO.
        /// </summary>
        /// <param name="setToInvert">The set to invert</param>
        /// <returns>The inverted set</returns>
        public static HashSet<PitchClass> Invert(HashSet<PitchClass> setToInvert)
        {
            HashSet<PitchClass> inverted = new HashSet<PitchClass>();
            foreach (PitchClass pc in setToInvert)
                inverted.Add(new PitchClass(pc.PitchClassInteger * 11));
            return inverted;
        }

        /// <summary>
        /// Validates a transformation name. Valid transformations include Tx and TxI.
        /// </summary>
        /// <param name="transformationName">The transformation name</param>
        /// <returns>True if the name is valid; false otherwise</returns>
        public static bool IsValidTransformation(string transformationName)
        {
            char[] VALID_OPS = new char[3] { 'T', 'I', 'M' };
            char[] VALID_CHARS = new char[6] { 'T', 'R', 'I', 'r', 'M', '-' };
            if (transformationName.Length == 0)
                return false;
            if (System.Array.Find(VALID_OPS, (a) => a == transformationName[0]) == -1)
                return false;
            for (int i = 0; i < transformationName.Length; i++)
            {
                char c = transformationName[i];

                // T and M transformations must be followed by a number
                if (c == 'T' || c == 'M')
                {
                    if (i + 1 < transformationName.Length)
                    {
                        if (i + 2 < transformationName.Length)
                        {
                            if (transformationName[i + 1] == '-' && (transformationName[i + 2] < '0' || transformationName[i + 2] > '9'))
                                return false;
                        }
                        else if (transformationName[i + 1] < '0' || transformationName[i + 1] > '9')
                            return false;
                    }
                    else
                        return false;
                }

                // I transformations must not be followed by a number
                else if (c == 'I')
                {
                    if (i + 1 < transformationName.Length)
                    {
                        if (System.Array.Find(VALID_OPS, (a) => a == transformationName[i + 1]) == -1)
                            return false;
                    }
                }

                // If the current character is not a number or a valid op, the transformation string is invalid
                else if (System.Array.Find(VALID_CHARS, (a) => a == c) == -1 && c < '0' && c > '9')
                    return false;
            }
            return true;
        }

        /// <summary>
        /// Multiplies a pitch class set. This is a TTO if the multiplier is 1, 5, 7, or 11 (mod 12).
        /// </summary>
        /// <param name="setToMultiply">The set to multiply</param>
        /// <param name="multiplier">The multiplier (by default, 5). Note that multipliers other than 1, 5, 7, or 11 
        /// (mod 12) result in epimorphisms.</param>
        /// <returns>The multiplied set</returns>
        public static HashSet<PitchClass> Multiply(HashSet<PitchClass> setToMultiply, int multiplier = 5)
        {
            HashSet<PitchClass> multiplied = new HashSet<PitchClass>();
            foreach (PitchClass pc in setToMultiply)
                multiplied.Add(new PitchClass(pc.PitchClassInteger * multiplier));
            return multiplied;
        }

        /// <summary>
        /// Gets all proper subsets of the current pcset. These are literal subsets.
        /// </summary>
        /// <param name="pcset">A pcset</param>
        /// <returns>A list of all proper subsets, excluding the null set</returns>
        public static List<HashSet<PitchClass>> Subsets(HashSet<PitchClass> pcset)
        {
            List<HashSet<PitchClass>> store = new List<HashSet<PitchClass>>();
            HashSet<PitchClass> build = new HashSet<PitchClass>();
            List<PitchClass> remaining = ToSortedPcSeg(pcset);
            
            // Add the null set
            store.Add(new HashSet<PitchClass>());

            for (int i = 1; i < pcset.Count; i++)
                SubsetsHelper(store, build, remaining, 0, i);

            return store;
        }

        /// <summary>
        /// A helper for Subsets
        /// </summary>
        /// <param name="store">The list to store completed subsets</param>
        /// <param name="build">The current subset we are building</param>
        /// <param name="remaining">The remaining pitch classes from which to choose</param>
        /// <param name="selected">The number of pitch classes already chosen</param>
        /// <param name="max">The maximum number of pitch classes to choose</param>
        private static void SubsetsHelper(List<HashSet<PitchClass>> store, HashSet<PitchClass> build, List<PitchClass> remaining, int selected, int max)
        {
            if (selected == max)
                store.Add(build);
            else
            {
                for (int i = 0; i <= remaining.Count - max + selected; i++)
                {
                    HashSet<PitchClass> newBuild = new HashSet<PitchClass>(build);
                    List<PitchClass> newRemaining = new List<PitchClass>(remaining);
                    newBuild.Add(new PitchClass(newRemaining[i]));
                    newRemaining.RemoveRange(0, i + 1);
                    SubsetsHelper(store, newBuild, newRemaining, selected + 1, max);
                }
            }
        }

        /// <summary>
        /// Converts a pitch-class HashSet to a List
        /// </summary>
        /// <param name="set">The HashSet to convert</param>
        /// <returns>A List</returns>
        public static List<PitchClass> ToPcSeg(HashSet<PitchClass> set)
        {
            List<PitchClass> newList = new List<PitchClass>();
            foreach (PitchClass pc in set)
                newList.Add(new PitchClass(pc));
            return newList;
        }

        /// <summary>
        /// Converts a pitch-class HashSet to a sorted List
        /// </summary>
        /// <param name="set">The HashSet to convert</param>
        /// <returns>A sorted List</returns>
        public static List<PitchClass> ToSortedPcSeg(HashSet<PitchClass> set)
        {
            List<PitchClass> newList = new List<PitchClass>();
            foreach (PitchClass pc in set)
                newList.Add(new PitchClass(pc));
            newList.Sort((a, b) => a.PitchClassInteger.CompareTo(b.PitchClassInteger));
            return newList;
        }

        /// <summary>
        /// Converts a pitch-class set to a sorted string
        /// </summary>
        /// <param name="set">The set</param>
        /// <returns>A string</returns>
        public static string ToString(HashSet<PitchClass> set, string separator = "")
        {
            List<PitchClass> sorted = ToSortedPcSeg(set);
            System.Text.StringBuilder sb = new System.Text.StringBuilder();
            for (int i = 0; i < sorted.Count - 1; i++)
                sb.Append(sorted[i].PitchClassChar + separator);
            if (set.Count > 0)
                sb.Append(sorted[sorted.Count - 1].PitchClassChar);
            return sb.ToString();
        }

        /// <summary>
        /// Transposes a set of pitch classes. This is a TTO.
        /// </summary>
        /// <param name="setToTranspose">The set to transpose</param>
        /// <param name="numberOfTranspositions">The number of transpositions</param>
        /// <returns>The transposed set</returns>
        public static HashSet<PitchClass> Transpose(HashSet<PitchClass> setToTranspose, int numberOfTranspositions)
        {
            HashSet<PitchClass> transposedSet = new HashSet<PitchClass>();
            foreach (PitchClass pc in setToTranspose)
                transposedSet.Add(new PitchClass(pc.PitchClassInteger + numberOfTranspositions));
            return transposedSet;
        }

        /// <summary>
        /// Transforms the pitch class set. Note that this function will only behave as expected 
        /// if valid transformation names are provided - names can be validated using IsValidTransformation().
        /// </summary>
        /// <param name="transformationName">The transformation name</param>
        /// <returns>A transformed version of the set</returns>
        public static HashSet<PitchClass> Transform(HashSet<PitchClass> pcset, string transformationString)
        {
            List<Pair<char, int>> opList = new List<Pair<char, int>>();
            HashSet<PitchClass> pcList = Copy(pcset);
            int numCount = 0;
            char[] VALID_OPS = new char[3] { 'T', 'I', 'M' };
            opList.Add(new Pair<char, int>('0', 0));

            // Parse the transformation string and separate it into opcodes and numbers
            for (int i = transformationString.Length - 1; i >= 0; i--)
            {
                char c = transformationString[i];
                if (c >= '0' && c <= '9')
                {
                    opList[opList.Count - 1].Item2 += (int)System.Math.Pow(10, numCount) * (c - '0');
                    numCount++;
                }
                else if (c == '-' && i > 0)
                    opList[opList.Count - 1].Item2 *= -1;
                else
                {
                    if (System.Array.Find(VALID_OPS, (a) => a == c) != -1)
                    {
                        opList[opList.Count - 1].Item1 = c;
                        if (i > 0)
                            opList.Add(new Pair<char, int>('0', 0));
                    }
                    numCount = 0;
                }
            }

            // Perform the transformation
            foreach (Pair<char, int> pair in opList)
            {
                if (pair.Item1 == 'T')
                    pcList = Transpose(pcList, pair.Item2);
                else if (pair.Item1 == 'I')
                    pcList = Invert(pcList);
                else if (pair.Item1 == 'M')
                    pcList = Multiply(pcList, pair.Item2);
            }

            return pcList;
        }

        /// <summary>
        /// Unions two PcSets
        /// </summary>
        /// <param name="set1">A PcSet</param>
        /// <param name="set2">A PcSet</param>
        /// <returns>A new, unioned PcSet</returns>
        public static HashSet<PitchClass> Union(HashSet<PitchClass> set1, HashSet<PitchClass> set2)
        {
            HashSet<PitchClass> pcSet = Copy(set1);
            pcSet.UnionWith(set2);
            return pcSet;
        }

        /// <summary>
        /// Produces the most compact union of two pcsets
        /// </summary>
        /// <param name="set1">A pcset</param>
        /// <param name="set2">A pcset</param>
        /// <returns>The most compact union</returns>
        public static HashSet<PitchClass> UnionCompact(HashSet<PitchClass> set1, HashSet<PitchClass> set2)
        {
            HashSet<PitchClass> union = new HashSet<PitchClass>();
            int min = set1.Count + set2.Count;
            for (int i = 0; i < 12; i++)
            {
                HashSet<PitchClass> t = Transpose(set2, i);
                HashSet<PitchClass> inv = Transpose(Invert(set2), i);
                HashSet<PitchClass> i1 = Union(set1, t);
                HashSet<PitchClass> i2 = Union(set1, inv);
                if (i1.Count < min)
                {
                    min = i1.Count;
                    union = i1;
                }
                if (i2.Count < min)
                {
                    min = i2.Count;
                    union = i2;
                }
            }
            return union;
        }
    }
}
