/* File: SetComplex.cs
** Project: MusicTheory
** Author: Jeffrey Martin
**
** This file contains the implementation of the SetComplex class.
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
    enum SetComplexType
    {
        K,
        Kh
    }

    class SetComplex
    {
        SortedDictionary<string, PcSetClass> _complex;
        SetComplexType _complexType;
        PcSetClass _nexusSet;
        PcSetClass _nexusSetComplement;
        Dictionary<string, string> _primeForms;
        CompareForteNames forteComparer;
        ComparePrimeFormNames primeComparer;
        int[] cardinalities;
        PrimarySetName preferForteName;

        /// <summary>
        /// An array that indicates which cardinalities are present in the set-complex
        /// </summary>
        public int[] Cardinalities { get { return cardinalities; } }

        /// <summary>
        /// Whether or not to sort by Forte names (default is false)
        /// </summary>
        public PrimarySetName PreferForteNames
        {
            get { return preferForteName; }
            set
            {
                preferForteName = value;
                if (!(_complex is null) && preferForteName == PrimarySetName.Forte)
                {
                    SortedDictionary<string, PcSetClass> newComplex = new SortedDictionary<string, PcSetClass>(forteComparer);
                    foreach (KeyValuePair<string, PcSetClass> pair in _complex)
                        newComplex.Add(pair.Value.ForteName, pair.Value);
                    _complex = newComplex;
                }
                else
                {
                    SortedDictionary<string, PcSetClass> newComplex = new SortedDictionary<string, PcSetClass>(primeComparer);
                    foreach (KeyValuePair<string, PcSetClass> pair in _complex)
                        newComplex.Add(pair.Value.PrimeFormName, pair.Value);
                    _complex = newComplex;
                }
            }
        }

        /// <summary>
        /// The set complex
        /// </summary>
        public SortedDictionary<string, PcSetClass> Complex { get { return _complex; } }

        /// <summary>
        /// The set complex type
        /// </summary>
        public SetComplexType ComplexType { get { return _complexType; } }

        /// <summary>
        /// Creates a SetComplex
        /// </summary>
        public SetComplex()
        {
            cardinalities = new int[13];
            forteComparer = new CompareForteNames();
            primeComparer = new ComparePrimeFormNames();
        }

        /// <summary>
        /// Creates a SetComplex
        /// </summary>
        /// <param name="nexusSet">The nexus set</param>
        /// <param name="setComplexType">The set complex type</param>
        public SetComplex(PcSetClass nexusSet, SetComplexType setComplexType)
        {
            cardinalities = new int[12];
            forteComparer = new CompareForteNames();
            primeComparer = new ComparePrimeFormNames();
            LoadNexusSet(nexusSet, setComplexType);
        }

        /// <summary>
        /// Gets the members of the SetComplex of a specified cardinality
        /// </summary>
        /// <param name="cardinality">The cardinality</param>
        /// <returns>The members of the SetComplex of the specified cardinality</returns>
        public SortedDictionary<string, PcSetClass> GetCardinality(int cardinality)
        {
            SortedDictionary<string, PcSetClass> cardinalityList;
            if (preferForteName == PrimarySetName.Forte)
                cardinalityList = new SortedDictionary<string, PcSetClass>(forteComparer);
            else
                cardinalityList = new SortedDictionary<string, PcSetClass>(primeComparer);
            SortedDictionary<string, PcSetClass>.Enumerator e = _complex.GetEnumerator();
            e.MoveNext();
            while (e.Current.Value.Count < cardinality)
                e.MoveNext();
            while (e.Current.Value.Count == cardinality)
            {
                cardinalityList.Add(e.Current.Key, e.Current.Value);
                e.MoveNext();
                if (e.Current.Key is null)
                    break;
            }

            return cardinalityList;
        }

        /// <summary>
        /// Checks to see if a set is in the complex
        /// </summary>
        /// <param name="set">The set</param>
        /// <returns>True if the set is in the complex; false otherwise</returns>
        public bool IsInComplex(PcSetClass set)
        {
            if (preferForteName == PrimarySetName.Forte && _complex.ContainsKey(set.ForteName))
                return true;
            else if (preferForteName == PrimarySetName.PrimeForm && _complex.ContainsKey(set.PrimeFormName))
                return true;
            return false;
        }

        /// <summary>
        /// Checks to see if a list of sets is in the complex
        /// </summary>
        /// <param name="set">The set</param>
        /// <returns>True if the set is in the complex; false otherwise</returns>
        public bool IsInComplex(List<PcSetClass> sets)
        {
            for (int i = 0; i < sets.Count; i++)
            {
                if (!_complex.ContainsKey(sets[i].PrimeFormName))
                    return false;
            }
            return true;
        }

        /// <summary>
        /// Loads a nexus set and generates a set complex
        /// </summary>
        /// <param name="nexusSet">The nexus set</param>
        /// <param name="setComplexType">The set complex type</param>
        public void LoadNexusSet(PcSetClass nexusSet, SetComplexType setComplexType)
        {
            _complexType = setComplexType;

            if (_nexusSet is null)
            {
                if (preferForteName == PrimarySetName.Forte)
                    _complex = new SortedDictionary<string, PcSetClass>(forteComparer);
                else
                    _complex = new SortedDictionary<string, PcSetClass>(primeComparer);
                _nexusSet = new PcSetClass(nexusSet);
                _nexusSetComplement = _nexusSet.GetComplement();
                _primeForms = Functions.GenerateSetTables()[0];
            }

            else
            {
                _nexusSet.LoadFromSet(nexusSet);
                _nexusSetComplement = _nexusSet.GetComplement();
            }

            if (setComplexType == SetComplexType.K)
                GenerateComplexK();
            else
                GenerateComplexKh();
        }

        /// <summary>
        /// Sets the complex type (K or Kh)
        /// </summary>
        /// <param name="setComplexType">The new complex type</param>
        public void SetType(SetComplexType setComplexType)
        {
            if (_complexType != setComplexType)
            {
                _complexType = setComplexType;
                if (_complexType == SetComplexType.K)
                    GenerateComplexK();
                else
                    GenerateComplexKh();
            }
        }

        /// <summary>
        /// Generates the complex K about the nexus set
        /// </summary>
        private void GenerateComplexK()
        {
            PcSetClass currentSet = new PcSetClass(_nexusSet);
            PcSetClass complement;

            if (_complex.Count > 0)
                _complex.Clear();
            for (int i = 0; i < cardinalities.Length; i++)
                cardinalities[i] = 0;

            foreach (string key in _primeForms.Keys)
            {
                currentSet.LoadFromPrimeFormName(key);
                complement = currentSet.GetComplement();
                if (_nexusSet.ContainsSubset(currentSet) || currentSet.ContainsSubset(_nexusSet)
                    || _nexusSet.ContainsSubset(complement) || complement.ContainsSubset(_nexusSet)
                    || _nexusSetComplement.ContainsSubset(currentSet) || currentSet.ContainsSubset(_nexusSetComplement)
                    || _nexusSetComplement.ContainsSubset(complement) || complement.ContainsSubset(_nexusSetComplement))
                {
                    if (preferForteName == PrimarySetName.Forte)
                        _complex.Add(currentSet.ForteName, new PcSetClass(currentSet));
                    else if (preferForteName == PrimarySetName.PrimeForm)
                        _complex.Add(currentSet.PrimeFormName, new PcSetClass(currentSet));
                    else
                        _complex.Add(currentSet.CarterName.ToString(), new PcSetClass(currentSet));
                    if (cardinalities[currentSet.Count] == 0)
                        cardinalities[currentSet.Count] = 1;
                }
            }
        }

        /// <summary>
        /// Generates the complex Kh about the nexus set
        /// </summary>
        private void GenerateComplexKh()
        {
            PcSetClass currentSet = new PcSetClass(_nexusSet);
            PcSetClass complement;

            if (_complex.Count > 0)
                _complex.Clear();
            for (int i = 0; i < cardinalities.Length; i++)
                cardinalities[i] = 0;

            foreach (string key in _primeForms.Keys)
            {
                currentSet.LoadFromPrimeFormName(key);
                complement = currentSet.GetComplement();
                if (((_nexusSet.ContainsSubset(currentSet) || currentSet.ContainsSubset(_nexusSet))
                    && (_nexusSetComplement.ContainsSubset(currentSet) || currentSet.ContainsSubset(_nexusSetComplement)))
                    || ((_nexusSet.ContainsSubset(complement) || complement.ContainsSubset(_nexusSet))
                    && (_nexusSetComplement.ContainsSubset(complement) || complement.ContainsSubset(_nexusSetComplement))))
                {
                    if (preferForteName == PrimarySetName.Forte)
                        _complex.Add(currentSet.ForteName, new PcSetClass(currentSet));
                    else if (preferForteName == PrimarySetName.PrimeForm)
                        _complex.Add(currentSet.PrimeFormName, new PcSetClass(currentSet));
                    else
                        _complex.Add(currentSet.CarterName.ToString(), new PcSetClass(currentSet));
                    if (cardinalities[currentSet.Count] == 0)
                        cardinalities[currentSet.Count] = 1;
                }
            }
        }
    }

    /// <summary>
    /// A comparer for set prime form names
    /// </summary>
    class ComparePrimeFormNames : IComparer<string>
    {
        public int Compare(string a, string b)
        {
            if (a.Length < b.Length)
                return -1;
            else if (a.Length > b.Length)
                return 1;
            else
                return a.CompareTo(b);
        }
    }

    /// <summary>
    /// A comparer for set Forte names
    /// </summary>
    class CompareForteNames : IComparer<string>
    {
        public int Compare(string a, string b)
        {
            string[] a_sub = a.Split('-');
            string[] b_sub = b.Split('-');
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
        }
    }
}
