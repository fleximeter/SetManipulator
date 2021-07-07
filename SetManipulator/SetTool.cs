using System;
using System.Collections.Generic;
using MusicTheory;

namespace SetManipulator
{
    class SetTool
    {
        private Dictionary<string, string>[] _nameTables;        // Name tables for sets
        PcSetClass set;
        PcSetClass complement;
        PcSetClass zSet;
        SetComplex kComplex;
        SetComplex khComplex;
        bool hasZ;
        PrimarySetName preferForteName;

        public SetTool()
        {
            _nameTables = Functions.GenerateSetTables();
            set = new PcSetClass(_nameTables);
            complement = new PcSetClass(_nameTables);
            zSet = new PcSetClass(_nameTables);
            kComplex = new SetComplex();
            khComplex = new SetComplex();
            hasZ = false;
            preferForteName = PrimarySetName.PrimeForm;
        }

        public void SetMenu()
        {
            DisplayMenu();
            string input;
            do
            {
                Console.Write("> ");
                input = Console.ReadLine();
                switch (input)
                {
                    case "1":
                        LoadSet();
                        Console.WriteLine();
                        break;
                    case "2":
                        CalculateSet();
                        Console.WriteLine();
                        break;
                    case "3":
                        CalculationMode();
                        Console.WriteLine();
                        break;
                    case "4":
                        DisplaySet();
                        Console.WriteLine();
                        break;
                    case "5":
                        DisplayComplement();
                        Console.WriteLine();
                        break;
                    case "6":
                        if (hasZ)
                            DisplayZ();
                        else
                            Console.WriteLine("This set-class has no Z-relation");
                        Console.WriteLine();
                        break;
                    case "7":
                        DisplaySubsets();
                        Console.WriteLine();
                        break;
                    case "8":
                        DisplaySecondary();
                        Console.WriteLine();
                        break;
                    case "9":
                        DisplayForms();
                        Console.WriteLine();
                        break;
                    case "10":
                        ContinuousSearch();
                        Console.WriteLine();
                        break;
                    case "11":
                        DisplayK();
                        break;
                    case "12":
                        DisplayKh();
                        break;
                    case "13":
                        SearchK();
                        Console.WriteLine();
                        break;
                    case "14":
                        SearchKh();
                        Console.WriteLine();
                        break;
                    case "m":
                    case "M":
                        DisplayMenu();
                        break;
                    case "p":
                    case "P":
                        Preferences();
                        break;
                    case "r":
                    case "R":
                        break;
                    default:
                        Console.WriteLine("Invalid entry");
                        break;
                }
            } while (input != "r");
        }

        void DisplayMenu()
        {
            Console.Write("\n1) Enter set-class\n2) Calculate set-class\n3) Continuous set-class calculation mode\n4) Display set-class\n" +
                "5) Display complement\n6) Display Z-relation\n7) Display subset-classes\n8) Display secondary form(s)\n9) Search forms\n" +
                "10) Continuous search mode\n11) Display set complex K\n12) Display set complex Kh\n13) Search set complex K\n" +
                "14) Search set complex Kh\nm) Display this menu\np) Set preferences\nr) Return\n");
        }

        void LoadSet()
        {
            string setName;
            Console.Write("Input the set-class > ");
            setName = Console.ReadLine().Trim();
            if (set.IsValidSetName(setName))
            {
                if (setName.Contains('-'))
                    set.LoadSetFromForteName(setName);
                else
                    set.LoadSetFromPrimeFormName(setName);
                complement = set.GetComplement();
                if (set.ForteName.Contains('Z'))
                {
                    zSet = set.GetZRelation();
                    hasZ = true;
                }
                else
                {
                    zSet.Clear();
                    hasZ = false;
                }
                kComplex.LoadNexusSet(set, SetComplexType.K);
                khComplex.LoadNexusSet(set, SetComplexType.Kh);
                DisplaySet();
            }
            else
                Console.WriteLine("Invalid set-class");
        }

        void CalculateSet()
        {
            List<PitchClass> pitchList = new List<PitchClass>();
            string pitches;
            Console.Write("Enter pitches > ");
            pitches = Console.ReadLine().ToUpper();
            foreach (char c in pitches)
                pitchList.Add(new PitchClass(c));
            if (PcSetClass.IsValidSet(pitchList))
            {
                set.LoadSetFromPitchClassList(pitchList);
                complement = set.GetComplement();
                if (set.ForteName.Contains('Z'))
                {
                    zSet = set.GetZRelation();
                    hasZ = true;
                }
                else
                {
                    zSet.Clear();
                    hasZ = false;
                }
                kComplex.LoadNexusSet(set, SetComplexType.K);
                khComplex.LoadNexusSet(set, SetComplexType.Kh);
                DisplaySet();
                Console.Write("You entered ");
                List<string> secondary = set.FindTransformationName(pitchList);
                for (int i = 0; i < secondary.Count; i++)
                {
                    if (i < secondary.Count - 2)
                        Console.Write(secondary[i] + ", ");
                    else if (i == secondary.Count - 2)
                        Console.Write(secondary[i] + " ");
                    else if (secondary.Count > 1)
                        Console.Write("and " + secondary[i] + "\n");
                    else
                        Console.Write(secondary[i] + "\n");
                }
            }
            else
                Console.WriteLine("Invalid set-class");
        }

        void CalculationMode()
        {
            List<PitchClass> pitchList = new List<PitchClass>();
            string pitches;
            Console.Write("Enter pitches (q to quit) > ");
            pitches = Console.ReadLine().ToUpper();
            while (pitches[0] != 'Q')
            {
                pitchList.Clear();
                foreach (char c in pitches)
                    pitchList.Add(new PitchClass(c));
                if (PcSetClass.IsValidSet(pitchList))
                {
                    set.LoadSetFromPitchClassList(pitchList);
                    complement = set.GetComplement();
                    if (set.ForteName.Contains('Z'))
                    {
                        zSet = set.GetZRelation();
                        hasZ = true;
                    }
                    else
                    {
                        zSet.Clear();
                        hasZ = false;
                    }
                    kComplex.LoadNexusSet(set, SetComplexType.K);
                    khComplex.LoadNexusSet(set, SetComplexType.Kh);
                    DisplaySet();
                    Console.Write("You entered ");
                    List<string> secondary = set.FindTransformationName(pitchList);
                    for (int i = 0; i < secondary.Count; i++)
                    {
                        if (i < secondary.Count - 2)
                            Console.Write(secondary[i] + ", ");
                        else if (i == secondary.Count - 2)
                            Console.Write(secondary[i] + " ");
                        else if (secondary.Count > 1)
                            Console.Write("and " + secondary[i] + "\n");
                        else
                            Console.Write(secondary[i] + "\n");
                    }
                }
                else
                    Console.WriteLine("Invalid set-class");
                Console.Write("\nEnter pitches (q to quit) > ");
                pitches = Console.ReadLine().ToUpper();
            }
        }

        void DisplaySet()
        {
            if (preferForteName == PrimarySetName.Forte)
            {
                Console.WriteLine(string.Format("{0,-17}{1,-12}", "Forte name:", set.ForteName));
                Console.WriteLine(string.Format("{0,-17}{1,-12}", "Prime form name:", '(' + set.PrimeFormName + ')'));
                Console.WriteLine(string.Format("{0,-17}{1,-12}", "Carter name:", set.CarterName));
            }
            else
            {
                Console.WriteLine(string.Format("{0,-17}{1,-12}", "Prime form name:", '(' + set.PrimeFormName + ')'));
                Console.WriteLine(string.Format("{0,-17}{1,-12}", "Forte name:", set.ForteName));
                Console.WriteLine(string.Format("{0,-17}{1,-12}", "Carter name:", set.CarterName));
            }
            Console.WriteLine(string.Format("{0,-17}{1,-12}", "IC vector:", set.ICVectorName));
            Console.WriteLine(string.Format("{0,-17}{1,-12}", "Complement:", '(' + complement.PrimeFormName + ')'));
        }

        void DisplayComplement()
        {
            if (preferForteName == PrimarySetName.Forte)
            {
                Console.WriteLine(string.Format("{0,-28}{1,-12}", "Complement Forte name:", complement.ForteName));
                Console.WriteLine(string.Format("{0,-28}{1,-12}", "Complement prime form name:", '(' + complement.PrimeFormName + ')'));
                Console.WriteLine(string.Format("{0,-28}{1,-12}", "Complement Carter name:", complement.CarterName));
            }
            else
            {
                Console.WriteLine(string.Format("{0,-28}{1,-12}", "Complement prime form name:", '(' + complement.PrimeFormName + ')'));
                Console.WriteLine(string.Format("{0,-28}{1,-12}", "Complement Forte name:", complement.ForteName));
                Console.WriteLine(string.Format("{0,-28}{1,-12}", "Complement Carter name:", complement.CarterName));
            }
            Console.WriteLine(string.Format("{0,-28}{1,-12}", "IC vector:", complement.ICVectorName));
        }

        void DisplayZ()
        {
            if (preferForteName == PrimarySetName.Forte)
            {
                Console.WriteLine(string.Format("{0,-28}{1,-12}", "Z-relation Forte name:", zSet.ForteName));
                Console.WriteLine(string.Format("{0,-28}{1,-12}", "Z-relation prime form name:", '(' + zSet.PrimeFormName + ')'));
                Console.WriteLine(string.Format("{0,-28}{1,-12}", "Z-relation Carter name:", zSet.CarterName));
            }
            else
            {
                Console.WriteLine(string.Format("{0,-28}{1,-12}", "Z-relation prime form name:", '(' + zSet.PrimeFormName + ')'));
                Console.WriteLine(string.Format("{0,-28}{1,-12}", "Z-relation Forte name:", zSet.ForteName));
                Console.WriteLine(string.Format("{0,-28}{1,-12}", "Z-relation Carter name:", zSet.CarterName));
            }
            Console.WriteLine(string.Format("{0,-28}{1,-12}", "IC vector:", zSet.ICVectorName));
        }

        void DisplaySubsets()
        {
            List<Pair<PcSetClass, int>> subsets;

            if (preferForteName == PrimarySetName.Forte)
            {
                Console.WriteLine("Subset-classes for " + set.ForteName + ":\n");
                Console.WriteLine("Null:\nnull: 1\n");
            }
            else
            {
                Console.WriteLine("Subset-classes for (" + set.PrimeFormName + "):\n");
                Console.WriteLine("Null:\n(null) - 1\n");
            }

            for (int i = 1; i < set.Count; i++)
            {
                subsets = set.GetSubsets(i, preferForteName);
                Console.WriteLine(subsets[0].Item1.SetType + "s:");
                if (preferForteName == PrimarySetName.Forte)
                {
                    for (int j = 0; j < subsets.Count; j += 9)
                    {
                        for (int k = j; k < j + 9 && k < subsets.Count; k++)
                            Console.Write(string.Format("{0,-13}", subsets[k].Item1.ForteName + ": " + subsets[k].Item2.ToString()));
                        Console.Write("\n");
                    }
                }
                else
                {
                    for (int j = 0; j < subsets.Count; j += 5)
                    {
                        for (int k = j; k < j + 5 && k < subsets.Count; k++)
                            Console.Write(string.Format("{0,-22}", '(' + subsets[k].Item1.PrimeFormName + ") - " + subsets[k].Item2.ToString()));
                        Console.Write("\n");
                    }
                }
                Console.Write("\n");
            }
            
            if (preferForteName == PrimarySetName.Forte)
                Console.WriteLine(set.SetType + "s:\n" + set.ForteName + ": 1");
            else
                Console.WriteLine(set.SetType + "s:\n(" + set.PrimeFormName + ") - 1");
        }

        void DisplaySecondary()
        {
            string secondary;
            string[] forms;
            Console.Write("Enter the secondary form(s): ");
            secondary = Console.ReadLine().ToUpper().Trim();
            if (secondary.Contains(','))
                forms = secondary.Split(',');
            else
                forms = secondary.Split(' ');
            foreach (string form in forms)
            {
                string trimForm = form.Trim();
                if (trimForm.Length > 0)
                {
                    if (PcSet.IsValidTransformation(trimForm))
                    {
                        List<PitchClass> transformation = Functions.ConvertPcSetToSortedList(set.Transform(trimForm));
                        Console.Write(string.Format("{0, -6}", trimForm + ": "));
                        foreach (PitchClass p in transformation)
                            Console.Write(p.PitchClassChar);
                        Console.Write('\n');
                    }
                    else
                        Console.WriteLine(string.Format("{0,-6}{1}", trimForm + ": ", "Invalid transformation name"));
                }
            }
        }

        void DisplayForms()
        {
            List<PitchClass> pitchList = new List<PitchClass>();
            string pitches;
            Console.Write("Enter pitches > ");
            pitches = Console.ReadLine().ToUpper();
            foreach (char c in pitches)
            {
                pitchList.Add(new PitchClass(c));
            }
            List<Pair<string, HashSet<PitchClass>>> secondary = set.GetSecondaryForms(pitchList);
            if (secondary.Count > 0)
            {
                Console.WriteLine("These pitches match forms");
                string secondaryPitches;
                List<PitchClass> list;
                for (int i = 0; i < secondary.Count; i++)
                {
                    secondaryPitches = "";
                    list = Functions.ConvertPcSetToList(secondary[i].Item2);
                    foreach (PitchClass pc in list)
                        secondaryPitches += pc.PitchClassChar;
                    Console.WriteLine(string.Format("{0,-5}{1,-9}", secondary[i].Item1, secondaryPitches));
                }
            }
            else
                Console.WriteLine("No matches!");
        }

        void ContinuousSearch()
        {
            List<PitchClass> pitchList = new List<PitchClass>();
            string pitches;
            Console.Write("Enter pitches (q to quit) > ");
            pitches = Console.ReadLine().ToUpper();
            while (pitches != "Q")
            {
                foreach (char c in pitches)
                {
                    pitchList.Add(new PitchClass(c));
                }
                List<Pair<string, HashSet<PitchClass>>> secondary = set.GetSecondaryForms(pitchList);
                if (secondary.Count > 0)
                {
                    Console.WriteLine("These pitches match forms");
                    string secondaryPitches;
                    List<PitchClass> list;
                    for (int i = 0; i < secondary.Count; i++)
                    {
                        secondaryPitches = "";
                        list = Functions.ConvertPcSetToList(secondary[i].Item2);
                        foreach (PitchClass pc in list)
                            secondaryPitches += pc.PitchClassChar;
                        Console.WriteLine(string.Format("{0,-5}{1,-9}", secondary[i].Item1, secondaryPitches));
                    }
                }
                Console.Write("Enter pitches (q to quit) > ");
                pitches = Console.ReadLine().ToUpper();
                pitchList.Clear();
            }
        }

        void DisplayK()
        {
            SortedDictionary<string, PcSetClass> cardinalityList;
            if (preferForteName == PrimarySetName.Forte)
                Console.WriteLine("Set-complex K about " + set.ForteName + ":\n");
            else
                Console.WriteLine("Set-complex K about (" + set.PrimeFormName + "):\n");

            for (int i = 1; i <= 12; i++)
            {
                if (kComplex.Cardinalities[i] == 1)
                {
                    cardinalityList = kComplex.GetCardinality(i);
                    SortedDictionary<string, PcSetClass>.Enumerator e = cardinalityList.GetEnumerator();
                    e.MoveNext();
                    Console.WriteLine(e.Current.Value.SetType + "s:");
                    if (preferForteName == PrimarySetName.Forte)
                    {
                        for (int j = 0; j < cardinalityList.Count; j += 12)
                        {
                            for (int k = j; k < j + 12 && k < cardinalityList.Count; k++)
                            {
                                Console.Write(string.Format("{0,-7}", e.Current.Value.ForteName));
                                e.MoveNext();
                            }
                            Console.Write("\n");
                        }
                    }
                    else
                    {
                        for (int j = 0; j < cardinalityList.Count; j += 5)
                        {
                            for (int k = j; k < j + 5 && k < cardinalityList.Count; k++)
                            {
                                Console.Write(string.Format("{0,-18}", '(' + e.Current.Value.PrimeFormName + ')'));
                                e.MoveNext();
                            }
                            Console.Write("\n");
                        }
                    }
                    Console.Write("\n");
                }
            }
        }

        void DisplayKh()
        {
            SortedDictionary<string, PcSetClass> cardinalityList;
            if (preferForteName == PrimarySetName.Forte)
                Console.WriteLine("Set-complex Kh about " + set.ForteName + ":\n");
            else
                Console.WriteLine("Set-complex Kh about (" + set.PrimeFormName + "):\n");

            for (int i = 1; i <= 12; i++)
            {
                if (khComplex.Cardinalities[i] == 1)
                {
                    cardinalityList = khComplex.GetCardinality(i);
                    SortedDictionary<string, PcSetClass>.Enumerator e = cardinalityList.GetEnumerator();
                    e.MoveNext();
                    Console.WriteLine(e.Current.Value.SetType + "s:");
                    if (preferForteName == PrimarySetName.Forte)
                    {
                        for (int j = 0; j < cardinalityList.Count; j += 12)
                        {
                            for (int k = j; k < j + 12 && k < cardinalityList.Count; k++)
                            {
                                Console.Write(string.Format("{0,-7}", e.Current.Value.ForteName));
                                e.MoveNext();
                            }
                            Console.Write("\n");
                        }
                    }
                    else
                    {
                        for (int j = 0; j < cardinalityList.Count; j += 5)
                        {
                            for (int k = j; k < j + 5 && k < cardinalityList.Count; k++)
                            {
                                Console.Write(string.Format("{0,-18}", '(' + e.Current.Value.PrimeFormName + ')'));
                                e.MoveNext();
                            }
                            Console.Write("\n");
                        }
                    }
                    Console.Write("\n");
                }
            }
        }

        void SearchK()
        {
            string setName;
            PcSetClass k = new PcSetClass(set);
            Console.Write("Input the set-class > ");
            setName = Console.ReadLine().Trim();
            if (set.IsValidSetName(setName))
            {
                if (setName.Contains('-'))
                    k.LoadSetFromForteName(setName);
                else
                    k.LoadSetFromPrimeFormName(setName);
                if (kComplex.IsInComplex(k) && preferForteName == PrimarySetName.Forte)
                    Console.WriteLine(k.ForteName + " is in the complex K of " + set.ForteName);
                else if (khComplex.IsInComplex(k))
                    Console.WriteLine('(' + k.PrimeFormName + ") is in the complex K of (" + set.PrimeFormName + ')');
                else
                    Console.WriteLine("Not found");
            }
            else
                Console.WriteLine("Invalid set-class");
        }

        void SearchKh()
        {
            string setName;
            PcSetClass kh = new PcSetClass(set);
            Console.Write("Input the set-class > ");
            setName = Console.ReadLine().Trim();
            if (set.IsValidSetName(setName))
            {
                if (setName.Contains('-'))
                    kh.LoadSetFromForteName(setName);
                else
                    kh.LoadSetFromPrimeFormName(setName);
                if (khComplex.IsInComplex(kh) && preferForteName == PrimarySetName.Forte)
                    Console.WriteLine(kh.ForteName + " is in the complex Kh of " + set.ForteName);
                else if (khComplex.IsInComplex(kh))
                    Console.WriteLine('(' + kh.PrimeFormName + ") is in the complex Kh of (" + set.PrimeFormName + ')');
                else
                    Console.WriteLine("Not found");
            }
            else
                Console.WriteLine("Invalid set-class");
        }

        void Preferences()
        {
            string input;
            Console.WriteLine("Enter \"f\" to prefer Forte names, or \"p\" to prefer prime form names: ");
            input = Console.ReadLine();
            switch (input)
            {
                case "f":
                case "F":
                    preferForteName = PrimarySetName.Forte;
                    kComplex.PreferForteNames = PrimarySetName.Forte;
                    khComplex.PreferForteNames = PrimarySetName.Forte;
                    break;
                case "p":
                case "P":
                    preferForteName = PrimarySetName.PrimeForm;
                    kComplex.PreferForteNames = PrimarySetName.PrimeForm;
                    khComplex.PreferForteNames = PrimarySetName.PrimeForm;
                    break;
                default:
                    Console.WriteLine("Invalid input");
                    break;
            }
        }
    }
}
