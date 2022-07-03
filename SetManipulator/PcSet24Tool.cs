/* File: PcSet24Tool.cs
** Project: SetManipulator
** Author: Jeffrey Martin
**
** This file contains the implementation of the PcSet24Tool class. PcSet24Tool contains user-facing
** functionality for working with microtonal pcsets.
**
** Copyright © 2022 by Jeffrey Martin. All rights reserved.
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
    class PcSet24Tool : MusicTool
    {
        HashSet<PitchClass24>[] pcset;
        PcSetClass24[] sc;
        bool packFromRight;

        /// <summary>
        /// Whether or not to pack from the right
        /// </summary>
        public override bool PackFromRight
        {
            get { return packFromRight; }
            set
            {
                packFromRight = value;
                for (int i = 0; i < sc.Length; i++)
                    sc[i].PackFromRight = value;
            }
        }

        /// <summary>
        /// Initializes the PcSet24Tool
        /// </summary>
        public PcSet24Tool()
        {
            pcset = new HashSet<PitchClass24>[3] { new HashSet<PitchClass24>(), new HashSet<PitchClass24>(), new HashSet<PitchClass24>() };
            sc = new PcSetClass24[5] { new PcSetClass24(), new PcSetClass24(), new PcSetClass24(), new PcSetClass24(), new PcSetClass24() };
            packFromRight = true;
        }

        /// <summary>
        /// Calculates the ANGLE between two pcsets
        /// </summary>
        /// <param name="command1">A pcset string</param>
        /// <param name="command2">A pcset string</param>
        public override void Angle(string command1 = "", string command2 = "")
        {
            List<PitchClass24>[] set = new List<PitchClass24>[2];
            double angle;

            if (command1 != "" && command2 == "")
            {
                set[0] = PcSeg.Parse24(command1);
                if (set[0].Count > 0 && PcSetClass24.IsValidSet(set[0]))
                {
                    sc[3].LoadFromPitchClassList(set[0]);
                    angle = PcSetClass24.CalculateAngle(sc[0], sc[3]) * (180.0 / Math.PI);
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
                set[0] = PcSeg.Parse24(command1);
                set[1] = PcSeg.Parse24(command2);
                if (set[0].Count > 0 && set[1].Count > 0 && PcSetClass24.IsValidSet(set[0]) && PcSetClass24.IsValidSet(set[1]))
                {
                    sc[3].LoadFromPitchClassList(set[0]);
                    sc[4].LoadFromPitchClassList(set[1]);
                    angle = PcSetClass24.CalculateAngle(sc[3], sc[4]) * (180.0 / Math.PI);
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
            List<PitchClass24> pc;
            string command;
            Console.Write("Enter pcs (q to quit) > ");
            command = Console.ReadLine().ToUpper();
            while (command != "Q")
            {
                pc = PcSeg.Parse24(command);
                if (pc.Count > 0)
                {
                    sc[0].LoadFromPitchClassList(pc);
                    sc[1] = sc[0].GetComplement();
                    pcset[0] = PcSeg.ToPcSet(pc);
                    Info();
                    Console.Write("You entered ");
                    List<string> secondary = sc[0].FindTransformationName(PcSeg.ToPcSet(pc));
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
            Console.WriteLine(string.Format("{0,-28}{1,-12}", "Complement prime form name:", '[' + sc[1].PrimeFormName + ']'));
            Console.WriteLine(string.Format("{0,-28}{1,-12}", "IC vector:", sc[1].ICVectorName));
        }

        /// <summary>
        /// Displays the ICV of a pcset
        /// </summary>
        /// <param name="command">A pcset string</param>
        public override void ICV(string command = "")
        {
            List<PitchClass24> set;
            if (command == "")
                Console.WriteLine(string.Format("{0,-11}{1,-12}", "IC vector:", sc[0].ICVectorName));
            else
            {
                set = PcSeg.Parse24(command);
                if (set.Count > 0 && PcSetClass24.IsValidSet(set))
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

                Console.WriteLine(string.Format("{0,-17}{1,-12}", "Prime form name:", '[' + sc[0].PrimeFormName + ']'));
                Console.WriteLine(string.Format("{0,-17}{1,-12}", "IC vector:", sc[0].ICVectorName));
                Console.WriteLine(string.Format("{0,-17}{1,-12}", "Complement:", '[' + sc[1].PrimeFormName + ']'));
            }
            else
            {
                List<PitchClass24> list = PcSeg.Parse24(command);
                if (list.Count > 0)
                {
                    sc[3].LoadFromPitchClassList(list);
                    sc[4] = sc[3].GetComplement();
                    Console.WriteLine(string.Format("{0,-17}{1,-12}", "Pcset:", '{' +
                    PcSeg.ToString(PcSeg.Sort(list)) + '}'));
                    
                    Console.WriteLine(string.Format("{0,-17}{1,-12}", "Prime form name:", '[' + sc[3].PrimeFormName + ']'));
                    Console.WriteLine(string.Format("{0,-17}{1,-12}", "IC vector:", sc[3].ICVectorName));
                    Console.WriteLine(string.Format("{0,-17}{1,-12}", "Complement:", '[' + sc[4].PrimeFormName + ']'));
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
            HashSet<PitchClass24> set1;
            HashSet<PitchClass24> set2;

            if (command1 == "")
            {
                Console.Write("Enter pcset 1 > ");
                command1 = Console.ReadLine();
                Console.Write("Enter pcset 2 > ");
                command2 = Console.ReadLine();
                set1 = PcSeg.ToPcSet(PcSeg.Parse24(command1));
                set2 = PcSeg.ToPcSet(PcSeg.Parse24(command2));
            }

            else if (command2 == "")
            {
                set1 = PcSet.Copy(pcset[0]);
                set2 = PcSeg.ToPcSet(PcSeg.Parse24(command1));
            }

            else
            {
                set1 = PcSeg.ToPcSet(PcSeg.Parse24(command1));
                set2 = PcSeg.ToPcSet(PcSeg.Parse24(command2));
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
            HashSet<PitchClass24> set1;
            HashSet<PitchClass24> set2;

            if (command1 == "")
            {
                Console.Write("Enter pcset 1 > ");
                command1 = Console.ReadLine();
                Console.Write("Enter pcset 2 > ");
                command2 = Console.ReadLine();
                set1 = PcSeg.ToPcSet(PcSeg.Parse24(command1));
                set2 = PcSeg.ToPcSet(PcSeg.Parse24(command2));
            }

            else if (command2 == "")
            {
                set1 = PcSet.Copy(pcset[0]);
                set2 = PcSeg.ToPcSet(PcSeg.Parse24(command1));
            }

            else
            {
                set1 = PcSeg.ToPcSet(PcSeg.Parse24(command1));
                set2 = PcSeg.ToPcSet(PcSeg.Parse24(command2));
            }

            if (set1.Count == 0 || set2.Count == 0)
                Console.WriteLine("Invalid pcset(s)");
            else
            {
                HashSet<PitchClass24> intersection = PcSet.IntersectMax(set1, set2);
                Console.WriteLine("Maximum intersection: {" + PcSeg.ToString(PcSet.ToSortedPcSeg(intersection)) + "}");
            }
        }

        /// <summary>
        /// Loads a pcset
        /// </summary>
        /// <param name="command">A pcset string</param>
        public override void Load(string command = "")
        {
            List<PitchClass24> pc;
            if (command == "")
            {
                Console.Write("Enter pcset > ");
                command = Console.ReadLine();
            }
            pc = PcSeg.Parse24(command);
            if (pc.Count > 0 && PcSetClass24.IsValidSet(pc))
            {
                pcset[1].Clear();
                pcset[0] = PcSeg.ToPcSet(pc);
                sc[0].LoadFromPitchClassSet(pcset[0]);
                sc[1] = sc[0].GetComplement();
                for (int i = 0; i < 24; i++)
                {
                    PitchClass24 pclass = new PitchClass24(i);
                    if (!pcset[0].Contains(pclass))
                        pcset[1].Add(pclass);
                }
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
            List<PitchClass24> pc;
            if (command == "")
            {
                pcset[0] = sc[0].GetSetCopy();
                pcset[1].Clear();
                for (int i = 0; i < 12; i++)
                {
                    PitchClass24 pclass = new PitchClass24(i);
                    if (!pcset[0].Contains(pclass))
                        pcset[1].Add(pclass);
                }
            }
            else
            {
                pc = PcSeg.Parse24(command);
                
                // If the user provided a valid pcset
                if (pc.Count > 0 && PcSetClass24.IsValidSet(pc))
                {
                    pcset[1].Clear();
                    sc[0].LoadFromPitchClassList(pc);
                    sc[1] = sc[0].GetComplement();
                    pcset[0] = sc[0].GetSetCopy();
                    for (int i = 0; i < 24; i++)
                    {
                        PitchClass24 pclass = new PitchClass24(i);
                        if (!pcset[0].Contains(pclass))
                            pcset[1].Add(pclass);
                    }
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
            List<PitchClass24> pitchList;
            if (command != "")
            {
                List<Pair<string, HashSet<PitchClass24>>> secondary;
                pitchList = PcSeg.Parse24(command);
                if (pitchList.Count <= pcset[0].Count)
                {
                    secondary = PcSet.GetSecondaryForms(pcset[0], pitchList);
                    if (secondary.Count > 0)
                    {
                        Console.WriteLine("These pcs match forms");
                        string secondaryPitches;
                        List<PitchClass24> list;
                        for (int i = 0; i < secondary.Count; i++)
                        {
                            secondaryPitches = "";
                            list = PcSet.ToSortedPcSeg(secondary[i].Item2);
                            secondaryPitches = PcSeg.ToString(list);
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
                    pitchList = PcSeg.Parse24(command);
                    List<Pair<string, HashSet<PitchClass24>>> secondary = sc[0].GetSecondaryForms(pitchList);
                    if (secondary.Count > 0)
                    {
                        Console.WriteLine("These pcs match forms");
                        string secondaryPitches;
                        List<PitchClass24> list;
                        for (int i = 0; i < secondary.Count; i++)
                        {
                            secondaryPitches = "";
                            list = PcSet.ToPcSeg(secondary[i].Item2);
                            secondaryPitches = PcSeg.ToString(list);
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
            List<List<PitchClass24>> orderedSubsets = new List<List<PitchClass24>>();
            List<List<PitchClass24>>.Enumerator e;
            if (command == "")
            {
                foreach (HashSet<PitchClass24> set in PcSet.Subsets(pcset[0]))
                    orderedSubsets.Add(PcSet.ToPcSeg(set));
                foreach (List<PitchClass24> list in orderedSubsets)
                    list.Sort();

                orderedSubsets = PcSeg.SortPcsegList(orderedSubsets);

                e = orderedSubsets.GetEnumerator();
                e.MoveNext();

                for (int j = 0; j < orderedSubsets.Count; j += 4)
                {
                    for (int k = j; k < j + 4 && k < orderedSubsets.Count; k++)
                    {
                        Console.Write(string.Format("{0,-29}", '{' +
                            PcSeg.ToString(e.Current) + '}'));
                        e.MoveNext();
                    }
                    Console.Write("\n");
                }
            }
            else
            {
                HashSet<PitchClass24> pcs = PcSeg.ToPcSet(PcSeg.Parse24(command));
                if (pcs.Count > 0)
                {
                    foreach (HashSet<PitchClass24> set in PcSet.Subsets(pcs))
                        orderedSubsets.Add(PcSet.ToPcSeg(set));
                    foreach (List<PitchClass24> list in orderedSubsets)
                        list.Sort();

                    orderedSubsets = PcSeg.SortPcsegList(orderedSubsets);

                    e = orderedSubsets.GetEnumerator();
                    e.MoveNext();

                    for (int j = 0; j < orderedSubsets.Count; j += 4)
                    {
                        for (int k = j; k < j + 4 && k < orderedSubsets.Count; k++)
                        {
                            Console.Write(string.Format("{0,-29}", '{' +
                                PcSeg.ToString(e.Current) + '}'));
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
            List<Pair<PcSetClass24, int>> subsets;
            if (command != "")
            {
                List<PitchClass24> pcs = PcSeg.Parse24(command);
                if (pcs.Count > 0 && PcSetClass24.IsValidSet(pcs))
                    sc[3].LoadFromPitchClassList(pcs);
                else
                    Console.WriteLine("Invalid pcset");
            }
            else
                sc[3].LoadFromPitchClassSet(sc[0].GetSetCopy());

            Console.WriteLine("Subset-classes for (" + sc[3].PrimeFormName + "):\n");
            Console.WriteLine("Null:\n(null) - 1\n");

            // Get all subset prime forms for the provided pcset
            for (int i = 1; i < sc[3].Count; i++)
            {
                subsets = sc[3].GetSubsets(i);
                Console.WriteLine("PCs: " + subsets[0].Item1.Count.ToString());
                for (int j = 0; j < subsets.Count; j += 4)
                {
                    for (int k = j; k < j + 4 && k < subsets.Count; k++)
                        Console.Write(string.Format("{0,-29}", '[' + subsets[k].Item1.PrimeFormName + ") - " + subsets[k].Item2.ToString()));
                    Console.Write("\n");
                }
                Console.Write("\n");
            }
        }

        /// <summary>
        /// Transforms a pcset
        /// </summary>
        /// <param name="command">A transformation string</param>
        public override void Transform(string command)
        {
            HashSet<PitchClass24> transformSet = pcset[0];  // The set to transform
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
                List<PitchClass24> pc;
                string[] newForms = new string[forms.Length - 1];
                pc = PcSeg.Parse24(forms[forms.Length - 1]);
                if (pc.Count > 0 && PcSetClass24.IsValidSet(pc))
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
                            List<PitchClass24> transformation = PcSet.ToSortedPcSeg(PcSet.Transform(transformSet, trimForm));
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
            HashSet<PitchClass24> set1;
            HashSet<PitchClass24> set2;

            if (command1 == "")
            {
                Console.Write("Enter pcset 1 > ");
                command1 = Console.ReadLine();
                Console.Write("Enter pcset 2 > ");
                command2 = Console.ReadLine();
                set1 = PcSeg.ToPcSet(PcSeg.Parse24(command1));
                set2 = PcSeg.ToPcSet(PcSeg.Parse24(command2));
            }

            else if (command2 == "")
            {
                set1 = PcSet.Copy(pcset[0]);
                set2 = PcSeg.ToPcSet(PcSeg.Parse24(command1));
            }

            else
            {
                set1 = PcSeg.ToPcSet(PcSeg.Parse24(command1));
                set2 = PcSeg.ToPcSet(PcSeg.Parse24(command2));
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
            HashSet<PitchClass24> set1;
            HashSet<PitchClass24> set2;

            if (command1 == "")
            {
                Console.Write("Enter pcset 1 > ");
                command1 = Console.ReadLine();
                Console.Write("Enter pcset 2 > ");
                command2 = Console.ReadLine();
                set1 = PcSeg.ToPcSet(PcSeg.Parse24(command1));
                set2 = PcSeg.ToPcSet(PcSeg.Parse24(command2));
            }

            else if (command2 == "")
            {
                set1 = PcSet.Copy(pcset[0]);
                set2 = PcSeg.ToPcSet(PcSeg.Parse24(command1));
            }

            else
            {
                set1 = PcSeg.ToPcSet(PcSeg.Parse24(command1));
                set2 = PcSeg.ToPcSet(PcSeg.Parse24(command2));
            }

            if (set1.Count == 0 || set2.Count == 0)
                Console.WriteLine("Invalid pcset(s)");
            else
            {
                HashSet<PitchClass24> union = PcSet.UnionCompact(set1, set2);
                Console.WriteLine("Compact union: {" + PcSeg.ToString(PcSet.ToSortedPcSeg(union)) + "}");
            }
        }
    }
}
