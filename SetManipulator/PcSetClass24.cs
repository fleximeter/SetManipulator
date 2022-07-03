using System.Collections.Generic;


namespace MusicTheory
{
    /// <summary>
    /// Represents a microtonal pitch class set
    /// </summary>
    public class PcSetClass24
    {
        private HashSet<PitchClass24> _pitchSet;                     // The pitch class set class
        private bool _packFromRight;                                 // Whether or not to pack from the right
        private string _setPrimeFormName;                            // The prime form name of the current set class
        private string _setIcVectorName;                             // The interval class vector of the set class (as a string)
        private int[] _setIcVector;                                  // The interval class vector of the set class (as an array)
        private readonly static int NUM_PC = 24;                     // The number of unique pitch classes
        private string _setType;                                     // The set class type

        /// <summary>
        /// The number of elements in the set class
        /// </summary>
        public int Count { get { return _pitchSet.Count; } }

        /// <summary>
        /// The prime form name of the set class
        /// </summary>
        public string PrimeFormName { get { return _setPrimeFormName; } }

        /// <summary>
        /// The interval class vector of the set class
        /// </summary>
        public int[] ICVector { get { return _setIcVector; } }

        /// <summary>
        /// The interval class vector of the set class, as a string
        /// </summary>
        public string ICVectorName { get { return _setIcVectorName; } }

        /// <summary>
        /// Whether or not to pack from the right
        /// </summary>
        public bool PackFromRight
        {
            get { return _packFromRight; }
            set
            {
                _packFromRight = value;
                LoadFromPitchClassSet(_pitchSet);
            }
        }

        /// <summary>
        /// The type of the set class
        /// </summary>
        public string SetType { get { return _setType; } }

        /// <summary>
        /// Creates a new empty set class
        /// </summary>
        /// <param name="nameTables">An array of name tables</param>
        public PcSetClass24()
        {
            Construct();
        }

        /// <summary>
        /// Creates a new set class from a pitch class list
        /// </summary>
        /// <param name="pitchList">A pitch class list</param>
        /// <param name="nameTables">An array of name tables</param>
        public PcSetClass24(List<PitchClass24> pitchList)
        {
            Construct();
            LoadFromPitchClassList(pitchList);
        }

        /// <summary>
        /// Creates a new set class from an existing set class
        /// </summary>
        /// <param name="set">An existing set class</param>
        public PcSetClass24(PcSetClass24 set)
        {
            _packFromRight = set._packFromRight;
            _pitchSet = new HashSet<PitchClass24>();
            _setIcVector = new int[13];
            _setPrimeFormName = set._setPrimeFormName;
            _setIcVectorName = set._setIcVectorName;
            _setType = set._setType;
            foreach (PitchClass24 pc in set._pitchSet)
                _pitchSet.Add(new PitchClass24(pc));
            for (int i = 0; i < set._setIcVector.Length; i++)
                _setIcVector[i] = set._setIcVector[i];
        }

        /// <summary>
        /// Creates a new set class
        /// </summary>
        /// <param name="PitchClass24es">A list of pitch classes</param>
        /// <param name="set">An existing set class</param>
        public PcSetClass24(HashSet<PitchClass24> pitchSet, PcSetClass24 set)
        {
            _pitchSet = new HashSet<PitchClass24>();
            _packFromRight = set._packFromRight;
            _setIcVector = new int[13] { 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0 };
            _setPrimeFormName = "";
            _setIcVectorName = "";
            _setType = "";
            LoadFromPitchClassSet(pitchSet);
        }

        /// <summary>
        /// Calculates the ANGLE between two set classes, based on interval class vector
        /// (Damon Scott and Eric J. Isaacson, "The Interval Angle: A Similarity Measure
        /// for Pitch-Class Sets," Perspectives of New Music 36:2 (Summer, 1998), 107-142)
        /// Adjusted for microtonal ic vector
        /// </summary>
        /// <param name="set1">A set class</param>
        /// <param name="set2">A set class</param>
        /// <returns>The ANGLE between the two set classes, in radians</returns>
        public static double CalculateAngle(PcSetClass24 set1, PcSetClass24 set2)
        {
            int[] ic1 = set1.ICVector;
            int[] ic2 = set2.ICVector;
            return System.Math.Acos(
                (ic1[0] * ic2[0] + ic1[1] * ic2[1] + ic1[2] * ic2[2] + ic1[3] * ic2[3] + ic1[4] * ic2[4] + ic1[5] * ic2[5] +
                ic1[6] * ic2[6] + ic1[7] * ic2[7] + ic1[8] * ic2[8] + ic1[9] * ic2[9] + ic1[10] * ic2[10] + ic1[11] * ic2[11])
                / (System.Math.Sqrt(System.Math.Pow(ic1[0], 2) + System.Math.Pow(ic1[1], 2) + System.Math.Pow(ic1[2], 2)
                + System.Math.Pow(ic1[3], 2) + System.Math.Pow(ic1[4], 2) + System.Math.Pow(ic1[5], 2)
                + System.Math.Pow(ic1[6], 2) + System.Math.Pow(ic1[7], 2) + System.Math.Pow(ic1[8], 2)
                + System.Math.Pow(ic1[9], 2) + System.Math.Pow(ic1[10], 2) + System.Math.Pow(ic1[11], 2)
                )
                * System.Math.Sqrt(System.Math.Pow(ic2[0], 2) + System.Math.Pow(ic2[1], 2) + System.Math.Pow(ic2[2], 2)
                + System.Math.Pow(ic2[3], 2) + System.Math.Pow(ic2[4], 2) + System.Math.Pow(ic2[5], 2)
                + System.Math.Pow(ic2[6], 2) + System.Math.Pow(ic2[7], 2) + System.Math.Pow(ic2[8], 2)
                + System.Math.Pow(ic2[9], 2) + System.Math.Pow(ic2[10], 2) + System.Math.Pow(ic2[11], 2))));
        }

        /// <summary>
        /// Calculates the index vector of a list of pitch classes. Note that this does not
        /// perform any validation - you must call IsValidSet() first if you need to validate
        /// the list.
        /// </summary>
        /// <param name="listToVector">The list to calculate the index vector</param>
        /// <returns>The index vector</returns>
        public static int[] CalculateIndexVector(List<PitchClass24> listToVector)
        {
            int[] indexVector = new int[12] { 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0 };
            for (int i = 0; i < listToVector.Count; i++)
            {
                for (int j = 0; j < listToVector.Count; j++)
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
        private void Construct()
        {
            _packFromRight = true;
            _pitchSet = new HashSet<PitchClass24>();
            _setIcVector = new int[13] { 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0 };
            _setPrimeFormName = "";
            _setIcVectorName = "";
            _setType = "";
        }

        /// <summary>
        /// Checks to see if the set class contains a specific pitch class
        /// </summary>
        /// <param name="PitchClass24">The pitch class to look for</param>
        /// <returns>True if the set class contains the pitch class; false otherwise</returns>
        public bool Contains(PitchClass24 PitchClass24) => _pitchSet.Contains(PitchClass24);

        /// <summary>
        /// Checks to see if the set class contains a specific subset
        /// </summary>
        /// <param name="subset">The subset class</param>
        /// <returns>True if the set class contains the subset class; false otherwise</returns>
        public bool ContainsSubset(PcSetClass24 subset)
        {
            if (subset is null)
                return true;
            else if (subset._pitchSet.Count == 0)
                return true;
            else if (subset.Count > _pitchSet.Count)
                return false;
            else
            {
                HashSet<PitchClass24> transpose;
                HashSet<PitchClass24> invert;
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
        public override bool Equals(object obj) => Equals(obj as PcSetClass24);

        /// <summary>
        /// Compares two set classes for equality
        /// </summary>
        /// <param name="set">The set class</param>
        /// <returns>True if the set classes are equal; false otherwise</returns>
        public bool Equals(PcSetClass24 set)
        {
            if (set != null)
                return _pitchSet.Equals(set._pitchSet);
            else
                return false;
        }

        /// <summary>
        /// Finds the Tn or TnI transformation(s) that produced the provided list of pcs
        /// </summary>
        /// <param name="pitchSet">The pitch list to analyze</param>
        /// <returns>A list of matching set names</returns>
        public List<string> FindTransformationName(HashSet<PitchClass24> pitchSet)
        {
            List<string> transposeNames = new List<string>();
            List<string> invertNames = new List<string>();

            if (pitchSet.Count == _pitchSet.Count)
            {
                // Exhaustively compute all possible transformations and match them against the provided pitch class list
                for (int i = 0; i < NUM_PC; i++)
                {
                    bool transposeMatch = true;
                    bool invertMatch = true;
                    HashSet<PitchClass24> transpose = Transpose(i);
                    HashSet<PitchClass24> invert = InvertTranspose(i);
                    foreach (PitchClass24 pc in pitchSet)
                    {
                        if (!transpose.Contains(pc))
                        {
                            transposeMatch = false;
                            break;
                        }
                    }
                    foreach (PitchClass24 pc in pitchSet)
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
        public PcSetClass24 GetComplement()
        {
            PcSetClass24 complement;
            HashSet<PitchClass24> pitches = new HashSet<PitchClass24>();
            for (int i = 0; i < NUM_PC; i++)
            {
                PitchClass24 pc = new PitchClass24(i);
                if (!_pitchSet.Contains(pc))
                    pitches.Add(pc);
            }
            complement = new PcSetClass24(pitches, this);
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
                foreach (PitchClass24 pc in _pitchSet)
                    code ^= pc.PitchClassInteger;
                return code;
            }
        }

        /// <summary>
        /// Gets a collection of secondary forms that contain a provided collection of pitches
        /// </summary>
        /// <param name="PitchClasses">The pitch classes to search for</param>
        /// <returns>The secondary forms</returns>
        public List<Pair<string, HashSet<PitchClass24>>> GetSecondaryForms(List<PitchClass24> PitchClasses)
        {
            List<Pair<string, HashSet<PitchClass24>>> secondaryForms = new List<Pair<string, HashSet<PitchClass24>>>();

            // The cardinality of PitchClass24es cannot be greater than the cardinality of the set
            if (PitchClasses.Count <= _pitchSet.Count)
            {
                // Brute-force all Tn- and TnI-related pcsets. Note that the provided pcs may match multiple related pcsets.
                for (int i = 0; i < NUM_PC; i++)
                {
                    HashSet<PitchClass24> transpose = PcSet.Transpose(_pitchSet, i);
                    HashSet<PitchClass24> invert = PcSet.Transpose(PcSet.Invert(_pitchSet), i);
                    bool isTranspose = true;
                    bool isInvert = true;

                    foreach (PitchClass24 pc in PitchClasses)
                    {
                        if (!transpose.Contains(pc))
                        {
                            isTranspose = false;
                            break;
                        }
                    }
                    foreach (PitchClass24 pc in PitchClasses)
                    {
                        if (!invert.Contains(pc))
                        {
                            isInvert = false;
                            break;
                        }
                    }

                    if (isTranspose)
                        secondaryForms.Add(new Pair<string, HashSet<PitchClass24>>('T' + i.ToString(), transpose));
                    if (isInvert)
                        secondaryForms.Add(new Pair<string, HashSet<PitchClass24>>('T' + i.ToString() + 'I', invert));
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
        public HashSet<PitchClass24> GetSetCopy()
        {
            HashSet<PitchClass24> set = new HashSet<PitchClass24>();
            foreach (PitchClass24 pc in _pitchSet)
                set.Add(new PitchClass24(pc));
            return set;
        }

        /// <summary>
        /// Gets all subsets of the current set
        /// </summary>
        /// <returns>A list of subsets</returns>
        public List<Pair<PcSetClass24, int>> GetSubsets()
        {
            List<Pair<PcSetClass24, int>> subsets = new List<Pair<PcSetClass24, int>>();
            Dictionary<string, int> foundSets = new Dictionary<string, int>();
            List<HashSet<PitchClass24>> store = PcSet.Subsets(_pitchSet);

            // Remove duplicate set-classes
            for (int i = 0; i < store.Count; i++)
            {
                PcSetClass24 set = new PcSetClass24(store[i], this);
                if (!foundSets.ContainsKey(set._setPrimeFormName))
                {
                    subsets.Add(new Pair<PcSetClass24, int>(set, 1));
                    foundSets[set._setPrimeFormName] = subsets.Count - 1;
                }
                else
                    subsets[foundSets[set._setPrimeFormName]].Item2++;
            }

            subsets.Sort((a, b) =>
            {
                if (a.Item1.Count < b.Item1.Count)
                    return -1;
                else if (a.Item1.Count > b.Item1.Count)
                    return 1;
                else
                    return a.Item1._setPrimeFormName.CompareTo(b.Item1._setPrimeFormName);
            });

            return subsets;
        }

        /// <summary>
        /// Gets all subsets of the current set of a specified cardinality
        /// </summary>
        /// <param name="cardinality">The cardinality of the subsets</param>
        /// <returns>A list of subsets</returns>
        public List<Pair<PcSetClass24, int>> GetSubsets(int cardinality)
        {
            List<Pair<PcSetClass24, int>> subsets = new List<Pair<PcSetClass24, int>>();
            Dictionary<string, int> foundSets = new Dictionary<string, int>();
            List<HashSet<PitchClass24>> store = PcSet.Subsets(_pitchSet);

            // Remove duplicate set-classes and set-classes of the wrong size
            for (int i = 0; i < store.Count; i++)
            {
                if (store[i].Count == cardinality)
                {
                    PcSetClass24 set = new PcSetClass24(store[i], this);
                    if (!foundSets.ContainsKey(set._setPrimeFormName))
                    {
                        subsets.Add(new Pair<PcSetClass24, int>(set, 1));
                        foundSets[set._setPrimeFormName] = subsets.Count - 1;
                    }
                    else
                        subsets[foundSets[set._setPrimeFormName]].Item2++;
                }
            }

            subsets.Sort((a, b) =>
            {
                if (a.Item1.Count < b.Item1.Count)
                    return -1;
                else if (a.Item1.Count > b.Item1.Count)
                    return 1;
                else
                    return a.Item1._setPrimeFormName.CompareTo(b.Item1._setPrimeFormName);
            });

            return subsets;
        }

        /// <summary>
        /// Inverts and transposes the prime form
        /// </summary>
        /// <param name="numberOfTranspositions">The number of transpositions</param>
        /// <returns>An inverted and transposed version of the prime form</returns>
        public HashSet<PitchClass24> InvertTranspose(int numberOfTranspositions)
        {
            HashSet<PitchClass24> invertTranspose = PcSet.Transpose(PcSet.Invert(_pitchSet), numberOfTranspositions);
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
        public static bool IsValidSet(List<PitchClass24> pitches)
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
                foreach (PitchClass24 i in pitches)
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

            // The name must be between 2 and 96 characters
            if (name.Length < 2 || name.Length > 96)
                return false;

            if (name == "null" || name == "NULL")
                return true;

            // Otherwise, it must be an integer name
            else
            {
                HashSet<PitchClass24> pcset = new HashSet<PitchClass24>();

                // We permit enclosing brackets or parentheses
                if ((name[0] == '[' && name[name.Length - 1] == ']') || (name[0] == '(' && name[name.Length - 1] == ')'))
                {
                    name.Remove(name.Length - 1, 1);
                    name.Remove(0, 1);
                }

                // Validate the pcset
                foreach (string str in name.Split(", "))
                    pcset.Add(new PitchClass24(System.Int32.Parse(str)));
                if (CalculatePrimeForm(pcset) == pcset)
                    return true;
                else
                    return false;
            }
        }

        /// <summary>
        /// Loads the set from a pitch class list. Note that this does not perform validation
        /// on the pitch class list! If you want to validate the list, you will have to call
        /// IsValidSet() first.
        /// </summary>
        /// <param name="PitchClassList">The pitch class list to load</param>
        public void LoadFromPitchClassList(List<PitchClass24> PitchClassList) => LoadFromPitchClassSet(PcSeg.ToPcSet(PitchClassList));

        /// <summary>
        /// Loads the set from a pcset. No validation is necessary because the HashSet
        /// type does not allow duplicates.
        /// </summary>
        /// <param name="pcset">A pcset</param>
        public void LoadFromPitchClassSet(HashSet<PitchClass24> pcset)
        {
            // Only one set-class exists for sets of cardinality 0, 11, and 12.
            // No further calculation is necessary.
            if (pcset.Count == 0)
                LoadNullSet();
            else if (pcset.Count == 11)
            {
                _pitchSet.Clear();
                for (int i = 0; i < 11; i++)
                    _pitchSet.Add(new PitchClass24(i));
                UpdateSetNames();
            }
            else if (pcset.Count == 12)
            {
                _pitchSet.Clear();
                for (int i = 0; i < 12; i++)
                    _pitchSet.Add(new PitchClass24(i));
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
            HashSet<PitchClass24> pitches = new HashSet<PitchClass24>();
            primeFormName = primeFormName.ToUpper();
            if (IsValidName(primeFormName))
            {
                foreach (char letter in primeFormName)
                    pitches.Add(new PitchClass24(letter));
                LoadFromPitchClassSet(pitches);
            }
        }

        /// <summary>
        /// Loads a set-class from an existing set-class
        /// </summary>
        /// <param name="sc">The existing PcSetClass</param>
        public void LoadFromSet(PcSetClass24 sc)
        {
            _pitchSet.Clear();
            _setPrimeFormName = sc._setPrimeFormName;
            _setIcVectorName = sc._setIcVectorName;
            _setType = sc._setType;
            foreach (PitchClass24 pc in sc._pitchSet)
                _pitchSet.Add(new PitchClass24(pc));
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
        public HashSet<PitchClass24> Transform(string transformationString)
        {
            List<Pair<char, int>> opList = new List<Pair<char, int>>();
            HashSet<PitchClass24> pcList = _pitchSet;
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
        public HashSet<PitchClass24> Transpose(int numberOfTranspositions)
        {
            HashSet<PitchClass24> transposed = PcSet.Transpose(_pitchSet, numberOfTranspositions);
            return transposed;
        }

        /// <summary>
        /// Compares two PcSetClasses for equality
        /// </summary>
        /// <param name="set1">A set-class</param>
        /// <param name="set2">A set-class</param>
        /// <returns>True if the two set-classes are equal; false otherwise</returns>
        public static bool operator ==(PcSetClass24 set1, PcSetClass24 set2)
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
        public static bool operator !=(PcSetClass24 set1, PcSetClass24 set2) => !(set1 == set2);

        /// <summary>
        /// Calculates the prime form of a provided set of pitch classes
        /// </summary>
        /// <param name="PitchClassList">The pitch class list to evaluate</param>
        /// <returns>The prime form of the set-class</returns>
        private HashSet<PitchClass24> CalculatePrimeForm(HashSet<PitchClass24> PitchClassList)
        {
            List<List<PitchClass24>> listsToWeight = new List<List<PitchClass24>>(2 * PitchClassList.Count);
            List<PitchClass24> PitchClasses = PcSet.ToPcSeg(PitchClassList);
            List<PitchClass24> inverted = PcSeg.Invert(PitchClasses);

            // Add regular forms to the lists to weight
            for (int i = 0; i < PitchClassList.Count; i++)
            {
                listsToWeight.Add(new List<PitchClass24>(PitchClassList.Count));
                for (int i2 = i; i2 < PitchClassList.Count; i2++)
                    listsToWeight[i].Add(new PitchClass24(PitchClasses[i2]));
                for (int i2 = 0; i2 < i; i2++)
                    listsToWeight[i].Add(new PitchClass24(PitchClasses[i2]));
                int initialPitch = listsToWeight[i][0].PitchClassInteger;
                for (int j = 0; j < listsToWeight[i].Count; j++)
                    listsToWeight[i][j].PitchClassInteger -= initialPitch;
                listsToWeight[i].Sort((a, b) => a.PitchClassInteger.CompareTo(b.PitchClassInteger));
            }

            // Add inverted forms to the lists to weight
            for (int i = 0; i < inverted.Count; i++)
            {
                listsToWeight.Add(new List<PitchClass24>(PitchClassList.Count));
                for (int i2 = i; i2 < inverted.Count; i2++)
                    listsToWeight[i + PitchClassList.Count].Add(new PitchClass24(inverted[i2]));
                for (int i2 = 0; i2 < i; i2++)
                    listsToWeight[i + PitchClassList.Count].Add(new PitchClass24(inverted[i2]));
                int initialPitch = listsToWeight[i + PitchClassList.Count][0].PitchClassInteger;
                for (int j = 0; j < listsToWeight[i + PitchClassList.Count].Count; j++)
                    listsToWeight[i + PitchClassList.Count][j].PitchClassInteger -= initialPitch;
                listsToWeight[i + PitchClassList.Count].Sort((a, b) => a.PitchClassInteger.CompareTo(b.PitchClassInteger));
            }

            if (_packFromRight)
                return WeightFromRight(listsToWeight);
            else
                return WeightLeft(listsToWeight);
        }

        /// <summary>
        /// Updates the set names. Only call this method when the set is in prime form at T0.
        /// </summary>
        private void UpdateSetNames()
        {
            // First clear the existing names
            _setPrimeFormName = "";
            _setIcVectorName = "";
            for (int i = 0; i < _setIcVector.Length; i++)
                _setIcVector[i] = 0;

            // Calculate the prime form name by listing all of the pitches in the set
            // (remember, the set must be in prime form at T0 for this to work!)
            List<PitchClass24> pitches = PcSet.ToSortedPcSeg(_pitchSet);
            _setPrimeFormName = PcSeg.ToString(pitches);

            // Get the set interval class vector
            for (int i = 0; i < pitches.Count; i++)
            {
                for (int j = i; j < pitches.Count; j++)
                {
                    int interval = (pitches[j] - pitches[i]).PitchClassInteger;
                    if (interval > NUM_PC / 2)
                        interval = interval * (NUM_PC - 1) % NUM_PC;
                    _setIcVector[interval]++;
                }
            }

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

            _setIcVectorName = "[";
            for (int i = 0; i < 12; i++)
                _setIcVectorName += _setIcVector[i].ToString() + ", ";
            _setIcVectorName += _setIcVector[12].ToString() + "]";
        }

        /// <summary>
        /// Weights a set of pitch class lists to the right
        /// </summary>
        /// <param name="listsToWeight">The lists to weight</param>
        /// <returns>A HashSet containing the most weighted form</returns>
        private HashSet<PitchClass24> WeightFromRight(List<List<PitchClass24>> listsToWeight)
        {
            HashSet<PitchClass24> set = new HashSet<PitchClass24>();
            int smallestItem;  // The smallest pitch integer we've found at the current index
            int j;             // The index of the list we are looking at

            // Weight the lists from the right
            for (int i = listsToWeight[0].Count - 1; i >= 0; i--)
            {
                if (listsToWeight.Count > 1)
                {
                    smallestItem = 23;  // The smallest pitch integer we've found at the current index

                    // Identify the smallest item at index i
                    for (j = 0; j < listsToWeight.Count; j++)
                    {
                        if (listsToWeight[j][i].PitchClassInteger < smallestItem)
                            smallestItem = listsToWeight[j][i].PitchClassInteger;
                    }

                    // Remove all lists that do not have the smallest item at index i
                    j = 0;
                    while (j < listsToWeight.Count)
                    {
                        if (listsToWeight[j][i].PitchClassInteger > smallestItem)
                            listsToWeight.RemoveAt(j);
                        else
                            j++;
                    }
                }
                else
                    break;
            }

            foreach (PitchClass24 pc in listsToWeight[0])
                set.Add(pc);
            return set;
        }

        /// <summary>
        /// Weights a set of pitch class lists from the left
        /// </summary>
        /// <param name="listsToWeight">The lists to weight</param>
        /// <returns>A HashSet containing the most weighted form</returns>
        private HashSet<PitchClass24> WeightLeft(List<List<PitchClass24>> listsToWeight)
        {
            HashSet<PitchClass24> set = new HashSet<PitchClass24>();
            int smallestItem = 23;  // The smallest pitch integer we've found at the current index
            int j = 0;              // The index of the list we are looking at
            int maxIndex = listsToWeight[0].Count - 1;  // The maximum index to look at

            if (listsToWeight.Count > 1)
            {
                // Identify the smallest item at maxIndex
                for (; j < listsToWeight.Count; j++)
                {
                    if (listsToWeight[j][maxIndex].PitchClassInteger < smallestItem)
                        smallestItem = listsToWeight[j][maxIndex].PitchClassInteger;
                }

                // Remove all lists that do not have the smallest item at maxIndex
                j = 0;
                while (j < listsToWeight.Count)
                {
                    if (listsToWeight[j][maxIndex].PitchClassInteger > smallestItem)
                        listsToWeight.RemoveAt(j);
                    else
                        j++;
                }

                // Weight the lists from the left
                for (int i = 0; i <= maxIndex; i++)
                {
                    if (listsToWeight.Count > 1)
                    {
                        smallestItem = 23;

                        // Identify the smallest item at index i
                        for (j = 0; j < listsToWeight.Count; j++)
                        {
                            if (listsToWeight[j][i].PitchClassInteger < smallestItem)
                                smallestItem = listsToWeight[j][i].PitchClassInteger;
                        }

                        // Remove all lists that do not have the smallest item at index i
                        j = 0;
                        while (j < listsToWeight.Count)
                        {
                            if (listsToWeight[j][i].PitchClassInteger > smallestItem)
                                listsToWeight.RemoveAt(j);
                            else
                                j++;
                        }
                    }
                    else
                        break;
                }
            }

            foreach (PitchClass24 pc in listsToWeight[0])
                set.Add(pc);
            return set;
        }
    }
}
