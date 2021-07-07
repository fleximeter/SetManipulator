/* File: Pitch.cs
** Project: MusicTheory
** Author: Jeffrey Martin
**
** This file contains the implementation of the Pitch class.
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
    /// Represents a single Pitch, descended from PitchClass
    /// </summary>
    public class Pitch : PitchClass
    {
        private int _noteNumber = 0;
        private const int _MIDI_C = 60;

        /// <summary>
        /// The MIDI note number of the Pitch
        /// </summary>
        public int MidiNote { get { return _noteNumber + _MIDI_C; } }

        /// <summary>
        /// The register of the Pitch in scientific notation
        /// </summary>
        public int Register { get { return _noteNumber / NUM_PC + 4; } }

        /// <summary>
        /// The Pitch number of the Pitch
        /// </summary>
        public int PitchNum
        {
            get { return _noteNumber; }
            set
            {
                _noteNumber = value;
                UpdatePitchClass(value);
            }
        }

        /// <summary>
        /// Creates a new Pitch, and initializes it to 0 (middle C)
        /// </summary>
        public Pitch()
        {
            _noteNumber = 0;
            _pitchClassInteger = 0;
        }

        /// <summary>
        /// Creates a new Pitch
        /// </summary>
        /// <param name="NoteNumber">The note number of the Pitch (middle C is 0)</param>
        public Pitch(int NoteNumber)
        {
            _noteNumber = NoteNumber;
            UpdatePitchClass(NoteNumber);
        }

        /// <summary>
        /// Creates a new Pitch
        /// </summary>
        /// <param name="pitch">An existing Pitch</param>
        public Pitch(Pitch pitch)
        {
            _noteNumber = pitch._noteNumber;
            _pitchClassInteger = pitch._pitchClassInteger;
        }

        /// <summary>
        /// Gets the hash code of the current Pitch
        /// </summary>
        /// <returns>The hash code</returns>
        public override int GetHashCode() => _noteNumber;

        /// <summary>
        /// Compares two Pitches
        /// </summary>
        /// <param name="obj">The object to compare</param>
        /// <returns>True if the two objects are equal; false otherwise</returns>
        public override bool Equals(object obj) => Equals(obj as Pitch);

        /// <summary>
        /// Compares two Pitches
        /// </summary>
        /// <param name="p">The Pitch to compare</param>
        /// <returns>True if the two Pitches are equal; false otherwise</returns>
        public bool Equals(Pitch p)
        {
            if (p is null)
                return false;
            else if (ReferenceEquals(this, p))
                return true;
            else if (_noteNumber == p._noteNumber)
                return true;
            else
                return false;
        }
        
        /// <summary>
        /// Adds two Pitches
        /// </summary>
        /// <param name="Pitch1">The first Pitch</param>
        /// <param name="Pitch2">The second Pitch</param>
        /// <returns>A new Pitch</returns>
        public static Pitch operator +(Pitch Pitch1, Pitch Pitch2) => new Pitch(Pitch1._noteNumber + Pitch2._noteNumber);

        /// <summary>
        /// Adds a Pitch and a value
        /// </summary>
        /// <param name="Pitch">A Pitch</param>
        /// <param name="val">A value</param>
        /// <returns>A new Pitch</returns>
        public static Pitch operator +(Pitch Pitch, int val) => new Pitch(Pitch._noteNumber + val);

        /// <summary>
        /// Subtracts another Pitch from a Pitch
        /// </summary>
        /// <param name="Pitch1">The first Pitch</param>
        /// <param name="Pitch2">The second Pitch</param>
        /// <returns>A new Pitch</returns>
        public static Pitch operator -(Pitch Pitch1, Pitch Pitch2) => new Pitch(Pitch1._noteNumber - Pitch2._noteNumber);

        /// <summary>
        /// Subtracts a value from a Pitch
        /// </summary>
        /// <param name="Pitch">A Pitch</param>
        /// <param name="val">A value</param>
        /// <returns>A new Pitch</returns>
        public static Pitch operator -(Pitch Pitch, int val) => new Pitch(Pitch._noteNumber - val);

        /// <summary>
        /// Multiplies two Pitches
        /// </summary>
        /// <param name="Pitch1">The first Pitch</param>
        /// <param name="Pitch2">The second Pitch</param>
        /// <returns>A new Pitch</returns>
        public static Pitch operator *(Pitch Pitch1, Pitch Pitch2) => new Pitch(Pitch1._noteNumber * Pitch2._noteNumber);

        /// <summary>
        /// Multiplies  a Pitch and a value
        /// </summary>
        /// <param name="Pitch">A Pitch</param>
        /// <param name="val">A value</param>
        /// <returns>A new Pitch</returns>
        public static Pitch operator *(Pitch Pitch, int val) => new Pitch(Pitch._noteNumber * val);

        /// <summary>
        /// Divides a Pitch by a Pitch
        /// </summary>
        /// <param name="Pitch1">The first Pitch</param>
        /// <param name="Pitch2">The second Pitch</param>
        /// <returns>A new Pitch</returns>
        public static Pitch operator /(Pitch Pitch1, Pitch Pitch2) => new Pitch(Pitch1._noteNumber / Pitch2._noteNumber);

        /// <summary>
        /// Divides a Pitch by a value
        /// </summary>
        /// <param name="Pitch">The Pitch</param>
        /// <param name="val">The value</param>
        /// <returns>A new Pitch</returns>
        public static Pitch operator /(Pitch Pitch, int val) => new Pitch(Pitch._noteNumber / val);

        /// <summary>
        /// Compares two Pitches for >
        /// </summary>
        /// <param name="Pitch1">The first Pitch</param>
        /// <param name="Pitch2">The second Pitch</param>
        /// <returns>True or false</returns>
        public static bool operator >(Pitch Pitch1, Pitch Pitch2)
        {
            if (Pitch1._noteNumber > Pitch2._noteNumber)
                return true;
            return false;
        }

        /// <summary>
        /// Compares two Pitches for >=
        /// </summary>
        /// <param name="Pitch1">The first Pitch</param>
        /// <param name="Pitch2">The second Pitch</param>
        /// <returns>True or false</returns>
        public static bool operator >=(Pitch Pitch1, Pitch Pitch2)
        {
            if (Pitch1._noteNumber >= Pitch2._noteNumber)
                return true;
            return false;
        }

        /// <summary>
        /// Compares two Pitches for <
        /// </summary>
        /// <param name="Pitch1">The first Pitch</param>
        /// <param name="Pitch2">The second Pitch</param>
        /// <returns>True or false</returns>
        public static bool operator <(Pitch Pitch1, Pitch Pitch2)
        {
            if (Pitch1._noteNumber < Pitch2._noteNumber)
                return true;
            return false;
        }

        /// <summary>
        /// Compares two Pitches for <=
        /// </summary>
        /// <param name="Pitch1">The first Pitch</param>
        /// <param name="Pitch2">The second Pitch</param>
        /// <returns>True or false</returns>
        public static bool operator <=(Pitch Pitch1, Pitch Pitch2)
        {
            if (Pitch1._noteNumber <= Pitch2._noteNumber)
                return true;
            return false;
        }

        /// <summary>
        /// Compares two Pitches for ==
        /// </summary>
        /// <param name="Pitch1">The first Pitch</param>
        /// <param name="Pitch2">The second Pitch</param>
        /// <returns>True or false</returns>
        public static bool operator ==(Pitch Pitch1, Pitch Pitch2)
        {
            if (Pitch1 is null)
            {
                if (Pitch2 is null)
                    return true;
                return false;
            }
            return Pitch1.Equals(Pitch2);
        }

        /// <summary>
        /// Compares two Pitches for !=
        /// </summary>
        /// <param name="Pitch1">The first Pitch</param>
        /// <param name="Pitch2">The second Pitch</param>
        /// <returns>True or false</returns>
        public static bool operator !=(Pitch Pitch1, Pitch Pitch2) => !(Pitch1 == Pitch2);
    }
}
