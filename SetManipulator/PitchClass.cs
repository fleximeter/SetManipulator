/* File: PitchClass.cs
** Project: MusicTheory
** Author: Jeffrey Martin
**
** This file contains the implementation of the PitchClass class.
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
    /// Indicates a preference for notating accidentals with sharps or flats
    /// </summary>
    public enum AccidentalPreference
    {
        Flat,
        Sharp
    }

    /// <summary>
    /// Represents a single pitch class and the operations that can be performed on it
    /// </summary>
    public class PitchClass
    {
        // The pitch class in integer notation (0-11)
        protected int _pitchClassInteger = 0;
        protected const int NUM_PC = 12;

        /// <summary>
        /// The pitch class integer. Setting is mod 12.
        /// </summary>
        public int PitchClassInteger
        {
            get { return _pitchClassInteger; }
            set { UpdatePitchClass(value); }
        }

        /// <summary>
        /// The pitch class as a character. 10 is A and 11 is B.
        /// </summary>
        public char PitchClassChar
        {
            get { return Functions.HexChars[_pitchClassInteger]; }
            set
            {
                if (value == 'A' || value == 'a')
                    UpdatePitchClass(10);
                else if (value == 'B' || value == 'b')
                    UpdatePitchClass(11);
                else
                    UpdatePitchClass(value - '0');
            }
        }

        /// <summary>
        /// Creates a new PitchClass
        /// </summary>
        public PitchClass() { }

        /// <summary>
        /// Creates a new PitchClass
        /// </summary>
        /// <param name="pitch">A pitch class to use</param>
        public PitchClass(int pitch) { UpdatePitchClass(pitch); }

        /// <summary>
        /// Creates a new PitchClass
        /// </summary>
        /// <param name="pitch">The pitch class, as a character</param>
        public PitchClass(char pitch)
        {
            if (pitch == 'A' || pitch == 'a')
                UpdatePitchClass(10);
            else if (pitch == 'B' || pitch == 'b')
                UpdatePitchClass(11);
            else
                UpdatePitchClass(pitch - '0');
        }
        
        /// <summary>
        /// Creates a new PitchClass
        /// </summary>
        /// <param name="pitchClass">A PitchClass to copy</param>
        public PitchClass(PitchClass pitchClass) { _pitchClassInteger = pitchClass.PitchClassInteger; }

        /// <summary>
        /// Compares two PitchClasses
        /// </summary>
        /// <param name="obj">The object to compare</param>
        /// <returns>True if the two objects are equal; false otherwise</returns>
        public override bool Equals(object obj) => Equals(obj as PitchClass);

        /// <summary>
        /// Compares two PitchClasses
        /// </summary>
        /// <param name="pc">The PitchClass to compare</param>
        /// <returns>True if the two PitchClasses are equal; false otherwise</returns>
        public bool Equals(PitchClass pc)
        {
            if (pc is null)
                return false;
            else if (ReferenceEquals(this, pc))
                return true;
            else if (_pitchClassInteger == pc.PitchClassInteger)
                return true;
            else
                return false;
        }

        /// <summary>
        /// Determines if a character is a valid pitch class character
        /// </summary>
        /// <param name="c">The character</param>
        /// <returns>True if it is valid; false otherwise</returns>
        public static bool IsValidPitchClassChar(char c)
        {
            if (c >= '0' && c <= '9')
                return true;
            else if (c == 'A' || c == 'a')
                return true;
            else if (c == 'B' || c == 'b')
                return true;
            else
                return false;
        }

        /// <summary>
        /// Gets the hash code of the current PitchClass
        /// </summary>
        /// <returns>The hash code</returns>
        public override int GetHashCode() => _pitchClassInteger;

        /// <summary>
        /// Gets the pitch class name
        /// </summary>
        /// <param name="accidentalPreference">Whether to prefer sharps or flats</param>
        /// <returns>The pitch class name</returns>
        public string GetPitchClassName(AccidentalPreference accidentalPreference)
        {
            if (accidentalPreference == AccidentalPreference.Flat)
                return Functions.NoteNamesFlat[_pitchClassInteger];
            else
                return Functions.NoteNamesSharp[_pitchClassInteger];
        }
        
        /// <summary>
        /// Updates the pitch class mod 12
        /// </summary>
        /// <param name="newPitchClass">The new pitch class</param>
        protected void UpdatePitchClass(int newPitchClass)
        {
            // Map the pitch integer to a pitch class
            newPitchClass %= NUM_PC;
            if (newPitchClass < 0)
                newPitchClass += NUM_PC;

            _pitchClassInteger = newPitchClass;
        }

        /// <summary>
        /// Operator for pitch class addition
        /// </summary>
        /// <param name="pitch">A pitch class that will be added to</param>
        /// <param name="val">An integer value to add to the pitch class</param>
        /// <returns>A new pitch class, consisting of the original pitch class 
        /// added to the value, mod 12</returns>
        public static PitchClass operator +(PitchClass pitch, int val) => new PitchClass(pitch.PitchClassInteger + val);

        /// <summary>
        /// Operator for pitch class addition
        /// </summary>
        /// <param name="pitch1">A pitch class that will be addd to</param>
        /// <param name="pitch2">A pitch class that will be added to the first pitch class</param>
        /// <returns>A new pitch class, consisting of pitch class 2 added to pitch class 1,
        /// mod 12</returns>
        public static PitchClass operator +(PitchClass pitch1, PitchClass pitch2) => new PitchClass(pitch1.PitchClassInteger + pitch2.PitchClassInteger);

        /// <summary>
        /// Operator for pitch class subtraction
        /// </summary>
        /// <param name="pitch">A pitch class that will be subtracted from</param>
        /// <param name="val">An integer value to subtract from the pitch class</param>
        /// <returns>A new pitch class, consisting of the original pitch class 
        /// minus val, mod 12</returns>
        public static PitchClass operator -(PitchClass pitch, int val) => new PitchClass(pitch.PitchClassInteger - val);

        /// <summary>
        /// Operator for pitch class subtraction
        /// </summary>
        /// <param name="pitch1">A pitch class that will be subtracted from</param>
        /// <param name="pitch2">A pitch class to subtract from the first pitch class</param>
        /// <returns>A new pitch class, consisting of the first pitch class
        /// minus the second pitch class, mod 12</returns>
        public static PitchClass operator -(PitchClass pitch1, PitchClass pitch2) => new PitchClass(pitch1.PitchClassInteger - pitch2.PitchClassInteger);

        /// <summary>
        /// Operator for pitch class multiplication
        /// </summary>
        /// <param name="pitch">A pitch class that will be multiplied</param>
        /// <param name="val">The multiplier</param>
        /// <returns>A new pitch class, consisting of the original pitch class 
        /// multiplied by val, mod 12</returns>
        public static PitchClass operator *(PitchClass pitch, int val) => new PitchClass(pitch.PitchClassInteger * val);

        /// <summary>
        /// Operator for pitch class multiplication
        /// </summary>
        /// <param name="pitch1">A pitch class that will be multiplied</param>
        /// <param name="pitch2">A pitch class that will be a multiplier</param>
        /// <returns>A new pitch class, consisting of the original pitch class 
        /// multiplied by the second pitch class</returns>
        public static PitchClass operator *(PitchClass pitch1, PitchClass pitch2) => new PitchClass(pitch1.PitchClassInteger * pitch2.PitchClassInteger);

        /// <summary>
        /// Operator for pitch class division
        /// </summary>
        /// <param name="pitch">A pitch class that will be divided</param>
        /// <param name="val">The divisor</param>
        /// <returns>A new pitch class, consisting of the original pitch class
        /// divided by val</returns>
        public static PitchClass operator /(PitchClass pitch, int val) => new PitchClass(pitch.PitchClassInteger / val);

        /// <summary>
        /// Operator for pitch class division
        /// </summary>
        /// <param name="pitch1">A pitch class that will be divided</param>
        /// <param name="pitch2">A pitch class that will serve as the divisor</param>
        /// <returns>A new pitch class, consisting of the original pitch class
        /// divided by the second pitch class</returns>
        public static PitchClass operator /(PitchClass pitch1, PitchClass pitch2) => new PitchClass(pitch1.PitchClassInteger / pitch2.PitchClassInteger);

        /// <summary>
        /// Greater than operator for pitch class comparison
        /// </summary>
        /// <param name="pitch1">The pitch that should be greater</param>
        /// <param name="pitch2">The pitch that should be less</param>
        /// <returns>True if pitch 1 is greater than pitch 2; false otherwise</returns>
        public static bool operator >(PitchClass pitch1, PitchClass pitch2)
        {
            if (pitch1.PitchClassInteger > pitch2.PitchClassInteger)
                return true;
            else
                return false;
        }

        /// Greater than or equal to operator for pitch class comparison
        /// </summary>
        /// <param name="pitch1">The pitch that should be greater</param>
        /// <param name="pitch2">The pitch that should be less</param>
        /// <returns>True if pitch 1 is greater than or equal to pitch 2; false otherwise</returns>
        public static bool operator >=(PitchClass pitch1, PitchClass pitch2)
        {
            if (pitch1.PitchClassInteger >= pitch2.PitchClassInteger)
                return true;
            else
                return false;
        }

        /// <summary>
        /// Less than operator for pitch class comparison
        /// </summary>
        /// <param name="pitch1">The pitch that should be less</param>
        /// <param name="pitch2">The pitch that should be greater</param>
        /// <returns>True if pitch 1 is less than pitch 2; false otherwise</returns>
        public static bool operator <(PitchClass pitch1, PitchClass pitch2)
        {
            if (pitch1.PitchClassInteger < pitch2.PitchClassInteger)
                return true;
            else
                return false;
        }

        /// <summary>
        /// Less than or equal to operator for pitch class comparison
        /// </summary>
        /// <param name="pitch1">The pitch that should be less</param>
        /// <param name="pitch2">The pitch that should be greater</param>
        /// <returns>True if pitch 1 is less than or equal to pitch 2; false otherwise</returns>
        public static bool operator <=(PitchClass pitch1, PitchClass pitch2)
        {
            if (pitch1.PitchClassInteger <= pitch2.PitchClassInteger)
                return true;
            else
                return false;
        }

        /// <summary>
        /// Equal operator
        /// </summary>
        /// <param name="pc1">Pitch class 1</param>
        /// <param name="pc2">Pitch class 2</param>
        /// <returns>True if the pitch classes are equal; false otherwise</returns>
        public static bool operator ==(PitchClass pc1, PitchClass pc2)
        {
            if (pc1 is null)
            {
                if (pc2 is null)
                    return true;
                return false;
            }
            return pc1.Equals(pc2);
        }

        /// <summary>
        /// Not equal operator
        /// </summary>
        /// <param name="pc1">Pitch class 1</param>
        /// <param name="pc2">Pitch class 2</param>
        /// <returns>True if the pitch classes are equal; false otherwise</returns>
        public static bool operator !=(PitchClass pc1, PitchClass pc2) => !(pc1 == pc2);
    }
}
