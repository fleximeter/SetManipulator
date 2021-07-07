/* File: PcSeg.cs
** Project: MusicTheory
** Author: Jeffrey Martin
**
** This file contains the implementation of the PcSeg class.
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
    /// Contains functions for working with pcsegs
    /// </summary>
    public static class PcSeg
    {
        /// <summary>
        /// Gets the complement of a pcseg
        /// </summary>
        /// <param name="pcseg">The pcseg</param>
        /// <returns>The complement</returns>
        public static List<PitchClass> Complement(List<PitchClass> pcseg)
        {
            List<PitchClass> list = new List<PitchClass>();
            Dictionary<int, char> usedPitches = new Dictionary<int, char>();
            foreach (PitchClass pc in pcseg)
                usedPitches[pc.PitchClassInteger] = '0';
            for (int i = 0; i < 12; i++)
            {
                if (!usedPitches.ContainsKey(i))
                    list.Add(new PitchClass(i));
            }
            return list;
        }

        /// <summary>
        /// Copies a pitch class list
        /// </summary>
        /// <param name="list">A list</param>
        /// <returns>A copy of the list</returns>
        private static List<PitchClass> CopyList(List<PitchClass> list)
        {
            List<PitchClass> newList = new List<PitchClass>(list.Capacity);
            foreach (PitchClass pc in list)
                newList.Add(new PitchClass(pc));
            return newList;
        }

        /// <summary>
        /// Gets all secondary forms that contain the provided ordered sequence of pitch classes
        /// </summary>
        /// <param name="pitches">An ordered sequence of pitch classes</param>
        /// <returns>A collection of secondary forms</returns>
        public static List<Pair<string, List<PitchClass>>> GetSecondaryForms(List<PitchClass> pcseg, List<PitchClass> pitches)
        {
            List<Pair<string, List<PitchClass>>> secondaryForms = new List<Pair<string, List<PitchClass>>>();

            for (int i = 0; i < 12; i++)
            {
                List<PitchClass> transpose = Transpose(pcseg, i);
                bool isValid = true;
                for (int j = 0; j <= 12 - pitches.Count; j++)
                {
                    if (transpose[j].PitchClassInteger == pitches[0].PitchClassInteger)
                    {
                        for (int k = j + 1; k - j < pitches.Count; k++)
                        {
                            if (transpose[k].PitchClassInteger != pitches[k - j].PitchClassInteger)
                            {
                                isValid = false;
                                break;
                            }
                        }
                        if (isValid)
                            secondaryForms.Add(new Pair<string, List<PitchClass>>('T' + i.ToString(), transpose));
                    }
                    if (!isValid)
                        break;
                }
            }
            for (int i = 0; i < 12; i++)
            {
                List<PitchClass> invert = Transpose(Invert(pcseg), i);
                bool isValid = true;
                for (int j = 0; j <= 12 - pitches.Count; j++)
                {
                    if (invert[j].PitchClassInteger == pitches[0].PitchClassInteger)
                    {
                        for (int k = j + 1; k - j < pitches.Count; k++)
                        {
                            if (invert[k].PitchClassInteger != pitches[k - j].PitchClassInteger)
                            {
                                isValid = false;
                                break;
                            }
                        }
                        if (isValid)
                            secondaryForms.Add(new Pair<string, List<PitchClass>>('T' + i.ToString() + 'I', invert));
                    }
                    if (!isValid)
                        break;
                }
            }
            for (int i = 0; i < 12; i++)
            {
                List<PitchClass> retrograde = Transpose(Retrograde(pcseg), i);
                bool isValid = true;
                for (int j = 0; j <= 12 - pitches.Count; j++)
                {
                    if (retrograde[j].PitchClassInteger == pitches[0].PitchClassInteger)
                    {
                        for (int k = j + 1; k - j < pitches.Count; k++)
                        {
                            if (retrograde[k].PitchClassInteger != pitches[k - j].PitchClassInteger)
                            {
                                isValid = false;
                                break;
                            }
                        }
                        if (isValid)
                            secondaryForms.Add(new Pair<string, List<PitchClass>>('T' + i.ToString() + 'R', retrograde));
                    }
                    if (!isValid)
                        break;
                }
            }
            for (int i = 0; i < 12; i++)
            {
                List<PitchClass> retrogradeInverse = Transpose(Retrograde(Invert(pcseg)), i);
                bool isValid = true;
                for (int j = 0; j <= 12 - pitches.Count; j++)
                {
                    if (retrogradeInverse[j].PitchClassInteger == pitches[0].PitchClassInteger)
                    {
                        for (int k = j + 1; k - j < pitches.Count; k++)
                        {
                            if (retrogradeInverse[k].PitchClassInteger != pitches[k - j].PitchClassInteger)
                            {
                                isValid = false;
                                break;
                            }
                        }
                        if (isValid)
                            secondaryForms.Add(new Pair<string, List<PitchClass>>('T' + i.ToString() + "RI", retrogradeInverse));
                    }
                    if (!isValid)
                        break;
                }
            }
            return secondaryForms;
        }

        /// <summary>
        /// Gets the interval string of a pcseg
        /// </summary>
        /// <param name="pcseg">A pcseg</param>
        /// <returns>The interval string</returns>
        public static List<PitchClass> Intervals(List<PitchClass> pcseg)
        {
            List<PitchClass> list = new List<PitchClass>();
            for (int i = 1; i < pcseg.Count; i++)
                list.Add(pcseg[i] - pcseg[i - 1]);
            return list;
        }

        /// <summary>
        /// Inverts a list of pitch classes without changing the original order. This is a TTO.
        /// </summary>
        /// <param name="listToInvert">The list to invert</param>
        /// <returns></returns>
        public static List<PitchClass> Invert(List<PitchClass> listToInvert)
        {
            List<PitchClass> inverted = new List<PitchClass>(listToInvert.Count);
            for (int i = 0; i < listToInvert.Count; i++)
                inverted.Add(new PitchClass(listToInvert[i].PitchClassInteger * 11));
            return inverted;
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
        /// Multiplies a pitch class list without changing the original order. This is a TTO if the multiplier is 1, 5, 7, or 11 (mod 12).
        /// </summary>
        /// <param name="listToMultiply">The list to multiply</param>
        /// <param name="multiplier">The multiplier (by default, 5). Note that multipliers other than 1, 5, 7, or 11 
        /// (mod 12) result in epimorphisms.</param>
        /// <returns>The multiplied list</returns>
        public static List<PitchClass> Multiply(List<PitchClass> listToMultiply, int multiplier = 5)
        {
            List<PitchClass> multiplied = new List<PitchClass>(listToMultiply.Count);
            for (int i = 0; i < listToMultiply.Count; i++)
                multiplied.Add(new PitchClass(listToMultiply[i].PitchClassInteger * multiplier));
            return multiplied;
        }

        /// <summary>
        /// Generates a pitch class list from a provided string
        /// </summary>
        /// <param name="pitches">A string of pitch classes</param>
        /// <returns>A pitch class list</returns>
        public static List<PitchClass> Parse(string pitches)
        {
            List<PitchClass> list = new List<PitchClass>();
            foreach (char p in pitches)
            {
                if (p < '0' || (p > '9' && p != 'a' && p != 'A' && p != 'b' && p != 'B'))
                {
                    list.Clear();
                    break;
                }
                list.Add(new PitchClass(p));
            }
            return list;
        }

        /// <summary>
        /// Retrogrades a PcSeg
        /// </summary>
        /// <param name="pcseg">A pcseg</param>
        /// <returns>A retrograded pcseg</returns>
        public static List<PitchClass> Retrograde(List<PitchClass> pcseg)
        {
            List<PitchClass> list = CopyList(pcseg);
            list.Reverse();
            return list;
        }

        /// <summary>
        /// Rotates a PcSeg
        /// </summary>
        /// <param name="i">The index of rotation</param>
        public static List<PitchClass> Rotate(List<PitchClass> pcseg, int i)
        {
            List<PitchClass> newList = new List<PitchClass>(pcseg.Capacity);
            i %= 12;
            if (i < 0)
                i += 12;
            for (int j = 0; j < pcseg.Count; j++)
                newList.Add(pcseg[(j + i) % pcseg.Count]);
            return newList;
        }

        /// <summary>
        /// Sorts a pc list
        /// </summary>
        /// <param name="pcseg">The list</param>
        /// <returns>A sorted copy of the list</returns>
        public static List<PitchClass> Sort(List<PitchClass> pcseg)
        {
            List<PitchClass> newList = new List<PitchClass>();
            foreach (PitchClass pc in pcseg)
                newList.Add(new PitchClass(pc));
            newList.Sort((a, b) => a.PitchClassInteger.CompareTo(b.PitchClassInteger));
            return newList;
        }

        /// <summary>
        /// Gets all subsegs of a pcseg
        /// </summary>
        /// <param name="pcseg">A pcseg</param>
        /// <returns>The subsets of the pcseg</returns>
        public static List<List<PitchClass>> Subsegs(List<PitchClass> pcseg)
        {
            List<List<PitchClass>> store = new List<List<PitchClass>>();
            List<PitchClass> build = new List<PitchClass>();
            List<PitchClass> remaining = new List<PitchClass>(pcseg);

            for (int i = 1; i < pcseg.Count; i++)
                SubsegsHelper(store, build, remaining, 0, i);

            return store;
        }

        /// <summary>
        /// A helper for Subsegs
        /// </summary>
        /// <param name="store">The list to store completed subsegs</param>
        /// <param name="build">The current subseg we are building</param>
        /// <param name="remaining">The remaining pitch classes from which to choose</param>
        /// <param name="selected">The number of pitch classes already chosen</param>
        /// <param name="max">The maximum number of pitch classes to choose</param>
        private static void SubsegsHelper(List<List<PitchClass>> store, List<PitchClass> build, List<PitchClass> remaining, int selected, int max)
        {
            if (selected == max)
                store.Add(build);
            else
            {
                for (int i = 0; i <= remaining.Count - max + selected; i++)
                {
                    List<PitchClass> newBuild = new List<PitchClass>(build);
                    List<PitchClass> newRemaining = new List<PitchClass>(remaining);
                    newBuild.Add(new PitchClass(newRemaining[i]));
                    newRemaining.RemoveRange(0, i + 1);
                    SubsegsHelper(store, newBuild, newRemaining, selected + 1, max);
                }
            }
        }

        /// <summary>
        /// Converts a pitch-class list to a HashSet
        /// </summary>
        /// <param name="list">The list</param>
        /// <returns>A HashSet</returns>
        public static HashSet<PitchClass> ToPcSet(List<PitchClass> list)
        {
            HashSet<PitchClass> set = new HashSet<PitchClass>();
            foreach (PitchClass pc in list)
                set.Add(new PitchClass(pc));
            return set;
        }

        /// <summary>
        /// Converts a pitch-class list to a string
        /// </summary>
        /// <param name="list">The pitch-class list</param>
        /// <returns>A string</returns>
        public static string ToString(List<PitchClass> list, string separator = "")
        {
            System.Text.StringBuilder sb = new System.Text.StringBuilder();
            for (int i = 0; i < list.Count - 1; i++)
                sb.Append(list[i].PitchClassChar + separator);
            if (list.Count > 0)
                sb.Append(list[list.Count - 1].PitchClassChar);
            return sb.ToString();
        }

        /// <summary>
        /// Transforms a row
        /// </summary>
        /// <param name="transformationString">The transformation name</param>
        /// <returns>The transformed row</returns>
        public static List<PitchClass> Transform(List<PitchClass> pcseg, string transformationString)
        {
            List<Pair<char, int>> opList = new List<Pair<char, int>>();
            List<PitchClass> transform = pcseg;
            int numCount = 0;
            char[] VALID_OPS = new char[5] { 'T', 'R', 'I', 'r', 'M' };
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
                    transform = Transpose(transform, pair.Item2);
                else if (pair.Item1 == 'R')
                    transform = Retrograde(transform);
                else if (pair.Item1 == 'I')
                    transform = Invert(transform);
                else if (pair.Item1 == 'r')
                    transform = Rotate(transform, pair.Item2);
                else if (pair.Item1 == 'M')
                    transform = Multiply(transform, pair.Item2);
            }

            return transform;
        }

        /// <summary>
        /// Transposes a list of pitch classes without changing the original order. This is a TTO.
        /// </summary>
        /// <param name="listToTranspose">The list to transpose</param>
        /// <param name="numberOfTranspositions">The number of transpositions</param>
        /// <returns>The transposed list</returns>
        public static List<PitchClass> Transpose(List<PitchClass> listToTranspose, int numberOfTranspositions)
        {
            List<PitchClass> transposedList = new List<PitchClass>(listToTranspose.Count);
            for (int i = 0; i < listToTranspose.Count; i++)
                transposedList.Add(new PitchClass(listToTranspose[i].PitchClassInteger + numberOfTranspositions));
            return transposedList;
        }
    }
}
