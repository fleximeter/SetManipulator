/* File: PSetTool.cs
** Project: SetManipulator
** Author: Jeffrey Martin
**
** This file contains the implementation of the PSetTool class. PSetTool contains user-facing
** functionality for working with psets.
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
    class PSetTool : MusicTool
    {
        HashSet<Pitch>[] _psets;

        /// <summary>
        /// Initializes the PSetTool
        /// </summary>
        public PSetTool()
        {
            _psets = new HashSet<Pitch>[3] { new HashSet<Pitch>(), new HashSet<Pitch>(), new HashSet<Pitch>() };
        }

        /// <summary>
        /// Displays information about a PSet
        /// </summary>
        /// <param name="command">A PSet</param>
        public override void Info(string command = "")
        {
            if (command == "")
                Console.WriteLine("Pset: {" + PSet.ToString(_psets[0], ", ") + '}');
            else
            {
                List<Pitch> list = PSeg.Sort(PSeg.Parse(command));
                if (list.Count == 0)
                    Console.WriteLine("Invalid pitch(es)");
                else
                {
                    _psets[1] = PSeg.ToPSet(list);
                }
                Console.WriteLine("Pset: {" + PSet.ToString(_psets[1], ", ") + '}');
            }
        }

        /// <summary>
        /// Loads a PSet
        /// </summary>
        /// <param name="command">A PSet</param>
        public override void Load(string command = "")
        {
            if (command == "")
            {
                Console.Write("Enter pitches > ");
                command = Console.ReadLine();
            }
            List<Pitch> list = PSeg.Sort(PSeg.Parse(command));
            if (list.Count == 0)
                Console.WriteLine("Invalid pitch(es)");
            else
            {
                _psets[0] = PSeg.ToPSet(list);
            }
        }

        public override void Search(string command = "")
        {
            //List<Pitch> pitchList;
            //if (command != "")
            //{
            //    List<Pair<string, HashSet<Pitch>>> secondary;
            //    pitchList = Functions.ConvertPStringToList(command);
            //    if (pitchList.Count <= _psets[0].Count)
            //    {
            //        secondary = PSet.GetSecondaryForms(_psets[0], pitchList);
            //        if (secondary.Count > 0)
            //        {
            //            Console.WriteLine("These pitches match forms");
            //            string secondaryPitches;
            //            List<Pitch> list;
            //            for (int i = 0; i < secondary.Count; i++)
            //            {
            //                secondaryPitches = "";
            //                list = Functions.ConvertPSetToSortedList(secondary[i].Item2);
            //                foreach (PitchClass pc in list)
            //                    secondaryPitches += pc.PitchClassChar;
            //                Console.WriteLine(string.Format("{0,-5}{1,-9}", secondary[i].Item1, secondaryPitches));
            //            }
            //        }
            //        else
            //            Console.WriteLine("No matches!");
            //    }
            //    else
            //        Console.WriteLine("Too many pitches!");
            //}
            //else
            //{
            //    Console.Write("Enter pitches (q to quit) > ");
            //    command = Console.ReadLine().ToUpper();
            //    while (command != "Q")
            //    {
            //        pitchList = Functions.ConvertPStringToList(command);
            //        List<Pair<string, HashSet<Pitch>>> secondary = sc[0].GetSecondaryForms(pitchList);
            //        if (secondary.Count > 0)
            //        {
            //            Console.WriteLine("These pitches match forms");
            //            string secondaryPitches;
            //            List<Pitch> list;
            //            for (int i = 0; i < secondary.Count; i++)
            //            {
            //                secondaryPitches = "";
            //                list = Functions.ConvertPSetToList(secondary[i].Item2);
            //                foreach (PitchClass pc in list)
            //                    secondaryPitches += pc.PitchClassChar;
            //                Console.WriteLine(string.Format("{0,-5}{1,-9}", secondary[i].Item1, secondaryPitches));
            //            }
            //        }
            //        else
            //            Console.WriteLine("No matches!");
            //        Console.Write("Enter pitches (q to quit) > ");
            //        command = Console.ReadLine().ToUpper();
            //        pitchList.Clear();
            //    }
            //}
        }

        /// <summary>
        /// Displays the subsets of a PSet
        /// </summary>
        /// <param name="command">A PSet</param>
        public override void Subsets(string command = "")
        {
            List<HashSet<Pitch>> subsets;
            List<HashSet<Pitch>>.Enumerator e;
            if (command == "")
            {
                subsets = PSet.Subsets(_psets[0]);
                e = subsets.GetEnumerator();
                e.MoveNext();

                for (int j = 0; j < subsets.Count; j += 5)
                {
                    for (int k = j; k < j + 5 && k < subsets.Count; k++)
                    {
                        Console.Write(string.Format("{0,-18}", '{' +
                            PSeg.ToString(PSet.ToSortedPSeg(e.Current)) + '}'));
                        e.MoveNext();
                    }
                    Console.Write("\n");
                }
            }
            else
            {
                List<Pitch> ps = PSeg.Parse(command);
                if (ps.Count > 0 && PSet.IsValidSet(ps))
                {
                    subsets = PSet.Subsets(PSeg.ToPSet(ps));
                    e = subsets.GetEnumerator();
                    e.MoveNext();

                    for (int j = 0; j < subsets.Count; j += 5)
                    {
                        for (int k = j; k < j + 5 && k < subsets.Count; k++)
                        {
                            Console.Write(string.Format("{0,-18}", '{' +
                                PSeg.ToString(PSet.ToSortedPSeg(e.Current)) + '}'));
                            e.MoveNext();
                        }
                        Console.Write("\n");
                    }
                }
                else
                    Console.WriteLine("Invalid pset");
            }
        }

        /// <summary>
        /// Transforms a PSet
        /// </summary>
        /// <param name="command">A transformation string</param>
        public override void Transform(string command)
        {
            HashSet<Pitch> transformSet = _psets[0];  // The set to transform
            string[] forms;     // The transformations
            int length = 0;     // The length of the longest transformation
            bool valid = true;  // If the user provided a pcset, flags if it is invalid

            if (command.Contains(','))
                forms = command.Split(',');
            else if (command.Contains(", "))
                forms = command.Split(", ");
            else
                forms = command.Split(' ');

            // If the user provided a pset at the end of the transformation string, we need to interpret that
            if (forms[forms.Length - 1][0] >= '0' && forms[forms.Length - 1][0] <= '9')
            {
                List<Pitch> pc;
                string[] newForms = new string[forms.Length - 1];
                pc = PSeg.Parse(forms[forms.Length - 1]);
                if (pc.Count > 0)
                {
                    transformSet = PSeg.ToPSet(pc);

                    // Eliminate the pcset from the list of transformations
                    for (int i = 0; i < forms.Length - 1; i++)
                        newForms[i] = forms[i];
                    forms = newForms;
                }
                else
                {
                    Console.WriteLine("Invalid pset");
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
                        if (PSet.IsValidTransformation(trimForm))
                        {
                            List<Pitch> transformation = PSet.ToSortedPSeg(PSet.Transform(transformSet, trimForm));
                            Console.Write(string.Format("{0, -" + (length + 2).ToString() + "}{1}", trimForm + ": ", "{" + PSeg.ToString(transformation) + "}"));
                            Console.Write('\n');
                        }
                        else
                            Console.WriteLine(string.Format("{0,-" + (length + 2).ToString() + "}{1}", trimForm + ": ", "Invalid transformation name"));
                    }
                }
            }
        }
    }
}
