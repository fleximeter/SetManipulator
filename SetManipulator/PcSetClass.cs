/* File: PcSetClass.cs
** Project: MusicTheory
** Author: Jeffrey Martin
**
** This file contains the implementation of the PcSetClass class.
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
    /// Specifies which set-class name takes precedence
    /// </summary>
    public enum PrimarySetName
    {
        Forte,
        PrimeForm
    }

    /// <summary>
    /// Represents a pitch class set
    /// </summary>
    public class PcSetClass
    {
        private HashSet<PitchClass> _pitchSet;                      // The pitch class set class
        private Dictionary<string, string> _forteToCarterNames;  // A Carter name table
        private Dictionary<string, string> _setToForteNames;     // A Forte name table
        private Dictionary<string, string> _forteToSetNames;     // A pitch set class name table
        private Dictionary<string, string> _zTable;              // A Z-relation table
        private string _setPrimeFormName;                        // The prime form name of the current set class
        private string _setForteName;                            // The Forte name of the current set class
        private int _setCarterName;                              // The Carter name of the current set class
        private bool _useLeftPacking;                            // Whether or not to calcuate the set class using packing from the left
        private string _setIcVectorName;                         // The interval class vector of the set class (as a string)
        private int[] _setIcVector;                              // The interval class vector of the set class (as an array)
        private readonly static int NUM_PC = 12;                 // The number of unique pitch classes
        private string _setType;                                 // The set class type

        /// <summary>
        /// The number of elements in the set class
        /// </summary>
        public int Count { get { return _pitchSet.Count; } }

        /// <summary>
        /// The prime form name of the set class
        /// </summary>
        public string PrimeFormName { get { return _setPrimeFormName; } }

        /// <summary>
        /// The Forte name of the set class
        /// </summary>
        public string ForteName { get { return _setForteName; } }

        /// <summary>
        /// The Carter name of the set class
        /// </summary>
        public int CarterName { get { return _setCarterName; } }

        /// <summary>
        /// The interval class vector of the set class
        /// </summary>
        public int[] ICVector { get { return _setIcVector; } }

        /// <summary>
        /// The interval class vector of the set class, as a string
        /// </summary>
        public string ICVectorName { get { return _setIcVectorName; } }

        /// <summary>
        /// The type of the set class
        /// </summary>
        public string SetType { get { return _setType; } }

        /// <summary>
        /// Creates a new empty set class
        /// </summary>
        /// <param name="nameTables">An array of name tables</param>
        public PcSetClass(Dictionary<string, string>[] nameTables)
        {
            Construct(nameTables);
        }

        /// <summary>
        /// Creates a new set class from a pitch class list
        /// </summary>
        /// <param name="pitchList">A pitch class list</param>
        /// <param name="nameTables">An array of name tables</param>
        public PcSetClass(List<PitchClass> pitchList, Dictionary<string, string>[] nameTables)
        {
            Construct(nameTables);
            LoadFromPitchClassList(pitchList);
        }

        /// <summary>
        /// Creates a new set class from an existing set class
        /// </summary>
        /// <param name="set">An existing set class</param>
        public PcSetClass(PcSetClass set)
        {
            _setToForteNames = set._setToForteNames;
            _forteToSetNames = set._forteToSetNames;
            _forteToCarterNames = set._forteToCarterNames;
            _zTable = set._zTable;
            _pitchSet = new HashSet<PitchClass>();
            _setIcVector = new int[7];
            _setPrimeFormName = set._setPrimeFormName;
            _setForteName = set._setForteName;
            _setCarterName = set._setCarterName;
            _useLeftPacking = set._useLeftPacking;
            _setIcVectorName = set._setIcVectorName;
            _setType = set._setType;
            foreach (PitchClass pc in set._pitchSet)
                _pitchSet.Add(new PitchClass(pc));
            for (int i = 0; i < set._setIcVector.Length; i++)
                _setIcVector[i] = set._setIcVector[i];
        }

        /// <summary>
        /// Creates a new set class
        /// </summary>
        /// <param name="pitchClasses">A list of pitch classes</param>
        /// <param name="set">An existing set class</param>
        private PcSetClass(HashSet<PitchClass> pitchSet, PcSetClass set)
        {
            _setToForteNames = set._setToForteNames;
            _forteToSetNames = set._forteToSetNames;
            _forteToCarterNames = set._forteToCarterNames;
            _zTable = set._zTable;
            _pitchSet = new HashSet<PitchClass>();
            _setIcVector = new int[7] { 0, 0, 0, 0, 0, 0, 0 };
            _setPrimeFormName = "";
            _setForteName = "";
            _setCarterName = 0;
            _useLeftPacking = false;
            _setIcVectorName = "";
            _setType = "";
            LoadFromPitchClassSet(pitchSet);
        }

        /// <summary>
        /// Calculates the ANGLE between two set classes, based on interval class vector
        /// (Damon Scott and Eric J. Isaacson, "The Interval Angle: A Similarity Measure
        /// for Pitch-Class Sets," Perspectives of New Music 36:2 (Summer, 1998), 107-142)
        /// </summary>
        /// <param name="set1">A set class</param>
        /// <param name="set2">A set class</param>
        /// <returns>The ANGLE between the two set classes, in radians</returns>
        public static double CalculateAngle(PcSetClass set1, PcSetClass set2)
        {
            int[] ic1 = set1.ICVector;
            int[] ic2 = set2.ICVector;
            return System.Math.Acos(
                (ic1[0] * ic2[0] + ic1[1] * ic2[1] + ic1[2] * ic2[2] + ic1[3] * ic2[3] + ic1[4] * ic2[4] + ic1[5] * ic2[5])
                / (System.Math.Sqrt(System.Math.Pow(ic1[0], 2) + System.Math.Pow(ic1[1], 2) + System.Math.Pow(ic1[2], 2)
                + System.Math.Pow(ic1[3], 2) + System.Math.Pow(ic1[4], 2) + System.Math.Pow(ic1[5], 2))
                * System.Math.Sqrt(System.Math.Pow(ic2[0], 2) + System.Math.Pow(ic2[1], 2) + System.Math.Pow(ic2[2], 2)
                + System.Math.Pow(ic2[3], 2) + System.Math.Pow(ic2[4], 2) + System.Math.Pow(ic2[5], 2))));
        }

        /// <summary>
        /// Calculates the index vector of a list of pitch classes. Note that this does not
        /// perform any validation - you must call IsValidSet() first if you need to validate
        /// the list.
        /// </summary>
        /// <param name="listToVector">The list to calculate the index vector</param>
        /// <returns>The index vector</returns>
        public static int[] CalculateIndexVector(List<PitchClass> listToVector)
        {
            int[] indexVector = new int[12] { 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0 };
            for (int i = 0; i < listToVector.Count; i++)
            {
                for(int j = 0; j < listToVector.Count; j++)
                    indexVector[(listToVector[i].PitchClassInteger + listToVector[j].PitchClassInteger) % NUM_PC]++;
            }
            return indexVector;
        }

        /// <summary>
        /// Removes all elements from the set class
        /// </summary>
        public void Clear()
        {
            if (_pitchSet != null)
                _pitchSet.Clear();
        }

        /// <summary>
        /// Handles common constructor functionality
        /// </summary>
        /// <param name="nameTables">A dictionary array of name tables</param>
        private void Construct(Dictionary<string, string>[] nameTables)
        {
            _setToForteNames = nameTables[0];
            _forteToSetNames = nameTables[1];
            _forteToCarterNames = nameTables[2];
            _zTable = nameTables[3];
            _pitchSet = new HashSet<PitchClass>();
            _setIcVector = new int[7] { 0, 0, 0, 0, 0, 0, 0 };
            _setPrimeFormName = "";
            _setForteName = "";
            _setCarterName = 0;
            _useLeftPacking = false;
            _setIcVectorName = "";
            _setType = "";
        }

        /// <summary>
        /// Checks to see if the set class contains a specific pitch class
        /// </summary>
        /// <param name="pitchClass">The pitch class to look for</param>
        /// <returns>True if the set class contains the pitch class; false otherwise</returns>
        public bool Contains(PitchClass pitchClass) => _pitchSet.Contains(pitchClass);

        /// <summary>
        /// Checks to see if the set class contains a specific subset
        /// </summary>
        /// <param name="subset">The subset class</param>
        /// <returns>True if the set class contains the subset class; false otherwise</returns>
        public bool ContainsSubset(PcSetClass subset)
        {
            if (subset is null)
                return true;
            else if (subset._pitchSet.Count == 0)
                return true;
            else if (subset.Count > _pitchSet.Count)
                return false;
            else if (subset.Count == _pitchSet.Count)
            {
                if (subset.ForteName == _setForteName)
                    return true;
                return false;
            }
            else
            {
                HashSet<PitchClass> transpose;
                HashSet<PitchClass> invert;
                for (int i = 0; i < NUM_PC; i++)
                {
                    transpose = PcSet.Transpose(subset._pitchSet, i);
                    invert = PcSet.Invert(subset._pitchSet);
                    if (_pitchSet.IsSupersetOf(transpose) || _pitchSet.IsSupersetOf(invert))
                        return true;
                }

                return false;
            }
        }

        /// <summary>
        /// Compares two set classes for equality
        /// </summary>
        /// <param name="obj">The set class</param>
        /// <returns>True if the set classes are equal; false otherwise</returns>
        public override bool Equals(object obj) => Equals(obj as PcSetClass);

        /// <summary>
        /// Compares two set classes for equality
        /// </summary>
        /// <param name="set">The set class</param>
        /// <returns>True if the set classes are equal; false otherwise</returns>
        public bool Equals(PcSetClass set) => _pitchSet.Equals(set._pitchSet);

        /// <summary>
        /// Finds the Tn or TnI transformation(s) that produced the provided list of pcs
        /// </summary>
        /// <param name="pitchList">The pitch list to analyze</param>
        /// <returns>A list of matching set names</returns>
        public List<string> FindTransformationName(List<PitchClass> pitchList)
        {
            List<string> transposeNames = new List<string>();
            List<string> invertNames = new List<string>();

            if (pitchList.Count == _pitchSet.Count)
            {
                // Exhaustively compute all possible transformations and match them against the provided pitch class list
                for (int i = 0; i < NUM_PC; i++)
                {
                    bool transposeMatch = true;
                    bool invertMatch = true;
                    HashSet<PitchClass> transpose = Transpose(i);
                    HashSet<PitchClass> invert = InvertTranspose(i);
                    foreach (PitchClass pc in pitchList)
                    {
                        if (!transpose.Contains(pc))
                        {
                            transposeMatch = false;
                            break;
                        }    
                    }
                    foreach (PitchClass pc in pitchList)
                    {
                        if (!invert.Contains(pc))
                        {
                            invertMatch = false;
                            break;
                        }
                    }
                    // Note that multiple transformations may match the provided list
                    if (transposeMatch)
                        transposeNames.Add('T' + i.ToString());
                    if (invertMatch)
                        invertNames.Add('T' + i.ToString() + 'I');
                }
            }

            transposeNames.AddRange(invertNames);
            return transposeNames;
        }

        /// <summary>
        /// Gets the complement PcSetClass. Note that this is an abstract complement.
        /// </summary>
        /// <returns>The complement</returns>
        public PcSetClass GetComplement()
        {
            PcSetClass complement;
            HashSet<PitchClass> pitches = new HashSet<PitchClass>();
            for (int i = 0; i < NUM_PC; i++)
            {
                PitchClass pc = new PitchClass(i);
                if (!_pitchSet.Contains(pc))
                   pitches.Add(pc);
            }
            complement = new PcSetClass(pitches, this);
            return complement;
        }

        /// <summary>
        /// Gets the hash code of the PcSetClass
        /// </summary>
        /// <returns>The hash code</returns>
        public override int GetHashCode()
        {
            if (_pitchSet is null)
                return 0;
            else if (_pitchSet.Count == 0)
                return 0;
            else
            {
                int code = 0;
                foreach (PitchClass pc in _pitchSet)
                    code ^= pc.PitchClassInteger;
                return code;
            }
        }

        /// <summary>
        /// Gets a collection of secondary forms that contain a provided collection of pitches
        /// </summary>
        /// <param name="pitchClasses">The pitch classes to search for</param>
        /// <returns>The secondary forms</returns>
        public List<Pair<string, HashSet<PitchClass>>> GetSecondaryForms(List<PitchClass> pitchClasses)
        {
            List<Pair<string, HashSet<PitchClass>>> secondaryForms = new List<Pair<string, HashSet<PitchClass>>>();

            // The cardinality of pitchClasses cannot be greater than the cardinality of the set
            if (pitchClasses.Count <= _pitchSet.Count)
            {
                // Brute-force all Tn- and TnI-related pcsets. Note that the provided pcs may match multiple related pcsets.
                for (int i = 0; i < NUM_PC; i++)
                {
                    HashSet<PitchClass> transpose = PcSet.Transpose(_pitchSet, i);
                    HashSet<PitchClass> invert = PcSet.Transpose(PcSet.Invert(_pitchSet), i);
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
        /// Gets a copy of the pc-set-class prime form.
        /// </summary>
        /// <returns>A pc set</returns>
        public HashSet<PitchClass> GetSetCopy()
        {
            HashSet<PitchClass> set = new HashSet<PitchClass>();
            foreach (PitchClass pc in _pitchSet)
                set.Add(new PitchClass(pc));
            return set;
        }

        /// <summary>
        /// Gets all subsets of the current set
        /// </summary>
        /// <returns>A list of subsets</returns>
        public List<Pair<PcSetClass, int>> GetSubsets(PrimarySetName namePreference = PrimarySetName.PrimeForm)
        {
            List<Pair<PcSetClass, int>> subsets = new List<Pair<PcSetClass, int>>();
            Dictionary<string, int> foundSets = new Dictionary<string, int>();
            List<HashSet<PitchClass>> store = new List<HashSet<PitchClass>>();
            HashSet<PitchClass> build = new HashSet<PitchClass>();
            List<PitchClass> remaining = new List<PitchClass>(_pitchSet);

            // Get the subsets
            for (int i = 1; i < _pitchSet.Count; i++)
                GetSubsetsHelper(store, build, remaining, 0, i);

            // Remove duplicate set-classes
            for (int i = 0; i < store.Count; i++)
            {
                PcSetClass set = new PcSetClass(store[i], this);
                if (!foundSets.ContainsKey(set._setPrimeFormName))
                {
                    subsets.Add(new Pair<PcSetClass, int>(set, 1));
                    foundSets[set._setPrimeFormName] = subsets.Count - 1;
                }
                else
                    subsets[foundSets[set._setPrimeFormName]].Item2++;
            }

            // Sort by set-class name preference
            if (namePreference == PrimarySetName.Forte)
            {
                subsets.Sort((a, b) =>
                {
                    string[] a_sub = a.Item1._setForteName.Split('-');
                    string[] b_sub = b.Item1._setForteName.Split('-');
                    if (a_sub[1][0] == 'Z')
                        a_sub[1] = a_sub[1].Trim('Z');
                    if (b_sub[1][0] == 'Z')
                        b_sub[1] = b_sub[1].Trim('Z');
                    if (a_sub[0].Length < b_sub[0].Length)
                        return -1;
                    else if (a_sub[0].Length > b_sub[0].Length)
                        return 1;
                    else if (a_sub[0] != b_sub[0])
                        return a_sub[0].CompareTo(b_sub[0]);
                    else
                    {
                        if (a_sub[1].Length < b_sub[1].Length)
                            return -1;
                        else if (a_sub[1].Length > b_sub[1].Length)
                            return 1;
                        else
                            return a_sub[1].CompareTo(b_sub[1]);
                    }
                });
            }

            else
            {
                subsets.Sort((a, b) =>
                {
                    if (a.Item1.Count < b.Item1.Count)
                        return -1;
                    else if (a.Item1.Count > b.Item1.Count)
                        return 1;
                    else
                        return a.Item1._setPrimeFormName.CompareTo(b.Item1._setPrimeFormName);
                });
            }

            // Insert the null set at the beginning of the list of subsets
            subsets.Insert(0, new Pair<PcSetClass, int>(new PcSetClass(new HashSet<PitchClass>(), this), 1));

            return subsets;
        }

        /// <summary>
        /// Gets all subsets of the current set of a specified cardinality
        /// </summary>
        /// <param name="cardinality">The cardinality of the subsets</param>
        /// <param name="sortByForte">Whether or not to sort by Forte name</param>
        /// <returns>A list of subsets</returns>
        public List<Pair<PcSetClass, int>> GetSubsets(int cardinality, PrimarySetName namePreference = PrimarySetName.PrimeForm)
        {
            List<Pair<PcSetClass, int>> subsets = new List<Pair<PcSetClass, int>>();
            Dictionary<string, int> foundSets = new Dictionary<string, int>();
            List<HashSet<PitchClass>> store = new List<HashSet<PitchClass>>();
            HashSet<PitchClass> build = new HashSet<PitchClass>();
            List<PitchClass> remaining = new List<PitchClass>(_pitchSet);

            // If the cardinality is 0, we insert the null set and are done
            if (cardinality == 0)
            {
                subsets.Add(new Pair<PcSetClass, int>(new PcSetClass(new HashSet<PitchClass>(), this), 1));
            }

            // Otherwise we need to find the subsets
            else
            {
                // Get the subsets
                GetSubsetsHelper(store, build, remaining, 0, cardinality);

                // Remove duplicate set-classes
                for (int i = 0; i < store.Count; i++)
                {
                    PcSetClass set = new PcSetClass(store[i], this);
                    if (!foundSets.ContainsKey(set._setPrimeFormName))
                    {
                        subsets.Add(new Pair<PcSetClass, int>(set, 1));
                        foundSets[set._setPrimeFormName] = subsets.Count - 1;
                    }
                    else
                        subsets[foundSets[set._setPrimeFormName]].Item2++;
                }

                // Sort by set-class name preference
                if (namePreference == PrimarySetName.Forte)
                {
                    subsets.Sort((a, b) =>
                    {
                        string[] a_sub = a.Item1._setForteName.Split('-');
                        string[] b_sub = b.Item1._setForteName.Split('-');
                        if (a_sub[1][0] == 'Z')
                            a_sub[1] = a_sub[1].Trim('Z');
                        if (b_sub[1][0] == 'Z')
                            b_sub[1] = b_sub[1].Trim('Z');
                        if (a_sub[0].Length < b_sub[0].Length)
                            return -1;
                        else if (a_sub[0].Length > b_sub[0].Length)
                            return 1;
                        else if (a_sub[0] != b_sub[0])
                            return a_sub[0].CompareTo(b_sub[0]);
                        else
                        {
                            if (a_sub[1].Length < b_sub[1].Length)
                                return -1;
                            else if (a_sub[1].Length > b_sub[1].Length)
                                return 1;
                            else
                                return a_sub[1].CompareTo(b_sub[1]);
                        }
                    });
                }

                else
                {
                    subsets.Sort((a, b) =>
                    {
                        if (a.Item1.Count < b.Item1.Count)
                            return -1;
                        else if (a.Item1.Count > b.Item1.Count)
                            return 1;
                        else
                            return a.Item1._setPrimeFormName.CompareTo(b.Item1._setPrimeFormName);
                    });
                }
            }

            return subsets;
        }

        /// <summary>
        /// A helper for GetSubsets
        /// </summary>
        /// <param name="store">The list to store completed subsets</param>
        /// <param name="build">The current subset we are building</param>
        /// <param name="remaining">The remaining pitch classes from which to choose</param>
        /// <param name="selected">The number of pitch classes already chosen</param>
        /// <param name="max">The maximum number of pitch classes to choose</param>
        private void GetSubsetsHelper(List<HashSet<PitchClass>> store, HashSet<PitchClass> build, List<PitchClass> remaining, int selected, int max)
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
                    GetSubsetsHelper(store, newBuild, newRemaining, selected + 1, max);
                }
            }
        }

        /// <summary>
        /// Gets the Z-related set-class
        /// </summary>
        /// <returns>The Z-related set-class</returns>
        public PcSetClass GetZRelation()
        {
            PcSetClass zSet = new PcSetClass(this);
            if (_setForteName.Contains('Z'))
            {
                zSet.LoadFromForteName(_zTable[_setForteName]);
            }

            else
                throw new KeyNotFoundException("No Z-relation exists for this set-class.");

            return zSet;
        }

        /// <summary>
        /// Inverts and transposes the prime form
        /// </summary>
        /// <param name="numberOfTranspositions">The number of transpositions</param>
        /// <returns>An inverted and transposed version of the prime form</returns>
        public HashSet<PitchClass> InvertTranspose(int numberOfTranspositions)
        {
            HashSet<PitchClass> invertTranspose = PcSet.Transpose(PcSet.Invert(_pitchSet), numberOfTranspositions);
            return invertTranspose;
        }

        /// <summary>
        /// Determines if a list of pitch classes forms a valid pcset
        /// </summary>
        /// <param name="pitches">The list of pitch classes to analyze</param>
        /// <returns>True if the list is a valid pcset; False otherwise</returns>
        public static bool IsValidSet(List<int> pitches)
        {
            bool validSet = true;  // Whether or not the pcset is valid
            Dictionary<int, char> usedPitches = new Dictionary<int, char>();  // A list of used pitches

            // If the list does not have enough pitches in it, the list is invalid
            if (pitches.Count > 12)
            {
                validSet = false;
            }

            else
            {
                // Cycle through the pitch list to make sure there are no duplicate pitch classes
                foreach (int i in pitches)
                {
                    // If the current pitch class is not a duplicate, add it to the list of used pitches.
                    if (!usedPitches.ContainsKey(i))
                        usedPitches[i] = '1';

                    // Otherwise the pcset is invalid and we can't use it.
                    else
                        validSet = false;
                }
            }

            return validSet;
        }

        /// <summary>
        /// Determines if a list of pitch classes forms a valid pcset
        /// </summary>
        /// <param name="pitches">The list of pitch classes to analyze</param>
        /// <returns>True if the list is a valid pcset; False otherwise</returns>
        public static bool IsValidSet(List<PitchClass> pitches)
        {
            bool validSet = true;  // Whether or not the pcset is valid
            Dictionary<int, char> usedPitches = new Dictionary<int, char>();  // A list of used pitches

            // If the list does not have enough pitches in it, the list is invalid
            if (pitches.Count > 12)
            {
                validSet = false;
            }

            else
            {
                // Cycle through the pitch list to make sure there are no duplicate pitch classes
                foreach (PitchClass i in pitches)
                {
                    // If the current pitch class is not a duplicate, add it to the list of used pitches.
                    if (!usedPitches.ContainsKey(i.PitchClassInteger))
                        usedPitches[i.PitchClassInteger] = '1';

                    // Otherwise the pcset is invalid and we can't use it.
                    else
                        validSet = false;
                }
            }

            return validSet;
        }

        /// <summary>
        /// Determines if a set-class name is valid. The name may be either a Forte name or prime form
        /// name. If it is a prime form name, it may be enclosed in square brackets [] or
        /// parentheses ().
        /// </summary>
        /// <param name="name">The set-class name to validate</param>
        /// <returns>True if the set-class name is valid; false otherwise</returns>
        public bool IsValidName(string name)
        {
            name = name.ToUpper();

            // The name must be between 1 and 14 characters
            if (name.Length < 1 || name.Length > 14)
                return false;

            if (name == "null" || name == "NULL")
                return true;

            // If there is a '-' in the name, it must be a Forte name
            else if (name.Contains('-'))
            {
                if (_forteToSetNames.ContainsKey(name))
                    return true;
                else
                    return false;
            }

            // Otherwise, it must be an integer name
            else
            {
                // We permit enclosing brackets or parentheses
                if ((name[0] == '[' && name[name.Length - 1] == ']') || (name[0] == '(' && name[name.Length - 1] == ')'))
                {
                    name.Remove(name.Length - 1, 1);
                    name.Remove(0, 1);
                }

                // Look up the set name in the table
                if (_setToForteNames.ContainsKey(name))
                    return true;
                else
                    return false;
            }
        }

        /// <summary>
        /// Loads a set-class from a Forte name. If the name is invalid, no changes are made to the set.
        /// </summary>
        /// <param name="forteName">The Forte name</param>
        public void LoadFromForteName(string forteName)
        {
            forteName = forteName.ToUpper();
            if (_forteToSetNames.ContainsKey(forteName))
                LoadFromPrimeFormName(_forteToSetNames[forteName]);
        }

        /// <summary>
        /// Loads the set from a pitch class list. Note that this does not perform validation
        /// on the pitch class list! If you want to validate the list, you will have to call
        /// IsValidSet() first.
        /// </summary>
        /// <param name="pitchClassList">The pitch class list to load</param>
        public void LoadFromPitchClassList(List<PitchClass> pitchClassList)
        {
            // Only one set-class exists for sets of cardinality 0, 11, and 12.
            // No further calculation is necessary.
            if (pitchClassList.Count == 0)
                LoadNullSet();
            else if (pitchClassList.Count == 11)
            {
                _pitchSet.Clear();
                for (int i = 0; i < 11; i++)
                    _pitchSet.Add(new PitchClass(i));
                UpdateSetNames();
            }
            else if (pitchClassList.Count == 12)
            {
                _pitchSet.Clear();
                for (int i = 0; i < 12; i++)
                    _pitchSet.Add(new PitchClass(i));
                UpdateSetNames();
            }

            // In all other cases, we must calculate the prime form of the set-class.
            else
            {
                _pitchSet = CalculatePrimeForm(PcSeg.ToPcSet(pitchClassList));
                UpdateSetNames();
            }
        }

        /// <summary>
        /// Loads the set from a pcset. No validation is necessary because the HashSet
        /// type does not allow duplicates.
        /// </summary>
        /// <param name="pcset">A pcset</param>
        public void LoadFromPitchClassSet(HashSet<PitchClass> pcset)
        {
            // Only one set-class exists for sets of cardinality 0, 11, and 12.
            // No further calculation is necessary.
            if (pcset.Count == 0)
                LoadNullSet();
            else if (pcset.Count == 11)
            {
                _pitchSet.Clear();
                for (int i = 0; i < 11; i++)
                    _pitchSet.Add(new PitchClass(i));
                UpdateSetNames();
            }
            else if (pcset.Count == 12)
            {
                _pitchSet.Clear();
                for (int i = 0; i < 12; i++)
                    _pitchSet.Add(new PitchClass(i));
                UpdateSetNames();
            }

            // In all other cases, we must calculate the prime form of the set-class.
            else
            {
                _pitchSet = CalculatePrimeForm(pcset);
                UpdateSetNames();
            }
        }

        /// <summary>
        /// Loads a set from a prime form name. If the name is invalid, no changes are made to the set.
        /// </summary>
        /// <param name="primeFormName">The prime form name</param>
        public void LoadFromPrimeFormName(string primeFormName)
        {
            HashSet<PitchClass> pitches = new HashSet<PitchClass>();
            primeFormName = primeFormName.ToUpper();
            if (IsValidName(primeFormName))
            {
                foreach (char letter in primeFormName)
                    pitches.Add(new PitchClass(letter));
                LoadFromPitchClassSet(pitches);
            }
        }

        /// <summary>
        /// Loads a set-class from an existing set-class
        /// </summary>
        /// <param name="sc">The existing PcSetClass</param>
        public void LoadFromSet(PcSetClass sc)
        {
            _pitchSet.Clear();
            _setPrimeFormName = sc._setPrimeFormName;
            _setForteName = sc._setForteName;
            _useLeftPacking = sc._useLeftPacking;
            _setIcVectorName = sc._setIcVectorName;
            _setType = sc._setType;
            foreach (PitchClass pc in sc._pitchSet)
                _pitchSet.Add(new PitchClass(pc));
            for (int i = 0; i < sc._setIcVector.Length; i++)
                _setIcVector[i] = sc._setIcVector[i];
        }

        /// <summary>
        /// Loads the null set-class
        /// </summary>
        public void LoadNullSet()
        {
            _pitchSet.Clear();
            _setType = "Null Set";
            _setPrimeFormName = "null";
            _setForteName = "null";
            _setCarterName = 0;
            _setIcVectorName = "[0000000]";
            for (int i = 0; i < _setIcVector.Length; i++)
                _setIcVector[i] = 0;
        }

        /// <summary>
        /// Transforms the pc set-class. Note that this function will only behave as expected 
        /// if valid transformation names are provided - names can be validated using IsValidTransformation().
        /// </summary>
        /// <param name="transformationName">The transformation name</param>
        /// <returns>A transformed version of the set-class</returns>
        public HashSet<PitchClass> Transform(string transformationString)
        {
            List<Pair<char, int>> opList = new List<Pair<char, int>>();
            HashSet<PitchClass> pcList = _pitchSet;
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
                    pcList = PcSet.Transpose(pcList, pair.Item2);
                else if (pair.Item1 == 'I')
                    pcList = PcSet.Invert(pcList);
                else if (pair.Item1 == 'M')
                    pcList = PcSet.Multiply(pcList, pair.Item2);
            }

            return pcList;
        }

        /// <summary>
        /// Transposes the prime form
        /// </summary>
        /// <param name="numberOfTranspositions">The number of transpositions</param>
        /// <returns>A transposed version of the prime form</returns>
        public HashSet<PitchClass> Transpose(int numberOfTranspositions)
        {
            HashSet<PitchClass> transposed = PcSet.Transpose(_pitchSet, numberOfTranspositions);
            return transposed;
        }

        /// <summary>
        /// Compares two PcSetClasses for equality
        /// </summary>
        /// <param name="set1">A set-class</param>
        /// <param name="set2">A set-class</param>
        /// <returns>True if the two set-classes are equal; false otherwise</returns>
        public static bool operator ==(PcSetClass set1, PcSetClass set2)
        {
            if (set1 is null)
            {
                if (set2 is null)
                    return true;
                return false;
            }
            else if (set1._pitchSet is null)
            {
                if (set2._pitchSet is null)
                    return true;
                return false;
            }
            else
            {
                return set1.Equals(set2);
            }
        }

        /// <summary>
        /// Compares two PcSetClasses for inequality
        /// </summary>
        /// <param name="set1">A set-class</param>
        /// <param name="set2">A set-class</param>
        /// <returns>True if the two set-classes are inequal; false otherwise</returns>
        public static bool operator !=(PcSetClass set1, PcSetClass set2) => !(set1 == set2);

        /// <summary>
        /// Calculates the prime form of a provided set of pitch classes
        /// </summary>
        /// <param name="pitchClassList">The pitch class list to evaluate</param>
        /// <returns>The prime form of the set-class</returns>
        private HashSet<PitchClass> CalculatePrimeForm(HashSet<PitchClass> pitchClassList)
        {
            List<List<PitchClass>> listsToWeight = new List<List<PitchClass>>(2 * pitchClassList.Count);
            List<PitchClass> pitchClasses = PcSet.ToPcSeg(pitchClassList);
            List<PitchClass> inverted = PcSeg.Invert(pitchClasses);
            List<PitchClass> final;
            HashSet<PitchClass> set = new HashSet<PitchClass>();

            // Add regular forms to the lists to weight
            for (int i = 0; i < pitchClassList.Count; i++)
            {
                listsToWeight.Add(new List<PitchClass>(pitchClassList.Count));
                for (int i2 = i; i2 < pitchClassList.Count; i2++)
                    listsToWeight[i].Add(new PitchClass(pitchClasses[i2]));
                for (int i2 = 0; i2 < i; i2++)
                    listsToWeight[i].Add(new PitchClass(pitchClasses[i2]));
                int initialPitch = listsToWeight[i][0].PitchClassInteger;
                for (int j = 0; j < listsToWeight[i].Count; j++)
                    listsToWeight[i][j].PitchClassInteger -= initialPitch;
                listsToWeight[i].Sort((a, b) => a.PitchClassInteger.CompareTo(b.PitchClassInteger));
            }

            // Add inverted forms to the lists to weight
            for (int i = 0; i < inverted.Count; i++)
            {
                listsToWeight.Add(new List<PitchClass>(pitchClassList.Count));
                for (int i2 = i; i2 < inverted.Count; i2++)
                    listsToWeight[i + pitchClassList.Count].Add(new PitchClass(inverted[i2]));
                for (int i2 = 0; i2 < i; i2++)
                    listsToWeight[i + pitchClassList.Count].Add(new PitchClass(inverted[i2]));
                int initialPitch = listsToWeight[i + pitchClassList.Count][0].PitchClassInteger;
                for (int j = 0; j < listsToWeight[i + pitchClassList.Count].Count; j++)
                    listsToWeight[i + pitchClassList.Count][j].PitchClassInteger -= initialPitch;
                listsToWeight[i + pitchClassList.Count].Sort((a, b) => a.PitchClassInteger.CompareTo(b.PitchClassInteger));
            }

            // If packing from the left (Forte)
            if (_useLeftPacking)
            {
                int index = 0;
                final = WeightLeft(listsToWeight, ref index);
            }

            // If packing from the right (Regener/Rahn)
            else
            {
                int index = pitchClassList.Count - 1;
                final = WeightFromRight(listsToWeight, ref index);
            }

            foreach (PitchClass pc in final)
                set.Add(pc);
            return set;
        }

        /// <summary>
        /// Updates the set names. Only call this method when the set is in prime form at T0.
        /// </summary>
        private void UpdateSetNames()
        {
            // First clear the existing names
            _setPrimeFormName = "";
            _setIcVectorName = "";
            _setForteName = "";
            for (int i = 0; i < _setIcVector.Length; i++)
                _setIcVector[i] = 0;

            // Calculate the prime form name by listing all of the pitches in the set
            // (remember, the set must be in prime form at T0 for this to work!)
            System.Text.StringBuilder name = new System.Text.StringBuilder(NUM_PC);
            List<PitchClass> pitches = PcSet.ToSortedPcSeg(_pitchSet);
            foreach (PitchClass pc in pitches)
                name.Append(pc.PitchClassChar);
            _setPrimeFormName = name.ToString();

            // Get the set interval class vector
            foreach (PitchClass pc1 in _pitchSet)
            {
                foreach (PitchClass pc2 in _pitchSet)
                {
                    int interval = (pc2 - pc1).PitchClassInteger;
                    if (interval > NUM_PC / 2)
                        interval = interval * (-1) + NUM_PC;
                    _setIcVector[interval]++;
                }
            }
            for (int i = 1; i < 7; i++)
                _setIcVector[i] /= 2;

            // Update the set type
            switch (_pitchSet.Count)
            {
                case 0:
                    _setType = "Null";
                    break;
                case 1:
                    _setType = "PC";
                    break;
                case 2:
                    _setType = "Dyad";
                    break;
                case 3:
                    _setType = "Trichord";
                    break;
                case 4:
                    _setType = "Tetrachord";
                    break;
                case 5:
                    _setType = "Pentachord";
                    break;
                case 6:
                    _setType = "Hexachord";
                    break;
                case 7:
                    _setType = "Heptachord";
                    break;
                case 8:
                    _setType = "Octachord";
                    break;
                case 9:
                    _setType = "Nonachord";
                    break;
                case 10:
                    _setType = "Decachord";
                    break;
                case 11:
                    _setType = "Hendecachord";
                    break;
                default:
                    _setType = "Dodecachord";
                    break;
            }

            _setForteName = _setToForteNames[_setPrimeFormName];
            if (_pitchSet.Count > 1 && _pitchSet.Count < 11)
                _setCarterName = int.Parse(_forteToCarterNames[_setForteName]);
            else
                _setCarterName = 0;
            _setIcVectorName = "[" + Functions.HexChars[_setIcVector[0]] + Functions.HexChars[_setIcVector[1]] + Functions.HexChars[_setIcVector[2]] + Functions.HexChars[_setIcVector[3]] + Functions.HexChars[_setIcVector[4]] + Functions.HexChars[_setIcVector[5]] + Functions.HexChars[_setIcVector[6]] + ']';
        }

        /// <summary>
        /// Weights a list of pitch classes from the right
        /// </summary>
        /// <param name="listsToWeight">The lists to weight</param>
        /// <param name="currentIndex">The current index</param>
        /// <returns>The most weighted form</returns>
        private List<PitchClass> WeightFromRight(List<List<PitchClass>> listsToWeight, ref int currentIndex)
        {
            // If we've narrowed it down to one list, return that list
            if (listsToWeight.Count == 1 || currentIndex == 0)
            {
                return listsToWeight[0];
            }

            // Otherwise, we need to keep narrowing down the lists
            else
            {
                int smallestItem = 11;                                        // The smallest pitch integer we've found at the current index
                List<int> validIndices = new List<int>(listsToWeight.Count);  // Valid indices that indicate lists we will pass to the next recursive call
                
                // Cycle through the lists and determine which indices contain the lowest pitch class.
                // Those lists will be passed to the next recursive call.
                for (int i = 0; i < listsToWeight.Count; i++)
                {
                    // If we've found an even lower pitch, clear the list of indices and start with this one
                    if (listsToWeight[i][currentIndex].PitchClassInteger < smallestItem)
                    {
                        smallestItem = listsToWeight[i][currentIndex].PitchClassInteger;
                        validIndices.Clear();
                        validIndices.Add(i);
                    }

                    // Or, if we've found an additional occurrence of the lowest pitch,
                    // add the current index to the list of indices
                    else if (listsToWeight[i][currentIndex].PitchClassInteger == smallestItem)
                    {
                        validIndices.Add(i);
                    }
                }

                // Eliminate all invalid permutations
                int currentSmallestItem = validIndices.Count - 1;
                for (int i = listsToWeight.Count - 1; i >= 0; i--)
                {
                    if (currentSmallestItem < 0)
                        listsToWeight.RemoveAt(i);
                    else if (i > validIndices[currentSmallestItem])
                        listsToWeight.RemoveAt(i);
                    else if (i == validIndices[currentSmallestItem])
                        currentSmallestItem--;
                }

                // Perform the next recursive call
                currentIndex--;
                return WeightFromRight(listsToWeight, ref currentIndex);
            }
        }

        /// <summary>
        /// Weights a list of pitch classes to the left
        /// </summary>
        /// <param name="listsToWeight">The lists to weight</param>
        /// <param name="currentIndex">The current index</param>
        /// <returns>The most weighted form</returns>
        private List<PitchClass> WeightLeft(List<List<PitchClass>> listsToWeight, ref int currentIndex)
        {
            // If we've narrowed it down to one list, return that list
            if (listsToWeight.Count == 1 || currentIndex == listsToWeight[0].Count - 1)
            {
                return listsToWeight[0];
            }

            // Otherwise, we need to keep narrowing down the lists
            else
            {
                int smallestItem = 11;                                        // The smallest pitch integer we've found at the current index
                List<int> validIndices = new List<int>(listsToWeight.Count);  // Valid indices that indicate lists we will pass to the next recursive call
                
                // Cycle through the lists and determine which indices contain the lowest pitch class.
                // Those lists will be passed to the next recursive call.
                for (int i = 0; i < listsToWeight.Count; i++)
                {
                    // If we've found an even lower pitch, clear the list of indices and start with this one
                    if (listsToWeight[i][currentIndex].PitchClassInteger < smallestItem)
                    {
                        smallestItem = listsToWeight[i][currentIndex].PitchClassInteger;
                        validIndices.Clear();
                        validIndices.Add(i);
                    }

                    // Or, if we've found an additional occurrence of the lowest pitch,
                    // add the current index to the list of indices
                    else if (listsToWeight[i][currentIndex].PitchClassInteger == smallestItem)
                    {
                        validIndices.Add(i);
                    }
                }

                // Eliminate all invalid permutations
                int currentSmallestItem = validIndices.Count - 1;
                for (int i = listsToWeight.Count - 1; i >= 0; i--)
                {
                    if (currentSmallestItem < 0)
                        listsToWeight.RemoveAt(i);
                    if (i > validIndices[currentSmallestItem])
                        listsToWeight.RemoveAt(i);
                    else if (i == validIndices[currentSmallestItem])
                        currentSmallestItem--;
                }

                // Perform the next recursive call
                currentIndex++;
                return WeightLeft(listsToWeight, ref currentIndex);
            }
        }
    }
}
