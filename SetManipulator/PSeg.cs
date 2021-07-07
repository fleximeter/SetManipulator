/* File: PSeg.cs
** Project: MusicTheory
** Author: Jeffrey Martin
**
** This file contains the implementation of the PSeg class.
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
    public static class PSeg
    {
        /// <summary>
        /// Copies a list
        /// </summary>
        /// <param name="pseg">A list</param>
        /// <returns>A copy of the list</returns>
        private static List<Pitch> CopyList(List<Pitch> pseg)
        {
            List<Pitch> list = new List<Pitch>();
            foreach (Pitch p in pseg)
                list.Add(new Pitch(p));
            return list;
        }

        /// <summary>
        /// Gets the interval string of a pcseg
        /// </summary>
        /// <param name="pseg">A pcseg</param>
        /// <returns>The interval string</returns>
        public static List<Pitch> Intervals(List<Pitch> pseg)
        {
            List<Pitch> list = new List<Pitch>();
            for (int i = 1; i < pseg.Count; i++)
                list.Add(pseg[i] - pseg[i - 1]);
            return list;
        }

        /// <summary>
        /// Validates a p-string
        /// </summary>
        /// <param name="pstring">A p-string</param>
        /// <returns>True if the p-string is valid; false otherwise</returns>
        public static bool IsValidPString(string pstring)
        {
            string[] ps;
            int a;
            string split;
            if (pstring.Contains(", "))
                split = ", ";
            else if (pstring.Contains(" "))
                split = " ";
            else
                split = ",";
            ps = pstring.Split(split);
            foreach (string p in ps)
            {
                if (p.Length == 0)
                    return false;
                if (!int.TryParse(p, out a))
                    return false;
            }
            return true;
        }

        /// <summary>
        /// Inverts the PSeg and returns a new PSeg
        /// </summary>
        /// <param name="pseg">A PSeg</param>
        /// <returns>An inverted form of the PSeg</returns>
        public static List<Pitch> Invert(List<Pitch> pseg)
        {
            List<Pitch> list = new List<Pitch>();
            foreach (Pitch p in pseg)
                list.Add(new Pitch(p.PitchNum * -1));
            return list;
        }

        /// <summary>
        /// Validates potential transformation names
        /// </summary>
        /// <param name="transformationName">The transformation name</param>
        /// <returns>True if the name is valid; false otherwise</returns>
        public static bool IsValidTransformation(string transformationName)
        {
            char[] VALID_OPS = new char[5] { 'T', 'R', 'I', 'r', 'M' };
            char[] VALID_CHARS = new char[6] { 'T', 'R', 'I', 'r', 'M', '-' };
            if (transformationName.Length == 0)
                return false;
            if (System.Array.Find(VALID_OPS, (a) => a == transformationName[0]) == -1)
                return false;
            for (int i = 0; i < transformationName.Length; i++)
            {
                char c = transformationName[i];

                // T, r, and M transformations must be followed by a number
                if (c == 'T' || c == 'M' || c == 'r')
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

                // R and I transformations must not be followed by a number
                else if (c == 'R' || c == 'I')
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
        /// Multiplies the PSeg and returns a new PSeg
        /// </summary>
        /// <param name="pseg">A PSeg</param>
        /// <param name="num">The multiplier</param>
        /// <returns>A multiplied form of the PSeg</returns>
        public static List<Pitch> Multiply(List<Pitch> pseg, int num)
        {
            List<Pitch> list = new List<Pitch>();
            foreach (Pitch p in pseg)
                list.Add(new Pitch(p.PitchNum * num));
            return list;
        }

        /// <summary>
        /// Generates a pitch list from a provided string
        /// </summary>
        /// <param name="pitches">A string of pitches</param>
        /// <returns>A pitch list</returns>
        public static List<Pitch> Parse(string pitches)
        {
            List<Pitch> list = new List<Pitch>();
            string[] plist = pitches.Split(',');
            bool invalid = false;
            foreach (string p in plist)
            {
                int num = 0;
                int pow = 0;
                for (int i = p.Length - 1; i >= 0; i--)
                {
                    if (p[i] >= '0' && p[i] <= '9')
                    {
                        num += (p[i] - '0') * (int)System.Math.Pow(10, pow);
                        pow++;
                        if (i == 0)
                            list.Add(new Pitch(num));
                    }
                    else if (p[i] == '-' && i == 0)
                    {
                        num *= -1;
                        list.Add(new Pitch(num));
                    }
                    else
                    {
                        invalid = true;
                        break;
                    }
                }
                if (invalid)
                {
                    list.Clear();
                    break;
                }
            }
            return list;
        }

        /// <summary>
        /// Retrogrades a PSeg
        /// </summary>
        /// <param name="pseg">A pseg</param>
        /// <returns>A retrograded pseg</returns>
        public static List<Pitch> Retrograde(List<Pitch> pseg)
        {
            List<Pitch> list = CopyList(pseg);
            list.Reverse();
            return list;
        }

        /// <summary>
        /// Rotates a PSeg
        /// </summary>
        /// <param name="i">The index of rotation</param>
        public static List<Pitch> Rotate(List<Pitch> pseg, int i)
        {
            List<Pitch> newList = new List<Pitch>(pseg.Capacity);
            i %= 12;
            if (i < 0)
                i += 12;
            for (int j = 0; j < pseg.Count; j++)
                newList.Add(pseg[(j + i) % pseg.Count]);
            return newList;
        }

        /// <summary>
        /// Sorts a pc list
        /// </summary>
        /// <param name="pseg">The list</param>
        /// <returns>A sorted copy of the list</returns>
        public static List<Pitch> Sort(List<Pitch> pseg)
        {
            List<Pitch> newList = new List<Pitch>();
            foreach (Pitch p in pseg)
                newList.Add(new Pitch(p));
            newList.Sort((a, b) => a.PitchNum.CompareTo(b.PitchNum));
            return newList;
        }

        /// <summary>
        /// Gets all subsegs of a pseg
        /// </summary>
        /// <param name="pcseg">A pseg</param>
        /// <returns>The subsets of the pseg</returns>
        public static List<List<Pitch>> Subsegs(List<Pitch> pcseg)
        {
            List<List<Pitch>> store = new List<List<Pitch>>();
            List<Pitch> build = new List<Pitch>();
            List<Pitch> remaining = new List<Pitch>(pcseg);

            for (int i = 1; i < pcseg.Count; i++)
                SubsegsHelper(store, build, remaining, 0, i);

            return store;
        }

        /// <summary>
        /// A helper for Subsegs
        /// </summary>
        /// <param name="store">The list to store completed subsegs</param>
        /// <param name="build">The current subseg we are building</param>
        /// <param name="remaining">The remaining pitches from which to choose</param>
        /// <param name="selected">The number of pitches already chosen</param>
        /// <param name="max">The maximum number of pitches to choose</param>
        private static void SubsegsHelper(List<List<Pitch>> store, List<Pitch> build, List<Pitch> remaining, int selected, int max)
        {
            if (selected == max)
                store.Add(build);
            else
            {
                for (int i = 0; i <= remaining.Count - max + selected; i++)
                {
                    List<Pitch> newBuild = new List<Pitch>(build);
                    List<Pitch> newRemaining = new List<Pitch>(remaining);
                    newBuild.Add(new Pitch(newRemaining[i]));
                    newRemaining.RemoveRange(0, i + 1);
                    SubsegsHelper(store, newBuild, newRemaining, selected + 1, max);
                }
            }
        }

        /// <summary>
        /// Converts a pitch list to a HashSet
        /// </summary>
        /// <param name="list">The list</param>
        /// <returns>A HashSet</returns>
        public static HashSet<Pitch> ToPSet(List<Pitch> list)
        {
            HashSet<Pitch> set = new HashSet<Pitch>();
            foreach (Pitch p in list)
                set.Add(new Pitch(p));
            return set;
        }

        /// <summary>
        /// Converts a pitch list to a string
        /// </summary>
        /// <param name="list">The pitchlist</param>
        /// <returns>A string</returns>
        public static string ToString(List<Pitch> list, string separator = ",")
        {
            System.Text.StringBuilder sb = new System.Text.StringBuilder();
            for (int i = 0; i < list.Count - 1; i++)
                sb.Append(list[i].PitchNum.ToString() + separator);
            if (list.Count > 0)
                sb.Append(list[list.Count - 1].PitchNum.ToString());
            return sb.ToString();
        }

        /// <summary>
        /// Transforms the pseg. Note that this function will only behave as expected 
        /// if valid transformation names are provided - names can be validated using IsValidTransformation().
        /// </summary>
        /// <param name="transformationString">The transformation name</param>
        /// <returns>A transformed version of the pseg</returns>
        public static List<Pitch> Transform(List<Pitch> pseg, string transformationString)
        {
            List<Pair<char, int>> opList = new List<Pair<char, int>>();
            List<Pitch> pList = CopyList(pseg);
            int numCount = 0;
            char[] VALID_OPS = new char[5] { 'T', 'I', 'M', 'R', 'r' };
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
                    pList = Transpose(pList, pair.Item2);
                else if (pair.Item1 == 'I')
                    pList = Invert(pList);
                else if (pair.Item1 == 'M')
                    pList = Multiply(pList, pair.Item2);
                else if (pair.Item1 == 'R')
                    pList = Retrograde(pList);
                else if (pair.Item1 == 'r')
                    pList = Rotate(pList, pair.Item2);
            }

            return pList;
        }

        /// <summary>
        /// Transposes the PSeg and returns a new PSeg
        /// </summary>
        /// <param name="pseg">A PSeg</param>
        /// <param name="num">The transposition index</param>
        /// <returns>A transposed form of the PSeg</returns>
        public static List<Pitch> Transpose(List<Pitch> pseg, int num)
        {
            List<Pitch> list = new List<Pitch>();
            foreach (Pitch p in pseg)
                list.Add(new Pitch(p.PitchNum + num));
            return list;
        }
    }
}
