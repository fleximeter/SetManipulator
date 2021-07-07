/* File: Row.cs
** Project: MusicTheory
** Author: Jeffrey Martin
**
** This file contains the implementation of the Row class.
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
    /// Represents a twelve-tone row
    /// </summary>
    public class Row
    {
        private PitchClass[] _pitchRow;         // The row
        private string _rowName;                // A row name that we can assign
        private readonly int _ROW_LENGTH = 12;  // The length of the row

        public PitchClass this[int i]
        {
            get { return _pitchRow[i]; }
        }

        /// <summary>
        /// Creates a new blank twelve-tone row
        /// </summary>
        public Row()
        {
            _pitchRow = new PitchClass[12];
            _rowName = "";
            for (int i = 0; i < _ROW_LENGTH; i++)
                _pitchRow[i] = new PitchClass(0);
        }

        /// <summary>
        /// Create a new twelve-tone row
        /// </summary>
        /// <param name="row">A 12-long row of unique pitches, with values from 0-11.</param>
        public Row(int[] row)
        {
            _pitchRow = new PitchClass[12];
            _rowName = "";
            for (int i = 0; i < _ROW_LENGTH; i++)
                _pitchRow[i] = new PitchClass();
            ImportRow(row);
        }

        /// <summary>
        /// Create a new twelve-tone row
        /// </summary>
        /// <param name="row">A list of unique pitch classes</param>
        public Row(List<PitchClass> row)
        {
            _pitchRow = new PitchClass[12];
            _rowName = "";
            for (int i = 0; i < _ROW_LENGTH; i++)
                _pitchRow[i] = new PitchClass();
            ImportRow(row);
        }

        /// <summary>
        /// Create a new twelve-tone row
        /// </summary>
        /// <param name="row">An existing twelve-tone row</param>
        public Row(Row row)
        {
            _pitchRow = new PitchClass[12];
            _rowName = "";
            for (int i = 0; i < 12; i++)
                _pitchRow[i] = new PitchClass(row[i]);
        }

        /// <summary>
        /// The name of the row (e.g. RI5)
        /// </summary>
        public string RowName
        {
            get { return _rowName; }
            set { _rowName = value; }
        }

        /// <summary>
        /// Gets the row as a string
        /// </summary>
        public string RowAsString
        {
            get
            {
                string row = "";
                foreach (PitchClass pc in _pitchRow)
                    row += pc.PitchClassChar;
                return row;
            }
        }

        /// <summary>
        /// Compares two Rows for equality
        /// </summary>
        /// <param name="obj">The second row</param>
        /// <returns>True if the two rows are equal; false otherwise</returns
        public override bool Equals(object obj) => Equals(obj as PitchClass);

        /// <summary>
        /// Compares two Rows for equality
        /// </summary>
        /// <param name="row">The second row</param>
        /// <returns>True if the two rows are equal; false otherwise</returns>
        public bool Equals(Row row)
        {
            if (_pitchRow.Length != row._pitchRow.Length)
                return false;
            for (int i = 0; i < _ROW_LENGTH; i++)
            {
                if (_pitchRow[i] != row._pitchRow[i])
                    return false;
            }
            return true;
        }

        /// <summary>
        /// Gets the hash code of the Row
        /// </summary>
        /// <returns>The hash code</returns>
        public override int GetHashCode()
        {
            if (_pitchRow is null)
                return 0;
            else
            {
                int code = _pitchRow[0].PitchClassInteger;
                for (int i = 0; i < _pitchRow.Length; i++)
                    code ^= _pitchRow[i].PitchClassInteger;
                return code;
            }
        }

        /// <summary>
        /// Get the interval list of the row
        /// </summary>
        /// <returns>The interval list</returns>
        public List<PitchClass> GetIntervalList() => PcSeg.Intervals(GetPcSeg());

        /// <summary>
        /// Gets the row as a pcseg
        /// </summary>
        /// <returns>The row as a pcseg</returns>
        public List<PitchClass> GetPcSeg()
        {
            List<PitchClass> pcseg = new List<PitchClass>(12);
            foreach (PitchClass pc in _pitchRow)
                pcseg.Add(new PitchClass(pc));
            return pcseg;
        }

        /// <summary>
        /// Gets all secondary forms that contain the provided ordered sequence of pitch classes
        /// </summary>
        /// <param name="pitches">An ordered sequence of pitch classes</param>
        /// <returns>A collection of secondary forms</returns>
        public List<Pair<string, Row>> GetSecondaryForms(List<PitchClass> pitches)
        {
            List<Pair<string, Row>> secondaryForms = new List<Pair<string, Row>>();

            for (int i = 0; i < _ROW_LENGTH; i++)
            {
                Row transpose = Transpose(i);
                bool isValid = true;
                for (int j = 0; j <= _ROW_LENGTH - pitches.Count; j++)
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
                            secondaryForms.Add(new Pair<string, Row>('T' + i.ToString(), transpose));
                    }
                    if (!isValid)
                        break;
                }
            }
            for (int i = 0; i < _ROW_LENGTH; i++)
            {
                Row invert = Invert().Transpose(i);
                bool isValid = true;
                for (int j = 0; j <= _ROW_LENGTH - pitches.Count; j++)
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
                            secondaryForms.Add(new Pair<string, Row>('T' + i.ToString() + 'I', invert));
                    }
                    if (!isValid)
                        break;
                }
            }
            for (int i = 0; i < _ROW_LENGTH; i++)
            {
                Row retrograde = Retrograde().Transpose(i);
                bool isValid = true;
                for (int j = 0; j <= _ROW_LENGTH - pitches.Count; j++)
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
                            secondaryForms.Add(new Pair<string, Row>('T' + i.ToString() + 'R', retrograde));
                    }
                    if (!isValid)
                        break;
                }
            }
            for (int i = 0; i < _ROW_LENGTH; i++)
            {
                Row retrogradeInverse = Invert().Retrograde().Transpose(i);
                bool isValid = true;
                for (int j = 0; j <= _ROW_LENGTH - pitches.Count; j++)
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
                            secondaryForms.Add(new Pair<string, Row>('T' + i.ToString() + "RI", retrogradeInverse));
                    }
                    if (!isValid)
                        break;
                }
            }
            return secondaryForms;
        }

        /// <summary>
        /// Imports a twelve-tone row from a list of pitch classes
        /// </summary>
        /// <param name="row">A 12-long row of unique pitches</param>
        public void ImportRow(List<PitchClass> row)
        {
            // Requires the row to contain exactly twelve notes before proceeding
            if (row.Count == 12)
            {
                bool validRow = true;  // Track if the row is valid
                List<PitchClass> usedPitches = new List<PitchClass>();  // A list of used pitches

                foreach (PitchClass i in row)
                {
                    // If the current pitch is not a duplicate, add it to the list of used pitches.
                    if (!usedPitches.Contains(i))
                        usedPitches.Add(i);

                    // Otherwise the row is invalid and we can't use it.
                    else
                        validRow = false;
                }

                // If we've determined the row is valid, we import it.
                if (validRow)
                {
                    for (int i = 0; i < _ROW_LENGTH; i++)
                        _pitchRow[i].PitchClassInteger = row[i].PitchClassInteger;
                }
            }
        }

        /// <summary>
        /// Imports a twelve-tone row from a list of integers
        /// </summary>
        /// <param name="row">A 12-long row of unique pitches, with values from 0-11.</param>
        public void ImportRow(int[] row)
        {
            // Requires the row to contain exactly twelve notes before proceeding
            if (row.Length == 12)
            {
                bool validRow = true;  // Track if the row is valid
                List<int> usedPitches = new List<int>();  // A list of used pitches

                foreach (int i in row)
                {
                    // If the current pitch is not a duplicate, and it is between 0 and 11 inclusive,
                    // add it to the list of used pitches.
                    if (!usedPitches.Contains(i) && i >= 0 && i < 12)
                        usedPitches.Add(i);

                    // Otherwise the row is invalid and we can't use it.
                    else
                        validRow = false;
                }

                // If we've determined the row is valid, we import it.
                if (validRow)
                {
                    for (int i = 0; i < _ROW_LENGTH; i++)
                        _pitchRow[i].PitchClassInteger = row[i];
                }
            }
        }

        /// <summary>
        /// <summary>
        /// Inverts the row
        /// </summary>
        /// <returns>The inversion of the row</returns>
        public Row Invert()
        {
            List<PitchClass> newPitchRow = new List<PitchClass>();

            foreach (PitchClass pitchClass in _pitchRow)
            {
                if (pitchClass > _pitchRow[0] || pitchClass < _pitchRow[0])
                    newPitchRow.Add(new PitchClass(2 * (_pitchRow[0].PitchClassInteger) - pitchClass.PitchClassInteger));
                else
                    newPitchRow.Add(new PitchClass(pitchClass.PitchClassInteger));
            }

            return new Row(newPitchRow);
        }

        /// <summary>
        /// Validates a row generator (sequence of 11 intervals)
        /// </summary>
        /// <param name="rGen">The row generator</param>
        /// <returns>True if the generator is valid; false otherwise</returns>
        public static bool IsValidRGen(int[] rGen)
        {
            if (rGen.Length != 11)
                return false;
            for (int i = 0; i < 10; i++)
            {
                int sum = rGen[i];
                for (int j = i + 1; j < 11; j++)
                {
                    sum += rGen[j];
                    if (sum % 12 == 0)
                        return false;
                }
            }
            return true;
        }

        /// <summary>
        /// Validates a potential twelve tone row
        /// </summary>
        /// <param name="row">The row to evaluate</param>
        /// <returns>True if the row is valid; false otherwise</returns>
        public static bool IsValidRow(string row)
        {
            if (row.Length != 12)
                return false;

            List<PitchClass> pcs = PcSeg.Parse(row);
            Dictionary<int, bool> usedPC = new Dictionary<int, bool>();
            foreach (PitchClass pc in pcs)
            {
                if (!usedPC.ContainsKey(pc.PitchClassInteger))
                    usedPC[pc.PitchClassInteger] = true;
                else
                    return false;
            }
            return true;
        }

        /// <summary>
        /// Validates a potential twelve tone row
        /// </summary>
        /// <param name="row">The row to evaluate</param>
        /// <returns>True if the row is valid; false otherwise</returns>
        public static bool IsValidRow(List<PitchClass> row)
        {
            if (row.Count != 12)
                return false;

            Dictionary<int, bool> usedPC = new Dictionary<int, bool>();
            foreach (PitchClass pc in row)
            {
                if (!usedPC.ContainsKey(pc.PitchClassInteger))
                    usedPC[pc.PitchClassInteger] = true;
                else
                    return false;
            }
            return true;
        }

        /// <summary>
        /// Loads a random twelve-tone row
        /// </summary>
        public void LoadRandomRow()
        {
            int[] numbers = new int[12] { 0, 1, 2, 3, 4, 5, 6, 7, 8, 9, 10, 11 };
            System.Random rand = new System.Random();
            int numTimes = rand.Next(5, 20);
            for (int i = 0; i < numTimes; i++)
            {
                List<int> num1 = new List<int>(numbers);
                for (int j = 0; j < _ROW_LENGTH; j++)
                {
                    int k = rand.Next(0, num1.Count);
                    numbers[j] = num1[k];
                    num1.RemoveAt(k);
                }
            }
            for (int i = 0; i < _ROW_LENGTH; i++)
                _pitchRow[i].PitchClassInteger = numbers[i];
            _rowName = "";
        }

        /// <summary>
        /// Loads a row from a row generator. Does not perform validation - you must use
        /// IsValidRGen() for that.
        /// </summary>
        /// <param name="rGen">The row generator</param>
        public void LoadRowFromRGen(int[] rGen)
        {
            _pitchRow[0].PitchClassInteger = 0;
            for (int i = 1; i < _ROW_LENGTH; i++)
                _pitchRow[i].PitchClassInteger = _pitchRow[i - 1].PitchClassInteger + rGen[i - 1];
            _rowName = "";
        }

        /// <summary>
        /// Multiplies the row
        /// </summary>
        /// <param name="Multiplier">The number by which to multiply</param>
        /// <returns>The new row</returns>
        public Row Multiply(int Multiplier)
        {
            List<PitchClass> newPitchRow = new List<PitchClass>();
            foreach (PitchClass pitchClass in _pitchRow)
                newPitchRow.Add(pitchClass * Multiplier);
            return new Row(newPitchRow);
        }

        /// <summary>
        /// Clears the current row name
        /// </summary>
        public void ResetRowName() { _rowName = ""; }

        /// Retrogrades the row
        /// </summary>
        /// <returns>The retrograde of the row</returns>
        public Row Retrograde()
        {
            List<PitchClass> newPitchRow = new List<PitchClass>();
            for (int i = _ROW_LENGTH - 1; i >= 0; i--)
                newPitchRow.Add(_pitchRow[i]);
            return new Row(newPitchRow);
        }

        /// <summary>
        /// Rotates the row
        /// </summary>
        /// <param name="NumberOfRotations">The number of times to rotate</param>
        /// <returns>The new row</returns>
        public Row Rotate(int NumberOfRotations)
        {
            List<PitchClass> newPitchRow = new List<PitchClass>();
            for (int i = 0; i < _ROW_LENGTH; i++)
                newPitchRow.Add(_pitchRow[(NumberOfRotations + i) % _ROW_LENGTH]);
            return new Row(newPitchRow);
        }

        /// <summary>
        /// Transforms a row
        /// </summary>
        /// <param name="transformationString">The transformation name</param>
        /// <returns>The transformed row</returns>
        public Row Transform(string transformationString)
        {
            List<Pair<char, int>> opList = new List<Pair<char, int>>();
            Row row = this;
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
                    row = row.Transpose(pair.Item2);
                else if (pair.Item1 == 'R')
                    row = row.Retrograde();
                else if (pair.Item1 == 'I')
                    row = row.Invert();
                else if (pair.Item1 == 'r')
                    row = row.Rotate(pair.Item2);
                else if (pair.Item1 == 'M')
                    row = row.Multiply(pair.Item2);
            }

            return row;
        }

        /// <summary>
        /// Transposes the row
        /// </summary>
        /// <param name="NumberOfTranspositions">The distance to transpose the row</param>
        /// <returns>The transposed row</returns>
        public Row Transpose(int NumberOfTranspositions)
        {
            List<PitchClass> newPitchRow = new List<PitchClass>();
            foreach (PitchClass pitchClass in _pitchRow)
                newPitchRow.Add(pitchClass + NumberOfTranspositions);
            return new Row(newPitchRow);
        }

        /// <summary>
        /// Transposes the row
        /// </summary>
        /// <param name="StartingPitch">The new starting pitch</param>
        /// <returns>The transposed row</returns>
        public Row Transpose(PitchClass StartingPitch)
        {
            int numberOfTranspositions = (StartingPitch - _pitchRow[0]).PitchClassInteger;
            List<PitchClass> newPitchRow = new List<PitchClass>();
            foreach (PitchClass pitchClass in _pitchRow)
                newPitchRow.Add(pitchClass + numberOfTranspositions);
            return new Row(newPitchRow);
        }

        /// <summary>
        /// Compares two Rows for equality
        /// </summary>
        /// <param name="row1">The first row</param>
        /// <param name="row2">The second row</param>
        /// <returns>True if the rows are equal; false otherwise</returns>
        public static bool operator ==(Row row1, Row row2)
        {
            if (row1 is null)
            {
                if (row2 is null)
                    return true;
                return false;
            }
            return row1.Equals(row2);
        }

        /// <summary>
        /// Compares two Rows for inequality
        /// </summary>
        /// <param name="row1">The first row</param>
        /// <param name="row2">The second row</param>
        /// <returns>True if the rows are inequal; false otherwise</returns>
        public static bool operator !=(Row row1, Row row2) => !(row1 == row2);
    }
}
