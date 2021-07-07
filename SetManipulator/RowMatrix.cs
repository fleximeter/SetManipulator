/* File: RowMatrix.cs
** Project: MusicTheory
** Author: Jeffrey Martin
**
** This file contains the implementation of the RowMatrix class.
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

namespace MusicTheory
{
    /// <summary>
    /// Represents a twelve tone matrix
    /// </summary>
    public class RowMatrix
    {
        PitchClass[,] _matrix = new PitchClass[12, 12];
        Row _row;
        int[] _bottomColumnNames = new int[12];
        int[] _leftRowNames = new int[12];
        int[] _rightRowNames = new int[12];
        int[] _topColumnNames = new int[12];
        readonly int _ROW_LENGTH = 12;

        public PitchClass this[int i, int j]
        {
            get { return _matrix[i, j]; }
        }

        /// <summary>
        /// Creates a new RowMatrix
        /// </summary>
        public RowMatrix() { }

        /// <summary>
        /// Creates a new RowMatrix
        /// </summary>
        /// <param name="row">A row to load into the matrix</param>
        public RowMatrix(Row row)
        {
            ImportRow(row);
        }

        /// <summary>
        /// Gets the transposition level of a RI form
        /// </summary>
        /// <param name="index">The index</param>
        /// <returns>The transposition level</returns>
        public int GetBottomColumnName(int index)
        {
            return _bottomColumnNames[index];
        }

        /// <summary>
        /// Gets the inverted row at the specified transposition
        /// </summary>
        /// <param name="index">The transposition index</param>
        /// <returns>The inverted and transposed row</returns>
        public Row GetI(int index)
        {
            return _row.Invert().Transpose(index);
        }

        /// <summary>
        /// Gets the row at the specified transposition
        /// </summary>
        /// <param name="index">The transposition index</param>
        /// <returns>The transposed row</returns>
        public Row GetP(int index)
        {
            return _row.Transpose(index);
        }

        /// <summary>
        /// Gets the retrograded row at the specified transposition
        /// </summary>
        /// <param name="index">The transposition index</param>
        /// <returns>The retrograded and transposed row</returns>
        public Row GetR(int index)
        {
            return _row.Retrograde().Transpose(index);
        }

        /// <summary>
        /// Gets the retrograde inverted row at the specified transposition
        /// </summary>
        /// <param name="index">The transposition index</param>
        /// <returns>The retrograde inverted and transposed row</returns>
        public Row GetRI(int index)
        {
            return _row.Invert().Retrograde().Transpose(index);
        }

        /// <summary>
        /// Gets the transposition level of an I form
        /// </summary>
        /// <param name="index">The index</param>
        /// <returns>The transposition level</returns>
        public int GetTopColumnName(int index)
        {
            return _topColumnNames[index];
        }

        /// <summary>
        /// Gets the transposition level of a P form
        /// </summary>
        /// <param name="index">The index</param>
        /// <returns>The transposition level</returns>
        public int GetLeftRowName(int index)
        {
            return _leftRowNames[index];
        }

        /// <summary>
        /// Gets the transposition level of a R form
        /// </summary>
        /// <param name="index">The index</param>
        /// <returns>The transposition level</returns>
        public int GetRightRowName(int index)
        {
            return _rightRowNames[index];
        }

        /// <summary>
        /// Gets the bottom column name at a specified index
        /// </summary>
        /// <param name="index">The index</param>
        /// <returns>The name as a string</returns>
        public string GetBottomColumnNameString(int index)
        {
            return "T" + _bottomColumnNames[index].ToString() + "RI";
        }

        /// <summary>
        /// Gets the top column name at a specified index
        /// </summary>
        /// <param name="index">The index</param>
        /// <returns>The name as a string</returns>
        public string GetTopColumnNameString(int index)
        {
            return "T" + _topColumnNames[index].ToString() + "I";
        }

        /// <summary>
        /// Gets the left row name at a specified index
        /// </summary>
        /// <param name="index">The index</param>
        /// <returns>The name as a string</returns>
        public string GetLeftRowNameString(int index)
        {
            return "T" + _leftRowNames[index].ToString();
        }

        /// <summary>
        /// Gets the right row name at a specified index
        /// </summary>
        /// <param name="index">The index</param>
        /// <returns>The name as a string</returns>
        public string GetRightRowNameString(int index)
        {
            return "T" + _rightRowNames[index].ToString() + "R";
        }

        /// <summary>
        /// Imports a row into the matrix
        /// </summary>
        /// <param name="row">The row to import</param>
        public void ImportRow(Row row)
        {
            _row = new Row(row);
            Row invert = _row.Invert();

            for (int i = 0; i < _ROW_LENGTH; i++)
            {
                Row transpose = _row.Transpose(invert[i]);
                for (int j = 0; j < _ROW_LENGTH; j++)
                    _matrix[i, j] = transpose[j];
            }

            UpdateNames();
        }

        /// <summary>
        /// Updates the names of the matrix
        /// </summary>
        private void UpdateNames()
        {
            for (int i = 0; i < _ROW_LENGTH; i++)
            {
                PitchClass pTransposition = _matrix[i, 0] - _matrix[0, 0];
                PitchClass iTransposition = _matrix[0, i] - _matrix[0, 0];
                PitchClass rTransposition = _matrix[i, _ROW_LENGTH - 1] - _matrix[0, _ROW_LENGTH - 1];
                PitchClass riTransposition = _matrix[_ROW_LENGTH - 1, i] - _matrix[_ROW_LENGTH - 1, 0];

                _bottomColumnNames[i] = riTransposition.PitchClassInteger;
                _leftRowNames[i] = pTransposition.PitchClassInteger;
                _rightRowNames[i] = rTransposition.PitchClassInteger;
                _topColumnNames[i] = iTransposition.PitchClassInteger;
            }
        }
    }
}
