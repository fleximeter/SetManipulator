/* File: PcSeg24Tool.cs
** Project: SetManipulator
** Author: Jeffrey Martin
**
** This file contains the implementation of the PcSeg24Tool class. PcSegTool contains user-facing
** functionality for working with microtonal pcsegs.
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
using System.Text;
using MusicTheory;

namespace SetManipulator
{
    class PcSeg24Tool : MusicTool
    {
        List<PitchClass24>[] _pcseg;
        List<PitchClass24> _intervals;

        /// <summary>
        /// Initializes the PcSegTool
        /// </summary>
        public PcSeg24Tool()
        {
            _pcseg = new List<PitchClass24>[2] { new List<PitchClass24>(), new List<PitchClass24>() };
            _intervals = new List<PitchClass24>();
        }

        /// <summary>
        /// Displays the complement of a pcseg
        /// </summary>
        /// <param name="command">A pcseg</param>
        public override void Complement(string command = "")
        {
            if (command == "")
            {
                Console.WriteLine("Complement of <" + PcSeg.ToString(_pcseg[0], " ") +
                    ">: {" + PcSeg.ToString(_pcseg[1], "") + "}");
            }
            else
            {
                List<PitchClass24> list = PcSeg.Parse24(command);
                List<PitchClass24> complement = PcSeg.Complement(list);
                Console.WriteLine("Complement of <" + PcSeg.ToString(list, " ") +
                    ">: {" + PcSeg.ToString(complement, "") + "}");
            }
        }

        /// <summary>
        /// Displays info about a pcseg or row
        /// </summary>
        /// <param name="command">A pcseg or row</param>
        public override void Info(string command = "")
        {
            List<PitchClass24> pcseg = _pcseg[0];
            if (command != "")
                pcseg = PcSeg.Parse24(command);
            Console.WriteLine("Pcseg: <" + PcSeg.ToString(pcseg, " ") + ">");
        }

        /// <summary>
        /// Displays a list of imbricated set-classes
        /// </summary>
        /// <param name="command">The cardinality of the imbricated set-classes</param>
        public override void imb(string command = "")
        {
            List<PitchClass24> pcseg;
            pcseg = _pcseg[0];

            if (command == "")
            {
                Console.Write("Enter cardinality n: ");
                command = Console.ReadLine();
            }

            uint n = uint.Parse(command);
            List<PcSetClass24> scs = PcSeg.imb_n(pcseg, n);
            
            for (int i = 0; i < scs.Count; i++)
            {
                List<PitchClass24> imbn = new List<PitchClass24>();
                for (int j = i; j < i + n; j++)
                    imbn.Add(pcseg[j]);
                Console.Write("<" + PcSeg.ToString(imbn, " ") + ">");
                Console.WriteLine("  |  " + "[" + scs[i].PrimeFormName + "]");
            }
        }

        /// <summary>
        /// Displays the interval sequence of a pcseg or row
        /// </summary>
        /// <param name="command">A pcseg</param>
        public override void Intervals(string command = "")
        {
            if (command == "")
                Console.WriteLine("Intervals: <" + PcSeg.ToString(_intervals, " ") + ">");
            else if (command[0] == 'T' || command[0] == 'M' || command[0] == 'I' || command[0] == 'R' || command[0] == 'r')
            {
                List<PitchClass24> intervals;
                if (PcSeg.IsValidTransformation(command))
                {
                    List<PitchClass24> secondaryForm = PcSeg.Transform(_pcseg[0], command);
                    intervals = PcSeg.Intervals(secondaryForm);
                    Console.WriteLine("Intervals: <" + PcSeg.ToString(intervals, " ") + ">");
                }
                else
                    Console.WriteLine(command + ": Invalid transformation");
            }
            else
            {
                List<PitchClass24> list = PcSeg.Parse24(command);
                List<PitchClass24> intervals = PcSeg.Intervals(list);
                Console.WriteLine("Intervals: <" + PcSeg.ToString(intervals, " ") + ">");
            }
        }

        /// <summary>
        /// Loads a pcseg or row
        /// </summary>
        /// <param name="command">A pcseg or row</param>
        public override void Load(string command = "")
        {
            List<PitchClass24> list;
            if (command == "")
            {
                Console.Write("Enter pcseg > ");
                command = Console.ReadLine();
            }
            list = PcSeg.Parse24(command);
            if (list.Count > 0)
            {
                _pcseg[0] = list;
                _pcseg[1] = PcSeg.Complement(_pcseg[0]);
                _intervals = PcSeg.Intervals(_pcseg[0]);
            }
            else
                Console.WriteLine("Invalid pcseg");
        }

        /// <summary>
        /// Searches transformations of the current pcseg or row
        /// </summary>
        /// <param name="command"></param>
        public override void OrderedSearch(string command = "")
        {
            List<PitchClass24> pc;
            List<Pair<string, List<PitchClass24>>> rowForms;
            if (command == "")
            {
                Console.Write("Enter pcs (q to quit)\n> ");
                command = Console.ReadLine();
                while (command != "q" && command != "Q")
                {
                    pc = PcSeg.Parse24(command);
                    if (pc.Count > 0)
                    {
                        rowForms = PcSeg.GetSecondaryForms(_pcseg[0], pc);
                        foreach (Pair<string, List<PitchClass24>> rowForm in rowForms)
                        {
                            Console.Write(string.Format("{0,-7}", rowForm.Item1 + ":"));
                            Console.WriteLine("<" + PcSeg.ToString(rowForm.Item2, " ") + ">");
                        }
                        if (rowForms.Count == 0)
                            Console.WriteLine("No row forms match your search term.");
                    }
                    else
                        Console.WriteLine("Invalid pc string");
                    Console.Write("Enter pcs (q to quit)\n> ");
                    command = Console.ReadLine();
                }
            }
            else
            {
                pc = PcSeg.Parse24(command);
                if (pc.Count > 0)
                {
                    rowForms = PcSeg.GetSecondaryForms(_pcseg[0], pc);
                    foreach (Pair<string, List<PitchClass24>> rowForm in rowForms)
                    {
                        Console.Write(string.Format("{0,-7}", rowForm.Item1 + ":"));
                        Console.WriteLine("<" + PcSeg.ToString(rowForm.Item2, " ") + ">");
                    }
                    if (rowForms.Count == 0)
                        Console.WriteLine("No row forms match your search term.");
                }
                else
                    Console.WriteLine("Invalid pc string");
            }
        }

        /// <summary>
        /// Searches all Tn and TnI forms of a pcset for a set of pcs
        /// </summary>
        /// <param name="command">The search string</param>
        public override void Search(string command = "")
        {
            List<PitchClass24> pc;
            List<Pair<string, List<PitchClass24>>> rowForms;
            if (command == "")
            {
                Console.Write("Enter pcs (q to quit)\n> ");
                command = Console.ReadLine();
                while (command != "q" && command != "Q")
                {
                    pc = PcSeg.Parse24(command);
                    if (pc.Count > 0)
                    {
                        rowForms = PcSeg.GetSecondaryFormsUnordered(_pcseg[0], pc);
                        foreach (Pair<string, List<PitchClass24>> rowForm in rowForms)
                        {
                            Console.Write(string.Format("{0,-7}", rowForm.Item1 + ":"));
                            Console.WriteLine("<" + PcSeg.ToString(rowForm.Item2, " ") + ">");
                        }
                        if (rowForms.Count == 0)
                            Console.WriteLine("No row forms match your search term.");
                    }
                    else
                        Console.WriteLine("Invalid pc string");
                    Console.Write("Enter pcs (q to quit)\n> ");
                    command = Console.ReadLine();
                }
            }
            else
            {
                pc = PcSeg.Parse24(command);
                if (pc.Count > 0)
                {
                    rowForms = PcSeg.GetSecondaryFormsUnordered(_pcseg[0], pc);
                    foreach (Pair<string, List<PitchClass24>> rowForm in rowForms)
                    {
                        Console.Write(string.Format("{0,-7}", rowForm.Item1 + ":"));
                        Console.WriteLine("<" + PcSeg.ToString(rowForm.Item2, " ") + ">");
                    }
                    if (rowForms.Count == 0)
                        Console.WriteLine("No row forms match your search term.");
                }
                else
                    Console.WriteLine("Invalid pc string");
            }
        }

        /// <summary>
        /// Displays all subsegs of a pcseg
        /// </summary>
        /// <param name="command">A pcseg string</param>
        public override void Subsegs(string command = "")
        {
            List<List<PitchClass24>> subsegs;
            List<List<PitchClass24>>.Enumerator e;
            if (command == "")
            {
                subsegs = PcSeg.Subsegs(_pcseg[0]);
                subsegs = PcSeg.SortPcsegList(subsegs);

                e = subsegs.GetEnumerator();
                e.MoveNext();

                for (int j = 0; j < subsegs.Count; j += 5)
                {
                    for (int k = j; k < j + 5 && k < subsegs.Count; k++)
                    {
                        Console.Write(string.Format("{0,-18}", '<' +
                            PcSeg.ToString(e.Current) + '>'));
                        e.MoveNext();
                    }
                    Console.Write("\n");
                }
            }
            else
            {
                List<PitchClass24> pcs = PcSeg.Parse24(command);
                if (pcs.Count > 0)
                {
                    subsegs = PcSeg.Subsegs(pcs);
                    subsegs = PcSeg.SortPcsegList(subsegs);

                    e = subsegs.GetEnumerator();
                    e.MoveNext();

                    for (int j = 0; j < subsegs.Count; j += 5)
                    {
                        for (int k = j; k < j + 5 && k < subsegs.Count; k++)
                        {
                            Console.Write(string.Format("{0,-18}", '<' +
                                PcSeg.ToString(e.Current) + '>'));
                            e.MoveNext();
                        }
                        Console.Write("\n");
                    }
                }
                else
                    Console.WriteLine("Invalid pcseg");
            }
        }

        /// <summary>
        /// Transforms a pcseg or row
        /// </summary>
        /// <param name="command">A transformation string</param>
        public override void Transform(string command)
        {
            List<PitchClass24> transformList = _pcseg[0];  // The pcseg to transform
            string[] forms;       // The transformations
            int length = 0;       // The length of the longest transformation
            bool valid = true;    // If the user provided a pcseg, flags if it is invalid
            bool custom = false;  // Tracks if the user provided a pcseg

            if (command.Contains(','))
                forms = command.Split(',');
            else if (command.Contains(", "))
                forms = command.Split(", ");
            else
                forms = command.Split(' ');

            // If the user provided a pcseg at the end of the transformation string, we need to interpret that
            if (forms[forms.Length - 1][0] == 'A' || forms[forms.Length - 1][0] == 'a' || forms[forms.Length - 1][0] == 'B'
                || forms[forms.Length - 1][0] == 'b' || (forms[forms.Length - 1][0] >= '0' && forms[forms.Length - 1][0] <= '9'))
            {
                custom = true;
                string[] newForms = new string[forms.Length - 1];
                transformList = PcSeg.Parse24(forms[forms.Length - 1]);
                if (transformList.Count > 0)
                {
                    // Eliminate the pcseg from the list of transformations
                    for (int i = 0; i < forms.Length - 1; i++)
                        newForms[i] = forms[i];
                    forms = newForms;
                }
                else
                {
                    Console.WriteLine("Invalid pcseg");
                    valid = false;
                }
            }

            // Proceed with transformations if the transform set is valid
            if (valid)
            {
                // Set the max length for formatting
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
                        if (PcSeg.IsValidTransformation(trimForm))
                        {
                            List<PitchClass24> secondaryForm = PcSeg.Transform(transformList, trimForm);
                            Console.WriteLine(string.Format("{0,-" + (length + 2).ToString() + "}{1}", trimForm + ":", "<" + PcSeg.ToString(secondaryForm, " ") + ">"));
                        }
                        else
                            Console.WriteLine(string.Format("{0,-" + (length + 2).ToString() + "}{1}", trimForm + ": ", "Invalid transformation"));
                    }
                }
            }
        }
    }
}
