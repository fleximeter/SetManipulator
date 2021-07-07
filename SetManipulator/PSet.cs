/* File: PSet.cs
** Project: MusicTheory
** Author: Jeffrey Martin
**
** This file contains the implementation of the PSet class.
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

using System.Collections.Generic;

namespace MusicTheory
{
    /// <summary>
    /// Represents a pitch set
    /// </summary>
    public static class PSet
    {
        /// <summary>
        /// Copies a pset
        /// </summary>
        /// <param name="pset">A pset</param>
        /// <returns>A copy of the pset</returns>
        public static HashSet<Pitch> Copy(HashSet<Pitch> pset)
        {
            HashSet<Pitch> copy = new HashSet<Pitch>();
            foreach (Pitch p in pset)
                copy.Add(new Pitch(p));
            return copy;
        }

        /// <summary>
        /// The IC matrix of the PSet
        /// </summary>
        /// <param name="pSet">A PSet</param>
        /// <returns>An IC matrix</returns>
        public static int[,] GetICMatrix(HashSet<Pitch> pSet)
        {
            List<Pitch> sorted = ToSortedPSeg(pSet);
            int[,] matrix = new int[pSet.Count, pSet.Count];
            for (int i = 0; i < pSet.Count; i++)
            {
                for (int j = 0; j < pSet.Count; j++)
                    matrix[i, j] = System.Math.Abs(sorted[j].PitchNum - sorted[i].PitchNum);
            }
            return matrix;
        }

        /// <summary>
        /// Intersects this PSet and another PSet and returns a new PSet
        /// </summary>
        /// <param name="pSet1">A PSet</param>
        /// <param name="pSet2">A PSet</param>
        /// <returns>A new PSet</returns>
        public static HashSet<Pitch> Intersect(HashSet<Pitch> pSet1, HashSet<Pitch> pSet2)
        {
            HashSet<Pitch> set = Copy(pSet1);
            set.IntersectWith(pSet2);
            return set;
        }

        /// <summary>
        /// Inverts the PSet and returns a new PSet
        /// </summary>
        /// <param name="pset">A pset</param>
        /// <returns>An inverted form of the PSet</returns>
        public static HashSet<Pitch> Invert(HashSet<Pitch> pset)
        {
            HashSet<Pitch> newSet = new HashSet<Pitch>();
            foreach (Pitch p in pset)
                newSet.Add(new Pitch(p.PitchNum * -1));
            return newSet;
        }

        /// <summary>
        /// Determines if a list of pitches is a valid pset
        /// </summary>
        /// <param name="list">A list of pitches</param>
        /// <returns>True or false</returns>
        public static bool IsValidSet(List<Pitch> list)
        {
            HashSet<Pitch> set = new HashSet<Pitch>();
            foreach (Pitch p in list)
                set.Add(new Pitch(p));
            if (set.Count == list.Count)
                return true;
            else
                return false;
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
        /// Multiplies the PSet and returns a new PSet
        /// </summary>
        /// <param name="pset">A pset</param>
        /// <param name="num">The multiplier</param>
        /// <returns>A multiplied form of the PSet</returns>
        public static HashSet<Pitch> Multiply(HashSet<Pitch> pset, int num)
        {
            HashSet<Pitch> newSet = new HashSet<Pitch>();
            foreach (Pitch p in pset)
                newSet.Add(new Pitch(p.PitchNum * num));
            return newSet;
        }

        /// <summary>
        /// Gets all proper subsets of the current pset. These are literal subsets.
        /// </summary>
        /// <param name="pset">A pset</param>
        /// <returns>A list of all proper subsets, excluding the null set</returns>
        public static List<HashSet<Pitch>> Subsets(HashSet<Pitch> pset)
        {
            List<HashSet<Pitch>> store = new List<HashSet<Pitch>>();
            HashSet<Pitch> build = new HashSet<Pitch>();
            List<Pitch> remaining = ToSortedPSeg(pset);

            for (int i = 1; i < pset.Count; i++)
                SubsetsHelper(store, build, remaining, 0, i);

            return store;
        }

        /// <summary>
        /// A helper for Subsets
        /// </summary>
        /// <param name="store">The list to store completed subsets</param>
        /// <param name="build">The current subset we are building</param>
        /// <param name="remaining">The remaining pitches from which to choose</param>
        /// <param name="selected">The number of pitches already chosen</param>
        /// <param name="max">The maximum number of pitches to choose</param>
        private static void SubsetsHelper(List<HashSet<Pitch>> store, HashSet<Pitch> build, List<Pitch> remaining, int selected, int max)
        {
            if (selected == max)
                store.Add(build);
            else
            {
                for (int i = 0; i <= remaining.Count - max + selected; i++)
                {
                    HashSet<Pitch> newBuild = new HashSet<Pitch>(build);
                    List<Pitch> newRemaining = new List<Pitch>(remaining);
                    newBuild.Add(new Pitch(newRemaining[i]));
                    newRemaining.RemoveRange(0, i + 1);
                    SubsetsHelper(store, newBuild, newRemaining, selected + 1, max);
                }
            }
        }

        /// <summary>
        /// Converts a pitch HashSet to a List
        /// </summary>
        /// <param name="set">The HashSet to convert</param>
        /// <returns>A List</returns>
        public static List<Pitch> ToPSeg(HashSet<Pitch> set)
        {
            List<Pitch> newList = new List<Pitch>();
            foreach (Pitch p in set)
                newList.Add(new Pitch(p));
            return newList;
        }

        /// <summary>
        /// Converts a pitch HashSet to a sorted List
        /// </summary>
        /// <param name="set">The HashSet to convert</param>
        /// <returns>A sorted List</returns>
        public static List<Pitch> ToSortedPSeg(HashSet<Pitch> set)
        {
            List<Pitch> newList = new List<Pitch>();
            foreach (Pitch pc in set)
                newList.Add(new Pitch(pc));
            newList.Sort((a, b) => a.PitchNum.CompareTo(b.PitchNum));
            return newList;
        }

        /// <summary>
        /// Converts a pitch set to a sorted string
        /// </summary>
        /// <param name="set">The set</param>
        /// <returns>A string</returns>
        public static string ToString(HashSet<Pitch> set, string separator = ",")
        {
            List<Pitch> sorted = ToSortedPSeg(set);
            System.Text.StringBuilder sb = new System.Text.StringBuilder();
            for (int i = 0; i < sorted.Count - 1; i++)
                sb.Append(sorted[i].PitchNum.ToString() + separator);
            if (set.Count > 0)
                sb.Append(sorted[sorted.Count - 1].PitchNum.ToString());
            return sb.ToString();
        }

        /// <summary>
        /// Transforms the pitch class set. Note that this function will only behave as expected 
        /// if valid transformation names are provided - names can be validated using IsValidTransformation().
        /// </summary>
        /// <param name="transformationString">The transformation string</param>
        /// <returns>A transformed version of the set</returns>
        public static HashSet<Pitch> Transform(HashSet<Pitch> pset, string transformationString)
        {
            List<Pair<char, int>> opList = new List<Pair<char, int>>();
            HashSet<Pitch> set = Copy(pset);
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
                    set = Transpose(set, pair.Item2);
                else if (pair.Item1 == 'I')
                    set = Invert(set);
                else if (pair.Item1 == 'M')
                    set = Multiply(set, pair.Item2);
            }

            return set;
        }

        /// <summary>
        /// Transposes the PSet and returns a new PSet
        /// </summary>
        /// <param name="pset">A pset</param>
        /// <param name="num">The transposition index</param>
        /// <returns>A transposed form of the PSet</returns>
        public static HashSet<Pitch> Transpose(HashSet<Pitch> pset, int num)
        {
            HashSet<Pitch> newSet = new HashSet<Pitch>();
            foreach (Pitch p in pset)
                newSet.Add(new Pitch(p.PitchNum + num));
            return newSet;
        }

        /// <summary>
        /// Unions this PSet and another PSet and returns a new PSet
        /// </summary>
        /// <param name="pSet1">A PSet</param>
        /// <param name="pSet2">A PSet</param>
        /// <returns>A new PSet</returns>
        public static HashSet<Pitch> Union(HashSet<Pitch> pSet1, HashSet<Pitch> pSet2)
        {
            HashSet<Pitch> set = Copy(pSet1);
            set.UnionWith(pSet2);
            return set;
        }
    }
}
