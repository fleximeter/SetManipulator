/* File: CSegTool.cs
** Project: SetManipulator
** Author: Jeffrey Martin
**
** This file contains the implementation of the CSegTool class. CSegTool contains user-facing
** functionality for working with csegs.
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
    class CSegTool : MusicTool
    {
        List<uint>[] csegs;

        /// <summary>
        /// Initializes the CSegTool
        /// </summary>
        public CSegTool()
        {
            csegs = new List<uint>[4] { new List<uint>(), new List<uint>(), new List<uint>(), new List<uint>() };
        }

        /// <summary>
        /// Displays information about a cseg
        /// </summary>
        /// <param name="command">A cseg</param>
        public override void Info(string command = "")
        {
            List<uint> cseg = csegs[0];
            if (command != "")
                cseg = CSeg.Parse(command);
            Console.WriteLine("Cseg: <" + CSeg.ToString(cseg, " ") + ">");
        }

        /// <summary>
        /// Loads a cseg
        /// </summary>
        /// <param name="command">A cseg</param>
        public override void Load(string command = "")
        {
            bool valid = true;
            if (command != "")
            {
                // If the user provided a pstring
                if (command[0] == 'p' || command[0] == 'P')
                {
                    command = command.TrimStart(new char[2] { 'p', 'P' });
                    if (PSeg.IsValidPString(command))
                        csegs[0] = CSeg.ConvertFromPseg(PSeg.Parse(command));
                    else
                        valid = false;
                }
                // If the user provided a cstring
                else if (CSeg.IsValidCString(command))
                    csegs[0] = CSeg.Parse(command);
                else
                    valid = false;
            }
            if (!valid)
                Console.WriteLine("Invalid cseg");
        }

        /// <summary>
        /// Loads the prime form of a cseg
        /// </summary>
        /// <param name="command"></param>
        public override void LoadPrime(string command = "")
        {
            bool valid = true;
            if (command != "")
            {
                // If the user provided a pstring
                if (command[0] == 'p' || command[0] == 'P')
                {
                    command = command.TrimStart(new char[2] { 'p', 'P' });
                    if (PSeg.IsValidPString(command))
                        csegs[0] = CSeg.ConvertFromPseg(PSeg.Parse(command));
                    else
                        valid = false;
                }
                // If the user provided a cstring
                else if (CSeg.IsValidCString(command))
                    csegs[0] = CSeg.Parse(command);
                else
                    valid = false;
            }
            if (valid)
                csegs[0] = CSeg.PrimeForm(csegs[0]);
            else
                Console.WriteLine("Invalid cseg");
        }

        /// <summary>
        /// Displays the COM matrix for a cseg
        /// </summary>
        public override void Matrix(string command1 = "", string command2 = "")
        {
            List<uint> cs1 = csegs[0];
            List<uint> cs2 = csegs[0];
            if (command1 != "" && command2 != "")
            {
                cs1 = CSeg.Parse(command1);
                cs2 = CSeg.Parse(command2);
            }
            else if (command1 != "")
                cs2 = CSeg.Parse(command1);
            Console.Write("\n   ");
            foreach (uint cp in cs1)
                Console.Write(string.Format("{0,-3}", cp.ToString()));
            Console.WriteLine();

            for (int i = 0; i < cs2.Count; i++)
            {
                Console.Write(string.Format("{0,-3}", cs2[i].ToString()));
                for (int j = 0; j < cs1.Count; j++)
                {
                    if (CSeg.Com(cs1[j], cs2[i]) == 1)
                        Console.Write("+  ");
                    else if (CSeg.Com(cs1[j], cs2[i]) == -1)
                        Console.Write("-  ");
                    else
                        Console.Write("0  ");
                }
                Console.WriteLine();
            }
            Console.WriteLine();
        }

        /// <summary>
        /// Displays all subsegs of a cseg
        /// </summary>
        /// <param name="command">A cseg string</param>
        public override void Subsegs(string command = "")
        {
            List<List<uint>> subsegs;
            List<List<uint>>.Enumerator e;
            if (command == "")
            {
                subsegs = CSeg.Subsegs(csegs[0]);
                e = subsegs.GetEnumerator();
                e.MoveNext();

                for (int j = 0; j < subsegs.Count; j += 5)
                {
                    for (int k = j; k < j + 5 && k < subsegs.Count; k++)
                    {
                        Console.Write(string.Format("{0,-18}", '<' +
                            CSeg.ToString(e.Current, " ") + '>'));
                        e.MoveNext();
                    }
                    Console.Write("\n");
                }
            }
            else
            {
                List<uint> cps = CSeg.Parse(command);
                if (cps.Count > 0)
                {
                    subsegs = CSeg.Subsegs(cps);
                    e = subsegs.GetEnumerator();
                    e.MoveNext();

                    for (int j = 0; j < subsegs.Count; j += 5)
                    {
                        for (int k = j; k < j + 5 && k < subsegs.Count; k++)
                        {
                            Console.Write(string.Format("{0,-18}", '<' +
                                CSeg.ToString(e.Current, " ") + '>'));
                            e.MoveNext();
                        }
                        Console.Write("\n");
                    }
                }
                else
                    Console.WriteLine("Invalid cseg");
            }
        }

        /// <summary>
        /// Transforms a pseg
        /// </summary>
        /// <param name="command">A transformation string</param>
        public override void Transform(string command)
        {
            List<uint> transformList = csegs[0];  // The pseg to transform
            string[] forms;  // The transformations
            int length = 0;       // The length of the longest transformation
            bool valid = true;    // If the user provided a pcseg, flags if it is invalid

            if (command.Contains(','))
                forms = command.Split(',');
            else if (command.Contains(", "))
                forms = command.Split(", ");
            else
                forms = command.Split(' ');

            // If the user provided a cseg at the end of the transformation string, we need to interpret that
            if (forms[forms.Length - 1][0] >= '0' && forms[forms.Length - 1][0] <= '9')
            {
                string[] newForms = new string[forms.Length - 1];
                transformList = CSeg.Parse(forms[forms.Length - 1]);
                if (transformList.Count > 0)
                {
                    // Eliminate the pcseg from the list of transformations
                    for (int i = 0; i < forms.Length - 1; i++)
                        newForms[i] = forms[i];
                    forms = newForms;
                }
                else
                {
                    Console.WriteLine("Invalid cseg");
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
                        if (CSeg.IsValidTransformation(trimForm))
                        {
                            List<uint> secondaryForm = CSeg.Transform(transformList, trimForm);
                            Console.WriteLine(string.Format("{0,-" + (length + 2).ToString() + "}{1}", trimForm + ":", "<" + CSeg.ToString(secondaryForm, " ") + ">"));
                        }
                        else
                            Console.WriteLine(string.Format("{0,-" + (length + 2).ToString() + "}{1}", trimForm + ": ", "Invalid transformation"));
                    }
                }
            }
        }
    }
}
