/* File: PcSegTool.cs
** Project: SetManipulator
** Author: Jeffrey Martin
**
** This file contains the implementation of the PcSegTool class. PcSegTool contains user-facing
** functionality for working with pcsegs and twelve-tone rows.
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
    class PcSegTool : MusicTool
    {
        Row[] _row;
        List<PitchClass>[] _pcseg;
        List<PitchClass> _intervals;
        RowMatrix _matrix;
        bool _haveRow;

        /// <summary>
        /// Initializes the PcSegTool
        /// </summary>
        public PcSegTool()
        {
            _row = new Row[5] { new Row(), new Row(), new Row(), new Row(), new Row() };
            _pcseg = new List<PitchClass>[2] { new List<PitchClass>(), new List<PitchClass>() };
            _matrix = new RowMatrix();
            _intervals = new List<PitchClass>();
            _haveRow = false;
        }

        /// <summary>
        /// Displays the complement of a pcseg
        /// </summary>
        /// <param name="command">A pcseg</param>
        public override void Complement(string command = "")
        {
            if (!_haveRow)
            {
                if (command == "")
                {
                    Console.WriteLine("Complement of <" + PcSeg.ToString(_pcseg[0], " ") +
                        ">: {" + PcSeg.ToString(_pcseg[1], "") + "}");
                }
                else
                {
                    List<PitchClass> list = PcSeg.Parse(command);
                    List<PitchClass> complement = PcSeg.Complement(list);
                    Console.WriteLine("Complement of <" + PcSeg.ToString(list, " ") +
                        ">: {" + PcSeg.ToString(complement, "") + "}");
                }
            }
        }

        /// <summary>
        /// Displays info about a pcseg or row
        /// </summary>
        /// <param name="command">A pcseg or row</param>
        public override void Info(string command = "")
        {
            List<PitchClass> pcseg = _pcseg[0];
            Row row = _row[0];
            bool haveRow = _haveRow;
            if (command != "")
            {
                pcseg = PcSeg.Parse(command);
                if (Row.IsValidRow(pcseg))
                {
                    row = new Row(pcseg);
                    haveRow = true;
                }
            }
            if (haveRow)
                Console.WriteLine("Row: <" + PcSeg.ToString(row.GetPcSeg(), " ") + ">");
            else
                Console.WriteLine("Pcseg: <" + PcSeg.ToString(pcseg, " ") + ">");
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
                List<PitchClass> intervals;
                if (_haveRow)
                {
                    if (PcSeg.IsValidTransformation(command))
                    {
                        Row secondaryForm = _row[0].Transform(command);
                        intervals = secondaryForm.GetIntervalList();
                        Console.WriteLine("Intervals: <" + PcSeg.ToString(intervals, " ") + ">");
                    }
                    else
                        Console.WriteLine(command + ": Invalid transformation");
                }
                else
                {
                    if (PcSeg.IsValidTransformation(command))
                    {
                        List<PitchClass> secondaryForm = PcSeg.Transform(_pcseg[0], command);
                        intervals = PcSeg.Intervals(secondaryForm);
                        Console.WriteLine("Intervals: <" + PcSeg.ToString(intervals, " ") + ">");
                    }
                    else
                        Console.WriteLine(command + ": Invalid transformation");
                }
            }
            else
            {
                List<PitchClass> list = PcSeg.Parse(command);
                List<PitchClass> intervals = PcSeg.Intervals(list);
                Console.WriteLine("Intervals: <" + PcSeg.ToString(intervals, " ") + ">");
            }
        }

        /// <summary>
        /// Loads a pcseg or row
        /// </summary>
        /// <param name="command">A pcseg or row</param>
        public override void Load(string command = "")
        {
            List<PitchClass> list;
            if (command == "")
            {
                Console.Write("Enter pcseg > ");
                command = Console.ReadLine();
            }
            list = PcSeg.Parse(command);
            if (list.Count > 0)
            {
                if (Row.IsValidRow(list))
                {
                    _row[0].ImportRow(list);
                    _matrix.ImportRow(_row[0]);
                    _intervals = _row[0].GetIntervalList();
                    _haveRow = true;
                }
                else
                {
                    _pcseg[0] = list;
                    _pcseg[1] = PcSeg.Complement(_pcseg[0]);
                    _intervals = PcSeg.Intervals(_pcseg[0]);
                    _haveRow = false;
                }
            }
            else
                Console.WriteLine("Invalid pcseg");
        }

        /// <summary>
        /// Loads a random twelve-tone row
        /// </summary>
        public override void LoadRandom()
        {
            _row[0].LoadRandomRow();
            _matrix.ImportRow(_row[0]);
            _intervals = _row[0].GetIntervalList();
            _haveRow = true;
            Info();
        }

        /// <summary>
        /// Loads a random all-interval twelve-tone row
        /// </summary>
        public override void LoadRandomAIR()
        {
            System.IO.StreamReader sr = new System.IO.StreamReader("eleven_interval.txt");
            System.Random rand = new System.Random();
            PitchClass pc = new PitchClass();
            int index = rand.Next(0, 3856);
            int[] rGen = new int[11];
            int rGenIndex = 0;
            string line = "";
            for (int i = 0; i < index; i++)
                line = sr.ReadLine();
            foreach (char c in line)
            {
                if (PitchClass.IsValidPitchClassChar(c))
                {
                    pc.PitchClassChar = c;
                    rGen[rGenIndex] = pc.PitchClassInteger;
                    rGenIndex++;
                }
            }
            _row[0].LoadRowFromRGen(rGen);
            _matrix.ImportRow(_row[0]);
            _intervals = _row[0].GetIntervalList();
            _haveRow = true;
            Info();
        }

        /// <summary>
        /// Displays a twelve-tone matrix
        /// </summary>
        public override void Matrix(string command1 = "", string command2 = "")
        {
            if (_haveRow)
            {
                Console.WriteLine(string.Format("\n     {0,-6}{1,-6}{2,-6}{3,-6}{4,-6}{5,-6}{6,-6}{7,-6}{8,-6}{9,-6}{10,-6}{11,-6}",
                    _matrix.GetTopColumnNameString(0), _matrix.GetTopColumnNameString(1), _matrix.GetTopColumnNameString(2),
                    _matrix.GetTopColumnNameString(3), _matrix.GetTopColumnNameString(4), _matrix.GetTopColumnNameString(5),
                    _matrix.GetTopColumnNameString(6), _matrix.GetTopColumnNameString(7), _matrix.GetTopColumnNameString(8),
                    _matrix.GetTopColumnNameString(9), _matrix.GetTopColumnNameString(10), _matrix.GetTopColumnNameString(11)));
                Console.WriteLine();

                for (int i = 0; i < 12; i++)
                {
                    Console.WriteLine(string.Format("{0,-6}{1,-6}{2,-6}{3,-6}{4,-6}{5,-6}{6,-6}{7,-6}{8,-6}{9,-6}{10,-6}{11,-6}{12,-5}{13,-4}",
                        _matrix.GetLeftRowNameString(i), _matrix[i, 0].PitchClassChar, _matrix[i, 1].PitchClassChar, _matrix[i, 2].PitchClassChar,
                        _matrix[i, 3].PitchClassChar, _matrix[i, 4].PitchClassChar, _matrix[i, 5].PitchClassChar, _matrix[i, 6].PitchClassChar,
                        _matrix[i, 7].PitchClassChar, _matrix[i, 8].PitchClassChar, _matrix[i, 9].PitchClassChar, _matrix[i, 10].PitchClassChar,
                        _matrix[i, 11].PitchClassChar, _matrix.GetRightRowNameString(i)));
                    Console.WriteLine();
                }

                Console.WriteLine(string.Format("     {0,-6}{1,-6}{2,-6}{3,-6}{4,-6}{5,-6}{6,-6}{7,-6}{8,-6}{9,-6}{10,-6}{11,-6}\n",
                    _matrix.GetBottomColumnNameString(0), _matrix.GetBottomColumnNameString(1), _matrix.GetBottomColumnNameString(2),
                    _matrix.GetBottomColumnNameString(3), _matrix.GetBottomColumnNameString(4), _matrix.GetBottomColumnNameString(5),
                    _matrix.GetBottomColumnNameString(6), _matrix.GetBottomColumnNameString(7), _matrix.GetBottomColumnNameString(8),
                    _matrix.GetBottomColumnNameString(9), _matrix.GetBottomColumnNameString(10), _matrix.GetBottomColumnNameString(11)));
            }
            else
                Console.WriteLine("You must enter a row before displaying the matrix.");
        }

        /// <summary>
        /// Searches transformations of the current pcseg or row
        /// </summary>
        /// <param name="command"></param>
        public override void Search(string command = "")
        {
            List<PitchClass> pc;
            List<Pair<string, List<PitchClass>>> rowForms;
            if (command == "")
            {
                Console.Write("Enter pcs (q to quit)\n> ");
                command = Console.ReadLine();
                while (command != "q" && command != "Q")
                {
                    pc = PcSeg.Parse(command);
                    if (pc.Count > 0)
                    {
                        if (_haveRow)
                            rowForms = PcSeg.GetSecondaryForms(_row[0].GetPcSeg(), pc);
                        else
                            rowForms = PcSeg.GetSecondaryForms(_pcseg[0], pc);
                        foreach (Pair<string, List<PitchClass>> rowForm in rowForms)
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
                pc = PcSeg.Parse(command);
                if (pc.Count > 0)
                {
                    if (_haveRow)
                        rowForms = PcSeg.GetSecondaryForms(_row[0].GetPcSeg(), pc);
                    else
                        rowForms = PcSeg.GetSecondaryForms(_pcseg[0], pc);
                    foreach (Pair<string, List<PitchClass>> rowForm in rowForms)
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
            List<List<PitchClass>> subsegs;
            List<List<PitchClass>>.Enumerator e;
            if (command == "")
            {
                if (_haveRow)
                    subsegs = PcSeg.Subsegs(_row[0].GetPcSeg());
                else
                    subsegs = PcSeg.Subsegs(_pcseg[0]);
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
                List<PitchClass> pcs = PcSeg.Parse(command);
                if (pcs.Count > 0)
                {
                    subsegs = PcSeg.Subsegs(pcs);
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
            List<PitchClass> transformList = _pcseg[0];  // The pcseg to transform
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
                transformList = PcSeg.Parse(forms[forms.Length - 1]);
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
                // We don't do this if the user provided a custom pcseg
                if (_haveRow && !custom)
                {
                    foreach (string form in forms)
                    {
                        string trimForm = form.Trim();
                        if (trimForm.Length > 0)
                        {
                            if (PcSeg.IsValidTransformation(trimForm))
                            {
                                Row secondaryForm = _row[0].Transform(trimForm);
                                Console.WriteLine(string.Format("{0,-" + (length + 2).ToString() + "}{1}", trimForm + ":", "<" + PcSeg.ToString(secondaryForm.GetPcSeg(), " ") + ">"));
                            }
                            else
                                Console.WriteLine(string.Format("{0,-" + (length + 2).ToString() + "}{1}", trimForm + ": ", "Invalid transformation"));
                        }
                    }
                }
                else
                {
                    foreach (string form in forms)
                    {
                        string trimForm = form.Trim();
                        if (trimForm.Length > 0)
                        {
                            if (PcSeg.IsValidTransformation(trimForm))
                            {
                                List<PitchClass> secondaryForm = PcSeg.Transform(transformList, trimForm);
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
}
