/* File: PSegTool.cs
** Project: SetManipulator
** Author: Jeffrey Martin
**
** This file contains the implementation of the PSegTool class. PSegTool contains user-facing
** functionality for working with psegs.
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
using System.Text;
using MusicTheory;

namespace SetManipulator
{
    class PSegTool : MusicTool
    {
        List<Pitch>[] _psegs;
        List<Pitch> _intervals;

        /// <summary>
        /// Initializes the PSegTool
        /// </summary>
        public PSegTool()
        {
            _psegs = new List<Pitch>[5] { new List<Pitch>(), new List<Pitch>(), new List<Pitch>(), new List<Pitch>(), new List<Pitch>() };
            _intervals = new List<Pitch>();
        }

        /// <summary>
        /// Displays information about a pseg
        /// </summary>
        /// <param name="command">A pseg</param>
        public override void Info(string command = "")
        {
            Console.WriteLine("Pseg: <" + PSeg.ToString(_psegs[0], " ") + ">");
        }

        /// <summary>
        /// Loads a pseg
        /// </summary>
        /// <param name="command">A pseg</param>
        public override void Load(string command = "")
        {
            List<Pitch> list;
            if (command == "")
            {
                Console.Write("Enter pseg > ");
                command = Console.ReadLine();
            }
            list = PSeg.Parse(command);
            if (list.Count > 0)
            {
                _psegs[0] = list;
                _intervals = PSeg.Intervals(_psegs[0]);
            }
            else
                Console.WriteLine("Invalid pseg");
        }

        /// <summary>
        /// Displays the interval sequence of a pseg
        /// </summary>
        /// <param name="command">A pseg</param>
        public override void Intervals(string command = "")
        {
            if (command == "")
                Console.WriteLine("Intervals: <" + PSeg.ToString(_intervals, " ") + ">");
            else if (command[0] == 'T' || command[0] == 'M' || command[0] == 'I' || command[0] == 'R' || command[0] == 'r')
            {
                List<Pitch> intervals;
                
                if (PSeg.IsValidTransformation(command))
                {
                    List<Pitch> secondaryForm = PSeg.Transform(_psegs[0], command);
                    intervals = PSeg.Intervals(secondaryForm);
                    Console.WriteLine("Intervals: <" + PSeg.ToString(intervals, " ") + ">");
                }
                else
                    Console.WriteLine(command + ": Invalid transformation");
            }
            else
            {
                List<Pitch> list = PSeg.Parse(command);
                List<Pitch> intervals = PSeg.Intervals(list);
                Console.WriteLine("Intervals: <" + PSeg.ToString(intervals, " ") + ">");
            }
        }

        public override void Search(string command = "")
        {
            //List<Pitch> p;
            //List<Pair<string, List<Pitch>>> rowForms;
            //if (command == "")
            //{
            //    Console.Write("Enter pitches (q to quit)\n> ");
            //    command = Console.ReadLine();
            //    while (command != "q" && command != "Q")
            //    {
            //        p = Functions.ConvertPStringToList(command);
            //        if (p.Count > 0)
            //        {
            //            rowForms = PSeg.GetSecondaryForms(_psegs[0], p);
            //            foreach (Pair<string, List<Pitch>> rowForm in rowForms)
            //            {
            //                Console.Write(string.Format("{0,-7}", rowForm.Item1 + ":"));
            //                Console.WriteLine("<" + Functions.ConvertPListToString(rowForm.Item2, " ") + ">");
            //            }
            //            if (rowForms.Count == 0)
            //                Console.WriteLine("No row forms match your search term.");
            //        }
            //        else
            //            Console.WriteLine("Invalid pitch string");
            //        Console.Write("Enter pitches (q to quit)\n> ");
            //        command = Console.ReadLine();
            //    }
            //}
            //else
            //{
            //    p = Functions.ConvertPStringToList(command);
            //    if (p.Count > 0)
            //    {
            //        rowForms = PSeg.GetSecondaryForms(_psegs[0], p);
            //        foreach (Pair<string, List<Pitch>> rowForm in rowForms)
            //        {
            //            Console.Write(string.Format("{0,-7}", rowForm.Item1 + ":"));
            //            Console.WriteLine("<" + Functions.ConvertPListToString(rowForm.Item2, " ") + ">");
            //        }
            //        if (rowForms.Count == 0)
            //            Console.WriteLine("No row forms match your search term.");
            //    }
            //    else
            //        Console.WriteLine("Invalid pitch string");
            //}
        }

        /// <summary>
        /// Displays all subsegs of a pseg
        /// </summary>
        /// <param name="command">A pseg string</param>
        public override void Subsegs(string command = "")
        {
            List<List<Pitch>> subsegs;
            List<List<Pitch>>.Enumerator e;
            if (command == "")
            {
                subsegs = PSeg.Subsegs(_psegs[0]);
                e = subsegs.GetEnumerator();
                e.MoveNext();

                for (int j = 0; j < subsegs.Count; j += 5)
                {
                    for (int k = j; k < j + 5 && k < subsegs.Count; k++)
                    {
                        Console.Write(string.Format("{0,-18}", '<' +
                            PSeg.ToString(e.Current, " ") + '>'));
                        e.MoveNext();
                    }
                    Console.Write("\n");
                }
            }
            else
            {
                List<Pitch> ps = PSeg.Parse(command);
                if (ps.Count > 0)
                {
                    subsegs = PSeg.Subsegs(ps);
                    e = subsegs.GetEnumerator();
                    e.MoveNext();

                    for (int j = 0; j < subsegs.Count; j += 5)
                    {
                        for (int k = j; k < j + 5 && k < subsegs.Count; k++)
                        {
                            Console.Write(string.Format("{0,-18}", '<' +
                                PSeg.ToString(e.Current, " ") + '>'));
                            e.MoveNext();
                        }
                        Console.Write("\n");
                    }
                }
                else
                    Console.WriteLine("Invalid pseg");
            }
        }

        /// <summary>
        /// Transforms a pseg
        /// </summary>
        /// <param name="command">A transformation string</param>
        public override void Transform(string command)
        {
            List<Pitch> transformList = _psegs[0];  // The pseg to transform
            string[] forms;  // The transformations
            int length = 0;       // The length of the longest transformation
            bool valid = true;    // If the user provided a pcseg, flags if it is invalid

            if (command.Contains(','))
                forms = command.Split(',');
            else if (command.Contains(", "))
                forms = command.Split(", ");
            else
                forms = command.Split(' ');

            // If the user provided a pcseg at the end of the transformation string, we need to interpret that
            if (forms[forms.Length - 1][0] >= '0' && forms[forms.Length - 1][0] <= '9')
            {
                string[] newForms = new string[forms.Length - 1];
                transformList = PSeg.Parse(forms[forms.Length - 1]);
                if (transformList.Count > 0)
                {
                    // Eliminate the pcseg from the list of transformations
                    for (int i = 0; i < forms.Length - 1; i++)
                        newForms[i] = forms[i];
                    forms = newForms;
                }
                else
                {
                    Console.WriteLine("Invalid pseg");
                    valid = false;
                }
            }

            // Proceed with transformations if the transform pseg is valid
            if (valid)
            {
                foreach (string form in forms)
                {
                    if (form.Length > length)
                        length = form.Length;
                    string trimForm = form.Trim();
                    if (trimForm.Length > 0)
                    {
                        if (PSeg.IsValidTransformation(trimForm))
                        {
                            List<Pitch> secondaryForm = PSeg.Transform(transformList, trimForm);
                            Console.WriteLine(string.Format("{0,-" + (length + 2).ToString() + "}{1}", trimForm + ":", "<" + PSeg.ToString(secondaryForm, " ") + ">"));
                        }
                        else
                            Console.WriteLine(string.Format("{0,-" + (length + 2).ToString() + "}{1}", trimForm + ": ", "Invalid transformation"));
                    }
                }
            }
        }
    }
}
