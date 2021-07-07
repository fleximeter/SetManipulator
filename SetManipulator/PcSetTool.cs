/* File: PcSetTool.cs
** Project: SetManipulator
** Author: Jeffrey Martin
**
** This file contains the implementation of the PcSetTool class. PcSetTool contains user-facing
** functionality for working with pcsets.
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

using System;
using System.Collections.Generic;
using MusicTheory;

namespace SetManipulator
{
    class PcSetTool : MusicTool
    {
        HashSet<PitchClass>[] pcset;
        PcSetClass[] sc;
        SetComplex k;
        SetComplex kh;
        Dictionary<string, string>[] tables;
        bool hasZ;
        PrimarySetName preferForteName;

        /// <summary>
        /// Whether or not to prefer Forte names
        /// </summary>
        public override PrimarySetName DefaultSetName
        {
            get { return preferForteName; }
            set { preferForteName = value; }
        }

        /// <summary>
        /// Initializes the PcSetTool
        /// </summary>
        public PcSetTool()
        {
            tables = Functions.GenerateSetTables();
            pcset = new HashSet<PitchClass>[3] { new HashSet<PitchClass>(), new HashSet<PitchClass>(), new HashSet<PitchClass>() };
            sc = new PcSetClass[5] { new PcSetClass(tables), new PcSetClass(tables), new PcSetClass(tables), new PcSetClass(tables), new PcSetClass(tables) };
            k = new SetComplex();
            kh = new SetComplex();
            hasZ = false;
            preferForteName = PrimarySetName.PrimeForm;
        }

        /// <summary>
        /// Calculates the ANGLE between two pcsets
        /// </summary>
        /// <param name="command1">A pcset string</param>
        /// <param name="command2">A pcset string</param>
        public override void Angle(string command1 = "", string command2 = "")
        {
            List<PitchClass>[] set = new List<PitchClass>[2];
            double angle;
            
            if (command1 != "" && command2 == "")
            {
                set[0] = PcSeg.Parse(command1);
                if (set[0].Count > 0 && PcSetClass.IsValidSet(set[0]))
                {
                    sc[3].LoadFromPitchClassList(set[0]);
                    angle = PcSetClass.CalculateAngle(sc[0], sc[3]) * (180.0 / Math.PI);
                    Console.WriteLine("ANGLE: {0}°", angle);
                }
                else
                    Console.WriteLine("Invalid pcset");
            }
            else
            {
                if (command1 == "")
                {
                    Console.WriteLine("Enter pcset 1: ");
                    command1 = Console.ReadLine();
                    Console.WriteLine("Enter pcset 2: ");
                    command2 = Console.ReadLine();
                }
                set[0] = PcSeg.Parse(command1);
                set[1] = PcSeg.Parse(command2);
                if (set[0].Count > 0 && set[1].Count > 0 && PcSetClass.IsValidSet(set[0]) && PcSetClass.IsValidSet(set[1]))
                {
                    sc[3].LoadFromPitchClassList(set[0]);
                    sc[4].LoadFromPitchClassList(set[1]);
                    angle = PcSetClass.CalculateAngle(sc[3], sc[4]) * (180.0 / Math.PI);
                    Console.WriteLine("ANGLE: {0}°", angle);
                }
                else
                    Console.WriteLine("Invalid pcset(s)");
            }
        }

        /// <summary>
        /// Calculates prime forms of pcsets that the user enters
        /// </summary>
        public override void Calculate()
        {
            List<PitchClass> pc;
            string command;
            Console.Write("Enter pcs (q to quit) > ");
            command = Console.ReadLine().ToUpper();
            while (command[0] != 'Q')
            {
                pc = PcSeg.Parse(command);
                if (pc.Count > 0 && PcSetClass.IsValidSet(pc))
                {
                    sc[0].LoadFromPitchClassList(pc);
                    sc[1] = sc[0].GetComplement();
                    pcset[0] = sc[0].GetSetCopy();
                    if (sc[0].ForteName.Contains('Z'))
                    {
                        hasZ = true;
                        sc[2] = sc[0].GetZRelation();
                    }
                    else
                        hasZ = false;
                    k.LoadNexusSet(sc[0], SetComplexType.K);
                    kh.LoadNexusSet(sc[0], SetComplexType.Kh);
                    Info();
                    Console.Write("You entered ");
                    List<string> secondary = sc[0].FindTransformationName(pc);
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
                    Console.WriteLine("Invalid pcset");
                Console.Write("\nEnter pcs (q to quit) > ");
                command = Console.ReadLine().ToUpper();
            }
        }

        /// <summary>
        /// Displays a pcset complement
        /// </summary>
        /// <param name="command">A pcset string</param>
        public override void Complement(string command = "")
        {
            Console.WriteLine(string.Format("{0,-18}{1,-12}", "Complement pcset:", '{' +
                PcSeg.ToString(PcSet.ToSortedPcSeg(pcset[1])) + '}'));
        }

        /// <summary>
        /// Displays the prime form of a pcset complement
        /// </summary>
        public override void ComplementPrime()
        {
            if (preferForteName == PrimarySetName.Forte)
            {
                Console.WriteLine(string.Format("{0,-28}{1,-12}", "Complement Forte name:", sc[1].ForteName));
                Console.WriteLine(string.Format("{0,-28}{1,-12}", "Complement prime form name:", '(' + sc[1].PrimeFormName + ')'));
                Console.WriteLine(string.Format("{0,-28}{1,-12}", "Complement Carter name:", sc[1].CarterName));
            }
            else
            {
                Console.WriteLine(string.Format("{0,-28}{1,-12}", "Complement prime form name:", '(' + sc[1].PrimeFormName + ')'));
                Console.WriteLine(string.Format("{0,-28}{1,-12}", "Complement Forte name:", sc[1].ForteName));
                Console.WriteLine(string.Format("{0,-28}{1,-12}", "Complement Carter name:", sc[1].CarterName));
            }
            Console.WriteLine(string.Format("{0,-28}{1,-12}", "IC vector:", sc[1].ICVectorName));
        }

        /// <summary>
        /// Displays the K complex of a pcset
        /// </summary>
        /// <param name="command">A pcset string</param>
        public override void ComplexK(string command = "")
        {
            SortedDictionary<string, PcSetClass> cardinalityList;
            if (preferForteName == PrimarySetName.Forte)
                Console.WriteLine("Set-complex K about " + sc[0].ForteName + ":\n");
            else
                Console.WriteLine("Set-complex K about (" + sc[0].PrimeFormName + "):\n");

            for (int i = 1; i <= 12; i++)
            {
                if (k.Cardinalities[i] == 1)
                {
                    cardinalityList = k.GetCardinality(i);
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

        /// <summary>
        /// Displays the Kh complex of a pcset
        /// </summary>
        /// <param name="command">A pcset string</param>
        public override void ComplexKh(string command = "")
        {
            SortedDictionary<string, PcSetClass> cardinalityList;
            if (preferForteName == PrimarySetName.Forte)
                Console.WriteLine("Set-complex Kh about " + sc[0].ForteName + ":\n");
            else
                Console.WriteLine("Set-complex Kh about (" + sc[0].PrimeFormName + "):\n");

            for (int i = 1; i <= 12; i++)
            {
                if (kh.Cardinalities[i] == 1)
                {
                    cardinalityList = kh.GetCardinality(i);
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

        public override void FindK()
        {

        }

        public override void FindKh()
        {

        }

        /// <summary>
        /// Displays the ICV of a pcset
        /// </summary>
        /// <param name="command">A pcset string</param>
        public override void ICV(string command = "")
        {
            List<PitchClass> set;
            if (command == "")
                Console.WriteLine(string.Format("{0,-11}{1,-12}", "IC vector:", sc[0].ICVectorName));
            else
            {
                set = PcSeg.Parse(command);
                if (set.Count > 0 && PcSetClass.IsValidSet(set))
                {
                    sc[2].LoadFromPitchClassList(set);
                    Console.WriteLine(string.Format("{0,-11}{1,-12}", "IC vector:", sc[2].ICVectorName));
                }
                else
                    Console.WriteLine("Invalid pcset");
            }
        }

        /// <summary>
        /// Displays information about a pcset
        /// </summary>
        /// <param name="command">A pcset string</param>
        public override void Info(string command = "")
        {
            if (command == "")
            {
                Console.WriteLine(string.Format("{0,-17}{1,-12}", "Pcset:", '{' +
                    PcSeg.ToString(PcSet.ToSortedPcSeg(pcset[0])) + '}'));
                if (preferForteName == PrimarySetName.Forte)
                {
                    Console.WriteLine(string.Format("{0,-17}{1,-12}", "Forte name:", sc[0].ForteName));
                    Console.WriteLine(string.Format("{0,-17}{1,-12}", "Prime form name:", '(' + sc[0].PrimeFormName + ')'));
                    Console.WriteLine(string.Format("{0,-17}{1,-12}", "Carter name:", sc[0].CarterName));
                    Console.WriteLine(string.Format("{0,-17}{1,-12}", "IC vector:", sc[0].ICVectorName));
                    Console.WriteLine(string.Format("{0,-17}{1,-12}", "Complement:", sc[1].ForteName));
                }
                else
                {
                    Console.WriteLine(string.Format("{0,-17}{1,-12}", "Prime form name:", '(' + sc[0].PrimeFormName + ')'));
                    Console.WriteLine(string.Format("{0,-17}{1,-12}", "Forte name:", sc[0].ForteName));
                    Console.WriteLine(string.Format("{0,-17}{1,-12}", "Carter name:", sc[0].CarterName));
                    Console.WriteLine(string.Format("{0,-17}{1,-12}", "IC vector:", sc[0].ICVectorName));
                    Console.WriteLine(string.Format("{0,-17}{1,-12}", "Complement:", '(' + sc[1].PrimeFormName + ')'));
                }
            }
            else
            {
                List<PitchClass> list = PcSeg.Parse(command);
                if (list.Count > 0)
                {
                    sc[3].LoadFromPitchClassList(list);
                    sc[4] = sc[3].GetComplement();
                    Console.WriteLine(string.Format("{0,-17}{1,-12}", "Pcset:", '{' +
                    PcSeg.ToString(PcSeg.Sort(list)) + '}'));
                    if (preferForteName == PrimarySetName.Forte)
                    {
                        Console.WriteLine(string.Format("{0,-17}{1,-12}", "Forte name:", sc[3].ForteName));
                        Console.WriteLine(string.Format("{0,-17}{1,-12}", "Prime form name:", '(' + sc[3].PrimeFormName + ')'));
                        Console.WriteLine(string.Format("{0,-17}{1,-12}", "Carter name:", sc[3].CarterName));
                        Console.WriteLine(string.Format("{0,-17}{1,-12}", "IC vector:", sc[3].ICVectorName));
                        Console.WriteLine(string.Format("{0,-17}{1,-12}", "Complement:", sc[4].ForteName));
                    }
                    else
                    {
                        Console.WriteLine(string.Format("{0,-17}{1,-12}", "Prime form name:", '(' + sc[3].PrimeFormName + ')'));
                        Console.WriteLine(string.Format("{0,-17}{1,-12}", "Forte name:", sc[3].ForteName));
                        Console.WriteLine(string.Format("{0,-17}{1,-12}", "Carter name:", sc[3].CarterName));
                        Console.WriteLine(string.Format("{0,-17}{1,-12}", "IC vector:", sc[3].ICVectorName));
                        Console.WriteLine(string.Format("{0,-17}{1,-12}", "Complement:", '(' + sc[4].PrimeFormName + ')'));
                    }
                }
                else
                    Console.WriteLine("Invalid pcset");
            }
        }

        /// <summary>
        /// Produces the intersection of two pcsets
        /// </summary>
        /// <param name="command1">A pcset string</param>
        /// <param name="command2">A pcset string</param>
        public override void Intersect(string command1 = "", string command2 = "")
        {
            HashSet<PitchClass> set1;
            HashSet<PitchClass> set2;

            if (command1 == "")
            {
                Console.Write("Enter pcset 1 > ");
                command1 = Console.ReadLine();
                Console.Write("Enter pcset 2 > ");
                command2 = Console.ReadLine();
                set1 = PcSeg.ToPcSet(PcSeg.Parse(command1));
                set2 = PcSeg.ToPcSet(PcSeg.Parse(command2));
            }

            else if (command2 == "")
            {
                set1 = PcSet.Copy(pcset[0]);
                set2 = PcSeg.ToPcSet(PcSeg.Parse(command1));
            }

            else
            {
                set1 = PcSeg.ToPcSet(PcSeg.Parse(command1));
                set2 = PcSeg.ToPcSet(PcSeg.Parse(command2));
            }

            if (set1.Count == 0 || set2.Count == 0)
                Console.WriteLine("Invalid pcset(s)");
            else
            {
                set1.IntersectWith(set2);
                Console.WriteLine("Intersection: {" + PcSeg.ToString(PcSet.ToSortedPcSeg(set1)) + "}");
            }
        }

        /// <summary>
        /// Produces the maximum intersection of two pcsets
        /// </summary>
        /// <param name="command1">A pcset string</param>
        /// <param name="command2">A pcset string</param>
        public override void IntersectMax(string command1 = "", string command2 = "")
        {
            HashSet<PitchClass> set1;
            HashSet<PitchClass> set2;

            if (command1 == "")
            {
                Console.Write("Enter pcset 1 > ");
                command1 = Console.ReadLine();
                Console.Write("Enter pcset 2 > ");
                command2 = Console.ReadLine();
                set1 = PcSeg.ToPcSet(PcSeg.Parse(command1));
                set2 = PcSeg.ToPcSet(PcSeg.Parse(command2));
            }

            else if (command2 == "")
            {
                set1 = PcSet.Copy(pcset[0]);
                set2 = PcSeg.ToPcSet(PcSeg.Parse(command1));
            }

            else
            {
                set1 = PcSeg.ToPcSet(PcSeg.Parse(command1));
                set2 = PcSeg.ToPcSet(PcSeg.Parse(command2));
            }

            if (set1.Count == 0 || set2.Count == 0)
                Console.WriteLine("Invalid pcset(s)");
            else
            {
                HashSet<PitchClass> intersection = PcSet.IntersectMax(set1, set2);
                Console.WriteLine("Maximum intersection: {" + PcSeg.ToString(PcSet.ToSortedPcSeg(intersection)) + "}");
            }
        }

        /// <summary>
        /// Loads a pcset
        /// </summary>
        /// <param name="command">A pcset string</param>
        public override void Load(string command = "")
        {
            List<PitchClass> pc;
            if (command == "")
            {
                Console.Write("Enter pcset > ");
                command = Console.ReadLine();
            }
            pc = PcSeg.Parse(command);
            if (pc.Count > 0 && PcSetClass.IsValidSet(pc))
            {
                pcset[1].Clear();
                pcset[0] = PcSeg.ToPcSet(pc);
                sc[0].LoadFromPitchClassSet(pcset[0]);
                sc[1] = sc[0].GetComplement();
                if (sc[0].ForteName.Contains('Z'))
                {
                    hasZ = true;
                    sc[2] = sc[0].GetZRelation();
                }
                else
                    hasZ = false;
                for (int i = 0; i < 12; i++)
                {
                    PitchClass pclass = new PitchClass(i);
                    if (!pcset[0].Contains(pclass))
                        pcset[1].Add(pclass);
                }
                k.LoadNexusSet(sc[0], SetComplexType.K);
                kh.LoadNexusSet(sc[0], SetComplexType.Kh);
            }
            else
                Console.WriteLine("Invalid pcset");
        }

        /// <summary>
        /// Loads a pcset prime form
        /// </summary>
        /// <param name="command">A pcset string</param>
        public override void LoadPrime(string command = "")
        {
            List<PitchClass> pc;
            if (command == "")
            {
                pcset[0] = sc[0].GetSetCopy();
                pcset[1].Clear();
                for (int i = 0; i < 12; i++)
                {
                    PitchClass pclass = new PitchClass(i);
                    if (!pcset[0].Contains(pclass))
                        pcset[1].Add(pclass);
                }
            }
            else
            {
                pc = PcSeg.Parse(command);
                // If the user provided a Forte name
                if (command.Contains('-') && sc[0].IsValidName(command))
                {
                    pcset[1].Clear();
                    sc[0].LoadFromForteName(command);
                    sc[1] = sc[0].GetComplement();
                    pcset[0] = sc[0].GetSetCopy();
                    if (sc[0].ForteName.Contains('Z'))
                    {
                        hasZ = true;
                        sc[2] = sc[0].GetZRelation();
                    }
                    else
                        hasZ = false;
                    for (int i = 0; i < 12; i++)
                    {
                        PitchClass pclass = new PitchClass(i);
                        if (!pcset[0].Contains(pclass))
                            pcset[1].Add(pclass);
                    }
                    k.LoadNexusSet(sc[0], SetComplexType.K);
                    kh.LoadNexusSet(sc[0], SetComplexType.Kh);
                }
                // If the user provided a valid pcset
                else if (pc.Count > 0 && PcSetClass.IsValidSet(pc))
                {
                    pcset[1].Clear();
                    sc[0].LoadFromPitchClassList(pc);
                    sc[1] = sc[0].GetComplement();
                    pcset[0] = sc[0].GetSetCopy();
                    if (sc[0].ForteName.Contains('Z'))
                    {
                        hasZ = true;
                        sc[2] = sc[0].GetZRelation();
                    }
                    else
                        hasZ = false;
                    for (int i = 0; i < 12; i++)
                    {
                        PitchClass pclass = new PitchClass(i);
                        if (!pcset[0].Contains(pclass))
                            pcset[1].Add(pclass);
                    }
                    k.LoadNexusSet(sc[0], SetComplexType.K);
                    kh.LoadNexusSet(sc[0], SetComplexType.Kh);
                }
                else
                    Console.WriteLine("Invalid pcset");
            }
        }

        /// <summary>
        /// Searches all Tn and TnI forms of a pcset for a set of pcs
        /// </summary>
        /// <param name="command">The search string</param>
        public override void Search(string command = "")
        {
            List<PitchClass> pitchList;
            if (command != "")
            {
                List<Pair<string, HashSet<PitchClass>>> secondary;
                pitchList = PcSeg.Parse(command);
                if (pitchList.Count <= pcset[0].Count)
                {
                    secondary = PcSet.GetSecondaryForms(pcset[0], pitchList);
                    if (secondary.Count > 0)
                    {
                        Console.WriteLine("These pcs match forms");
                        string secondaryPitches;
                        List<PitchClass> list;
                        for (int i = 0; i < secondary.Count; i++)
                        {
                            secondaryPitches = "";
                            list = PcSet.ToSortedPcSeg(secondary[i].Item2);
                            foreach (PitchClass pc in list)
                                secondaryPitches += pc.PitchClassChar;
                            Console.WriteLine(string.Format("{0,-5}{1,-9}", secondary[i].Item1, secondaryPitches));
                        }
                    }
                    else
                        Console.WriteLine("No matches!");
                }
                else
                    Console.WriteLine("Too many pcs!");
            }
            else
            {
                Console.Write("Enter pcs (q to quit) > ");
                command = Console.ReadLine().ToUpper();
                while (command != "Q")
                {
                    pitchList = PcSeg.Parse(command);
                    List<Pair<string, HashSet<PitchClass>>> secondary = sc[0].GetSecondaryForms(pitchList);
                    if (secondary.Count > 0)
                    {
                        Console.WriteLine("These pcs match forms");
                        string secondaryPitches;
                        List<PitchClass> list;
                        for (int i = 0; i < secondary.Count; i++)
                        {
                            secondaryPitches = "";
                            list = PcSet.ToPcSeg(secondary[i].Item2);
                            foreach (PitchClass pc in list)
                                secondaryPitches += pc.PitchClassChar;
                            Console.WriteLine(string.Format("{0,-5}{1,-9}", secondary[i].Item1, secondaryPitches));
                        }
                    }
                    else
                        Console.WriteLine("No matches!");
                    Console.Write("Enter pcs (q to quit) > ");
                    command = Console.ReadLine().ToUpper();
                    pitchList.Clear();
                }
            }
        }

        /// <summary>
        /// Displays all subsets of a pcset
        /// </summary>
        /// <param name="command">A pcset string</param>
        public override void Subsets(string command = "")
        {
            List<HashSet<PitchClass>> subsets;
            List<HashSet<PitchClass>>.Enumerator e;
            if (command == "")
            {
                subsets = PcSet.Subsets(pcset[0]);
                e = subsets.GetEnumerator();
                e.MoveNext();

                for (int j = 0; j < subsets.Count; j += 5)
                {
                    for (int k = j; k < j + 5 && k < subsets.Count; k++)
                    {
                        Console.Write(string.Format("{0,-18}", '{' +
                            PcSeg.ToString(PcSet.ToSortedPcSeg(e.Current)) + '}'));
                        e.MoveNext();
                    }
                    Console.Write("\n");
                }
            }
            else
            {
                List<PitchClass> pcs = PcSeg.Parse(command);
                if (pcs.Count > 0 && PcSetClass.IsValidSet(pcs))
                {
                    subsets = PcSet.Subsets(PcSeg.ToPcSet(pcs));
                    e = subsets.GetEnumerator();
                    e.MoveNext();

                    for (int j = 0; j < subsets.Count; j += 5)
                    {
                        for (int k = j; k < j + 5 && k < subsets.Count; k++)
                        {
                            Console.Write(string.Format("{0,-18}", '{' +
                                PcSeg.ToString(PcSet.ToSortedPcSeg(e.Current)) + '}'));
                            e.MoveNext();
                        }
                        Console.Write("\n");
                    }
                }
                else
                    Console.WriteLine("Invalid pcset");
            }
        }

        /// <summary>
        /// Displays the prime form of all subset-classes of a pcset
        /// </summary>
        /// <param name="command">A pcset string</param>
        public override void SubsetsPrime(string command = "")
        {
            List<Pair<PcSetClass, int>> subsets;
            if (command != "")
            {
                List<PitchClass> pcs = PcSeg.Parse(command);
                if (pcs.Count > 0 && PcSetClass.IsValidSet(pcs))
                    sc[3].LoadFromPitchClassList(pcs);
                else
                    Console.WriteLine("Invalid pcset");
            }
            else
                sc[3].LoadFromPitchClassSet(sc[0].GetSetCopy());

            if (preferForteName == PrimarySetName.Forte)
            {
                Console.WriteLine("Subset-classes for " + sc[3].ForteName + ":\n");
                Console.WriteLine("Null:\nnull: 1\n");
            }
            else
            {
                Console.WriteLine("Subset-classes for (" + sc[3].PrimeFormName + "):\n");
                Console.WriteLine("Null:\n(null) - 1\n");
            }

            // Get all subset prime forms for the provided pcset
            for (int i = 1; i < sc[3].Count; i++)
            {
                subsets = sc[3].GetSubsets(i, preferForteName);
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
                Console.WriteLine(sc[3].SetType + "s:\n" + sc[3].ForteName + ": 1");
            else
                Console.WriteLine(sc[3].SetType + "s:\n(" + sc[3].PrimeFormName + ") - 1");
        }

        /// <summary>
        /// Transforms a pcset
        /// </summary>
        /// <param name="command">A transformation string</param>
        public override void Transform(string command)
        {
            HashSet<PitchClass> transformSet = pcset[0];  // The set to transform
            string[] forms;     // The transformations
            int length = 0;     // The length of the longest transformation
            bool valid = true;  // If the user provided a pcset, flags if it is invalid

            if (command.Contains(','))
                forms = command.Split(',');
            else if (command.Contains(", "))
                forms = command.Split(", ");
            else
                forms = command.Split(' ');

            // If the user provided a pcset at the end of the transformation string, we need to interpret that
            if (forms[forms.Length - 1][0] == 'A' || forms[forms.Length - 1][0] == 'a' || forms[forms.Length - 1][0] == 'B'
                || forms[forms.Length - 1][0] == 'b' || (forms[forms.Length - 1][0] >= '0' && forms[forms.Length - 1][0] <= '9'))
            {
                List<PitchClass> pc;
                string[] newForms = new string[forms.Length - 1];
                pc = PcSeg.Parse(forms[forms.Length - 1]);
                if (pc.Count > 0 && PcSetClass.IsValidSet(pc))
                {
                    transformSet = PcSeg.ToPcSet(pc);

                    // Eliminate the pcset from the list of transformations
                    for (int i = 0; i < forms.Length - 1; i++)
                        newForms[i] = forms[i];
                    forms = newForms;
                }
                else
                {
                    Console.WriteLine("Invalid pcset");
                    valid = false;
                }
            }

            // Proceed with transformations if the transform set is valid
            if (valid)
            {
                foreach (string form in forms)
                {
                    if (form.Length > length)
                        length = form.Length;
                }
                foreach (string form in forms)
                {
                    string trimForm = form.Trim();
                    if (trimForm.Length > 0)
                    {
                        if (PcSet.IsValidTransformation(trimForm))
                        {
                            List<PitchClass> transformation = PcSet.ToSortedPcSeg(PcSet.Transform(transformSet, trimForm));
                            Console.Write(string.Format("{0, -" + (length + 2).ToString() + "}{1}", trimForm + ": ", "{" + PcSeg.ToString(transformation) + "}"));
                            Console.Write('\n');
                        }
                        else
                            Console.WriteLine(string.Format("{0,-" + (length + 2).ToString() + "}{1}", trimForm + ": ", "Invalid transformation name"));
                    }
                }
            }
        }

        /// <summary>
        /// Performs a union
        /// </summary>
        /// <param name="command1">A pcset string</param>
        /// <param name="command2">A pcset string</param>
        public override void Union(string command1 = "", string command2 = "")
        {
            HashSet<PitchClass> set1;
            HashSet<PitchClass> set2;

            if (command1 == "")
            {
                Console.Write("Enter pcset 1 > ");
                command1 = Console.ReadLine();
                Console.Write("Enter pcset 2 > ");
                command2 = Console.ReadLine();
                set1 = PcSeg.ToPcSet(PcSeg.Parse(command1));
                set2 = PcSeg.ToPcSet(PcSeg.Parse(command2));
            }

            else if (command2 == "")
            {
                set1 = PcSet.Copy(pcset[0]);
                set2 = PcSeg.ToPcSet(PcSeg.Parse(command1));
            }

            else
            {
                set1 = PcSeg.ToPcSet(PcSeg.Parse(command1));
                set2 = PcSeg.ToPcSet(PcSeg.Parse(command2));
            }

            if (set1.Count == 0 || set2.Count == 0)
                Console.WriteLine("Invalid pcset(s)");
            else
            {
                set1.UnionWith(set2);
                Console.WriteLine("Union: {" + PcSeg.ToString(PcSet.ToSortedPcSeg(set1)) + "}");
            }
        }

        /// <summary>
        /// Performs a compact union
        /// </summary>
        /// <param name="command1">A pcset string</param>
        /// <param name="command2">A pcset string</param>
        public override void UnionCompact(string command1 = "", string command2 = "")
        {
            HashSet<PitchClass> set1;
            HashSet<PitchClass> set2;

            if (command1 == "")
            {
                Console.Write("Enter pcset 1 > ");
                command1 = Console.ReadLine();
                Console.Write("Enter pcset 2 > ");
                command2 = Console.ReadLine();
                set1 = PcSeg.ToPcSet(PcSeg.Parse(command1));
                set2 = PcSeg.ToPcSet(PcSeg.Parse(command2));
            }

            else if (command2 == "")
            {
                set1 = PcSet.Copy(pcset[0]);
                set2 = PcSeg.ToPcSet(PcSeg.Parse(command1));
            }

            else
            {
                set1 = PcSeg.ToPcSet(PcSeg.Parse(command1));
                set2 = PcSeg.ToPcSet(PcSeg.Parse(command2));
            }

            if (set1.Count == 0 || set2.Count == 0)
                Console.WriteLine("Invalid pcset(s)");
            else
            {
                HashSet<PitchClass> union = PcSet.UnionCompact(set1, set2);
                Console.WriteLine("Compact union: {" + PcSeg.ToString(PcSet.ToSortedPcSeg(union)) + "}");
            }
        }

        /// <summary>
        /// Displays the Z-relation of a pcset
        /// </summary>
        /// <param name="command">A command</param>
        public override void ZRelation(string command = "")
        {
            if (hasZ)
            {
                if (preferForteName == PrimarySetName.Forte)
                {
                    Console.WriteLine(string.Format("{0,-28}{1,-12}", "Z-relation Forte name:", sc[2].ForteName));
                    Console.WriteLine(string.Format("{0,-28}{1,-12}", "Z-relation prime form name:", '(' + sc[2].PrimeFormName + ')'));
                    Console.WriteLine(string.Format("{0,-28}{1,-12}", "Z-relation Carter name:", sc[2].CarterName));
                }
                else
                {
                    Console.WriteLine(string.Format("{0,-28}{1,-12}", "Z-relation prime form name:", '(' + sc[2].PrimeFormName + ')'));
                    Console.WriteLine(string.Format("{0,-28}{1,-12}", "Z-relation Forte name:", sc[2].ForteName));
                    Console.WriteLine(string.Format("{0,-28}{1,-12}", "Z-relation Carter name:", sc[2].CarterName));
                }
                Console.WriteLine(string.Format("{0,-28}{1,-12}", "IC vector:", sc[2].ICVectorName));
            }
            else
                Console.WriteLine("The current pcset, {" +
                PcSeg.ToString(PcSet.ToSortedPcSeg(pcset[0])) + "}, has no Z-relation.");
        }
    }
}
