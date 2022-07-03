/* File: CSeg.cs
** Project: MusicTheory
** Author: Jeffrey Martin
**
** This file contains the implementation of the CSeg class.
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
using System.Text;

namespace MusicTheory
{
    /// <summary>
    /// Contains functions for working with csegs
    /// </summary>
    public static class CSeg
    {
        /// <summary>
        /// The COM function for two cps
        /// </summary>
        /// <param name="a">A cp</param>
        /// <param name="b">A cp</param>
        /// <returns>1 if a < b, 0 if a == b, -1 if a > b</b></returns>
        public static int Com(uint a, uint b)
        {
            if (a < b)
                return 1;
            else if (a > b)
                return -1;
            else
                return 0;
        }

        /// <summary>
        /// Generates a COM matrix for two csegs
        /// </summary>
        /// <param name="contour1">A cseg</param>
        /// <param name="contour2">A cseg</param>
        /// <returns></returns>
        public static int[,] ComMatrix(List<uint> contour1, List<uint> contour2)
        {
            int[,] matrix = new int[contour2.Count, contour1.Count];
            for (int i = 0; i < contour2.Count; i++)
            {
                for (int j = 0; j < contour1.Count; j++)
                    matrix[i, j] = Com(contour1[j], contour2[i]);
            }
            return matrix;
        }

        /// <summary>
        /// Converts a pseg to a cseg
        /// </summary>
        /// <param name="pseg">A pseg</param>
        /// <returns>A cseg</returns>
        public static List<uint> ConvertFromPseg(List<Pitch> pseg)
        {
            List<uint> cseg = new List<uint>();
            List<uint> cseg2;
            Dictionary<uint, uint> map = new Dictionary<uint, uint>();
            uint n = 0;
            uint lastFound = 0;
            int min = 0;

            // Convert from pitches to cps
            foreach (Pitch p in pseg)
            {
                if (p.PitchNum < min)
                    min = p.PitchNum;
            }
            min *= -1;
            for (int i = 0; i < pseg.Count; i++)
                cseg.Add((uint)(pseg[i].PitchNum + min));

            // Reduce to most compact collection of cps
            cseg2 = new List<uint>(cseg);
            cseg2.Sort();
            for (int i = 0; i < cseg2.Count; i++)
            {
                if (i == 0 || cseg2[i] > lastFound)
                {
                    map.Add(cseg2[i], n);
                    lastFound = cseg2[i];
                    n++;
                }
            }
            cseg2.Clear();
            for (int i = 0; i < cseg.Count; i++)
                cseg2.Add(map[cseg[i]]);

            return cseg2;
        }

        /// <summary>
        /// Compares two COM matrices for equivalence
        /// </summary>
        /// <param name="matrix1">A COM matrix</param>
        /// <param name="matrix2">A COM matrix</param>
        /// <returns></returns>
        public static bool Equivalence(int[,] matrix1, int[,] matrix2)
        {
            if (matrix1.GetLength(0) == matrix2.GetLength(0) && matrix1.GetLength(1) == matrix2.GetLength(1))
                return matrix1.Equals(matrix2);
            return false;
        }

        /// <summary>
        /// Inverts a cseg
        /// </summary>
        /// <param name="listToInvert">The cseg to invert</param>
        /// <returns>The inverted cseg</returns>
        public static List<uint> Invert(List<uint> listToInvert, uint max = 0)
        {
            List<uint> inverted = new List<uint>(listToInvert.Count);
            if (max == 0)
            {
                foreach (uint i in listToInvert)
                {
                    if (i > max)
                        max = i;
                }
                max++;
            }
            for (int i = 0; i < listToInvert.Count; i++)
                inverted.Add(max - 1 - listToInvert[i]);
            return inverted;
        }

        /// <summary>
        /// Validates a c-string
        /// </summary>
        /// <param name="cstring">A c-string</param>
        /// <returns>True if the c-string is valid; false otherwise</returns>
        public static bool IsValidCString(string cstring)
        {
            string[] cs;
            uint a;
            string split;
            if (cstring.Contains(", "))
                split = ", ";
            else if (cstring.Contains(" "))
                split = " ";
            else
                split = ",";
            cs = cstring.Split(split);
            foreach (string c in cs)
            {
                if (c.Length == 0)
                    return false;
                if (!uint.TryParse(c, out a))
                    return false;
            }
            return true;
        }

        /// <summary>
        /// Validates potential transformation names
        /// </summary>
        /// <param name="transformationName">The transformation name</param>
        /// <returns>True if the name is valid; false otherwise</returns>
        public static bool IsValidTransformation(string transformationName)
        {
            char[] VALID_OPS = new char[3] { 'R', 'I', 'r' };
            char[] VALID_CHARS = new char[4] { 'R', 'I', 'r', '-' };
            if (transformationName.Length == 0)
                return false;
            if (System.Array.Find(VALID_OPS, (a) => a == transformationName[0]) == -1)
                return false;
            for (int i = 0; i < transformationName.Length; i++)
            {
                char c = transformationName[i];

                // r transformations must be followed by a number
                if (c == 'r')
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
        /// Converts a string to a cseg
        /// </summary>
        /// <param name="cstring">A string</param>
        /// <returns>A cseg</returns>
        public static List<uint> Parse(string cstring)
        {
            List<uint> cseg = new List<uint>();
            string split = "";
            if (cstring.Contains(", "))
                split = ", ";
            else if (cstring.Contains(" "))
                split = " ";
            else
                split = ",";
            string[] cps = cstring.Split(split);
            foreach (string cp in cps)
                cseg.Add(uint.Parse(cp));
            return cseg;
        }

        /// <summary>
        /// Calculates the prime form of a cseg-class (Laprade)
        /// </summary>
        /// <param name="cseg">A cseg</param>
        /// <returns>The prime form cseg</returns>
        public static List<uint> PrimeForm(List<uint> cseg)
        {
            List<uint> prime = new List<uint>(cseg);
            Dictionary<uint, uint> map = new Dictionary<uint, uint>();
            uint n = 0;
            uint lastFound = 0;
            bool invertComplete = false;
            bool retrogradeComplete = false;
            int a = 0;
            int b = cseg.Count - 1;

            // 1. Reduce to most compact collection of cps
            prime.Sort();
            for (int i = 0; i < prime.Count; i++)
            {
                if (i == 0 || prime[i] > lastFound)
                {
                    map.Add(prime[i], n);
                    lastFound = prime[i];
                    n++;
                }
            }
            prime.Clear();
            for (int i = 0; i < cseg.Count; i++)
                prime.Add(map[cseg[i]]);

            // 2. Determine if inversion is necessary, and invert if so
            if (cseg.Count < 3)
                invertComplete = true;
            while (!invertComplete)
            {
                if (a >= b)
                    invertComplete = true;
                else if (prime[a] == prime[b])
                {
                    a++;
                    b--;
                }
                else if ((int)n - 1 - (int)prime[b] < (int)prime[a])
                {
                    prime = Invert(prime);
                    invertComplete = true;
                }
                else
                    invertComplete = true;
            }

            // 3. Determine if retrograde is necessary, and retrograde if so
            a = 0;
            b = cseg.Count - 1;
            if (cseg.Count < 2)
                retrogradeComplete = true;
            while (!retrogradeComplete)
            {
                if (a >= b)
                    retrogradeComplete = true;
                else if (prime[a] == prime[b])
                {
                    a++;
                    b--;
                }
                else if (prime[b] < prime[a])
                {
                    prime = Retrograde(prime);
                    retrogradeComplete = true;
                }
                else
                    retrogradeComplete = true;
            }

            return prime;
        }

        /// <summary>
        /// Retrogrades a cseg
        /// </summary>
        /// <param name="cseg">A cseg</param>
        /// <returns>The retrograded cseg</returns>
        public static List<uint> Retrograde(List<uint> cseg)
        {
            List<uint> list = new List<uint>(cseg);
            list.Reverse();
            return list;
        }

        /// <summary>
        /// Rotates a cseg
        /// </summary>
        /// <param name="cseg">A cseg</param>
        /// <param name="i">The index of rotation</param>
        /// <returns>The rotated cseg</returns>
        public static List<uint> Rotate(List<uint> cseg, int i)
        {
            List<uint> newList = new List<uint>(cseg.Capacity);
            if (i < 0)
                i = ((i % cseg.Count) + cseg.Count) % cseg.Count;
            for (int j = 0; j < cseg.Count; j++)
                newList.Add(cseg[(j - i + cseg.Count) % cseg.Count]);
            return newList;
        }

        /// <summary>
        /// Gets all subsegs of a cseg
        /// </summary>
        /// <param name="cseg">A cseg</param>
        /// <returns>The subsets of the cseg</returns>
        public static List<List<uint>> Subsegs(List<uint> cseg)
        {
            List<List<uint>> store = new List<List<uint>>();
            List<uint> build = new List<uint>();
            List<uint> remaining = new List<uint>(cseg);

            for (int i = 1; i < cseg.Count; i++)
                SubsegsHelper(store, build, remaining, 0, i);

            return store;
        }

        /// <summary>
        /// A helper for Subsegs
        /// </summary>
        /// <param name="store">The list to store completed subsegs</param>
        /// <param name="build">The current subseg we are building</param>
        /// <param name="remaining">The remaining cps from which to choose</param>
        /// <param name="selected">The number of cps already chosen</param>
        /// <param name="max">The maximum number of cps to choose</param>
        private static void SubsegsHelper(List<List<uint>> store, List<uint> build, List<uint> remaining, int selected, int max)
        {
            if (selected == max)
                store.Add(build);
            else
            {
                for (int i = 0; i <= remaining.Count - max + selected; i++)
                {
                    List<uint> newBuild = new List<uint>(build);
                    List<uint> newRemaining = new List<uint>(remaining);
                    newBuild.Add(newRemaining[i]);
                    newRemaining.RemoveRange(0, i + 1);
                    SubsegsHelper(store, newBuild, newRemaining, selected + 1, max);
                }
            }
        }

        /// <summary>
        /// Converts a cseg to string
        /// </summary>
        /// <param name="cseg">A cseg</param>
        /// <param name="separator">A separator</param>
        /// <returns>The cseg as a string</returns>
        public static string ToString(List<uint> cseg, string separator = "")
        {
            StringBuilder s = new StringBuilder();
            for (int i = 0; i < cseg.Count - 1; i++)
            {
                s.Append(cseg[i].ToString());
                s.Append(separator);
            }
            if (cseg.Count > 0)
                s.Append(cseg[cseg.Count - 1]);
            return s.ToString();
        }

        /// <summary>
        /// Transforms a cseg
        /// </summary>
        /// <param name="transformString">The transformation name</param>
        /// <returns>The transformed cseg</returns>
        public static List<uint> Transform(List<uint> cseg, string transformString)
        {
            List<Pair<char, int>> opList = new List<Pair<char, int>>();
            List<uint> transform = cseg;
            int numCount = 0;
            char[] VALID_OPS = new char[3] { 'R', 'I', 'r' };
            opList.Add(new Pair<char, int>('0', 0));

            // Parse the transformation string and separate it into opcodes and numbers
            for (int i = transformString.Length - 1; i >= 0; i--)
            {
                char c = transformString[i];
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
                if (pair.Item1 == 'R')
                    transform = Retrograde(transform);
                else if (pair.Item1 == 'I')
                    transform = Invert(transform);
                else if (pair.Item1 == 'r')
                    transform = Rotate(transform, pair.Item2);
            }

            return transform;
        }
    }
}
