/* File: MainProgram.cs
** Project: SetManipulator
** Author: Jeffrey Martin
**
** This file contains the implementation of the MainProgram class. MainProgram manages the main prompt
** and command interpretation.
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
using MusicTheory;

namespace SetManipulator
{
    class MainProgram
    {
        MusicTool[] mt;
        int mode;

        /// <summary>
        /// Initializer
        /// </summary>
        public MainProgram()
        {
            mt = new MusicTool[7] { new PcSetTool(), new PcSegTool(), new PSetTool(), new PSegTool(), new CSegTool(), 
                new PcSet24Tool(), new PcSeg24Tool() };
            mode = 0;
        }

        /// <summary>
        /// The main prompt for the program
        /// </summary>
        public void Menu()
        {
            string input;
            bool quit = false;

            Console.WriteLine("#################### Set Manipulator #####################");
            Console.WriteLine("Copyright (c) 2022 by Jeffrey Martin. All rights reserved.");
            Console.WriteLine("https://jeffreymartincomposer.com\n");
            Console.WriteLine("This program is licensed under the GNU GPL v3 and comes");
            Console.WriteLine("with ABSOLUTELY NO WARRANTY. For details, type \"about.\"\n");
            do
            {
                Console.Write("> ");
                input = Console.ReadLine();
                if (input.ToUpper() == "QUIT" || input.ToUpper() == "Q")
                    quit = true;
                else
                    ParseCommand(input);
            } while (!quit);
        }

        /// <summary>
        /// Parses the command and calls appropriate functions
        /// </summary>
        /// <param name="command"></param>
        void ParseCommand(string command)
        {
            string[] cmd = command.Split(' ');
            if (cmd.Length == 1)
            {
                cmd[0] = cmd[0].Trim();
                string cmd1 = cmd[0].ToUpper();
                if (cmd1 == "?" || cmd1 == "HELP" || cmd1 == "/?" || cmd1 == "-?")
                    Help();
                else if (cmd1 == "ABOUT")
                    About();
                else if (cmd1 == "ANGLE")
                    Angle();
                else if (cmd1 == "BIP")
                    bip();
                else if (cmd1 == "CALCULATE" || cmd1 == "CC")
                    Calculate();
                else if (cmd1 == "COMPLEMENT" || cmd1 == "C")
                    Complement();
                else if (cmd1 == "CP")
                    ComplementPrime();
                else if (cmd1 == "DC")
                    DerivedCore();
                else if (cmd1 == "ICV")
                    ICV();
                else if (cmd1 == "IMB")
                    imb();
                else if (cmd1 == "INFO" || cmd1 == "N")
                    Info();
                else if (cmd1 == "INTERSECT" || cmd1 == "IN")
                    Intersect();
                else if (cmd1 == "INTERVALS" || cmd1 == "INT")
                    Intervals();
                else if (cmd1 == "IX")
                    IntersectMax();
                else if (cmd1 == "K")
                    ComplexK();
                else if (cmd1 == "KH")
                    ComplexKh();
                else if (cmd1 == "LOAD" || cmd1 == "L")
                    Load();
                else if (cmd1 == "LP")
                    LoadPrime();
                else if (cmd1 == "LR")
                    LoadRandom();
                else if (cmd1 == "LAIR")
                    LoadRandomAIR();
                else if (cmd1 == "MATRIX" || cmd1 == "MX")
                    Matrix();
                else if (cmd1 == "OH")
                    OrderedSearch();
                else if (cmd1 == "SEARCH" || cmd1 == "H")
                    Search();
                else if (cmd1 == "SP")
                    SubsetsPrime();
                else if (cmd1 == "S" && (mode == 0 || mode == 2 || mode == 5))
                    Subsets();
                else if (cmd1 == "SUBSETS")
                    Subsets();
                else if (cmd1 == "SUBSEGS" || cmd1 == "S")
                    Subsegs();
                else if (cmd1 == "UNION" || cmd1 == "U")
                    Union();
                else if (cmd1 == "UC")
                    UnionCompact();
                else if (cmd1 == "VRG")
                    IsValidRowGen();
                else if (cmd1 == "Z-RELATION" || cmd1 == "Z")
                    ZRelation();
                else if (cmd[0][0] == 'T' || cmd[0][0] == 'M' || cmd[0][0] == 'I' || cmd[0][0] == 'R' || cmd[0][0] == 'r')
                    Transform(command.Trim());
                else
                    Console.WriteLine("Invalid command. For a list of commands, enter ?.\n");
            }
            else if (cmd.Length == 2)
            {
                cmd[0] = cmd[0].Trim();
                cmd[1] = cmd[1].Trim();
                string cmd1 = cmd[0].ToUpper();
                string cmd2 = cmd[1].ToUpper();
                if (cmd1 == "ANGLE")
                    Angle(cmd2);
                else if (cmd1 == "BIP")
                    bip(cmd2);
                else if (cmd1 == "COMPLEMENT")
                {
                    if (cmd2 == "PRIME")
                        ComplementPrime();
                    else
                        Complement(cmd2);
                }
                else if (cmd1 == "DERIVED" && cmd2 == "CORE")
                    DerivedCore();
                else if (cmd1 == "ICV")
                    ICV(cmd2);
                else if (cmd1 == "IMB")
                    imb(cmd2);
                else if (cmd1 == "INFO" || cmd1 == "N")
                    Info(cmd2);
                else if (cmd1 == "INTERSECT" && cmd2 == "MAX")
                    IntersectMax();
                else if (cmd1 == "INTERSECT" || cmd1 == "IN")
                    Intersect(cmd2);
                else if (cmd1 == "IX")
                    IntersectMax(cmd2);
                else if (cmd1 == "INTERVAL-CLASS" && cmd2 == "VECTOR")
                    ICV();
                else if (cmd1 == "INTERVALS" || cmd1 == "INT")
                    Intervals(cmd[1]);
                else if (cmd1 == "K")
                    ComplexK(cmd2);
                else if (cmd1 == "KH")
                    ComplexKh(cmd2);
                else if (cmd1 == "L")
                    Load(cmd2);
                else if (cmd1 == "LP")
                    LoadPrime(cmd2);
                else if (cmd1 == "LOAD")
                {
                    if (cmd2 == "PRIME")
                        LoadPrime();
                    else if (cmd2 == "RANDOM")
                        LoadRandom();
                    else
                        Load(cmd2);
                }
                else if (cmd1 == "MATRIX" || cmd1 == "MX")
                    Matrix(cmd[1]);
                else if (cmd1 == "MODE")
                    Mode(cmd[1]);
                else if (cmd1 == "OH")
                    OrderedSearch(cmd2);
                else if (cmd1 == "ORDERED" && cmd2 == "SEARCH")
                    OrderedSearch();
                else if (cmd1 == "SEARCH" || cmd1 == "H")
                    Search(cmd2);
                else if (cmd1 == "S" && (mode == 0 || mode == 2 || mode == 4))
                {
                    if (cmd2 == "PRIME")
                        SubsetsPrime();
                    else
                        Subsets(cmd2);
                }
                else if (cmd1 == "SUBSETS")
                {
                    if (cmd2 == "PRIME")
                        SubsetsPrime();
                    else
                        Subsets(cmd2);
                }
                else if (cmd1 == "SUBSEGS" || cmd1 == "S")
                    Subsegs(cmd2);
                else if (cmd1 == "SP")
                    SubsetsPrime(cmd2);
                else if (cmd1 == "UNION" && cmd2 == "COMPACT")
                    UnionCompact();
                else if (cmd1 == "UNION" || cmd1 == "U")
                    Union(cmd2);
                else if (cmd1 == "UC")
                    UnionCompact(cmd2);
                else if (cmd1 == "VRG")
                    IsValidRowGen(cmd2);
                else if (cmd1 == "Z-RELATION" || cmd1 == "Z")
                    ZRelation(cmd2);
                else if (cmd[0][0] == 'T' || cmd[0][0] == 'M' || cmd[0][0] == 'I' || cmd[0][0] == 'R' || cmd[0][0] == 'r')
                    Transform(command.Trim());
                else
                    Console.WriteLine("Invalid command. For a list of commands, enter ?.\n");
            }
            else if (cmd.Length == 3)
            {
                string cmd1 = cmd[0].ToUpper().Trim();
                string cmd2 = cmd[1].ToUpper().Trim();
                string cmd3 = cmd[2].ToUpper().Trim();
                if (cmd1 == "ANGLE")
                    Angle(cmd[1], cmd[2]);
                else if (cmd1 == "INTERSECT" && cmd2 == "MAX")
                    IntersectMax(cmd3);
                else if (cmd1 == "INTERSECT" || cmd1 == "IN")
                    Intersect(cmd[1], cmd[2]);
                else if (cmd1 == "INTERVAL-CLASS" && cmd1 == "VECTOR")
                    ICV(cmd[2]);
                else if (cmd1 == "IX")
                    IntersectMax(cmd[1], cmd[2]);
                else if (cmd1 == "LOAD")
                {
                    if (cmd2 == "PRIME")
                        LoadPrime(cmd[2]);
                    else if (cmd2 == "RANDOM" && cmd3 == "AIR")
                        LoadRandomAIR();
                    else
                        Console.WriteLine("Invalid command. For a list of commands, enter ?.\n");
                }
                else if (cmd1 == "MATRIX" || cmd1 == "MX")
                    Matrix(cmd[1], cmd[2]);
                else if (cmd1 == "ORDERED" && cmd2 == "SEARCH")
                    OrderedSearch(cmd3);
                else if (cmd1 == "SUBSETS" && cmd2 == "PRIME")
                    SubsetsPrime(cmd[2]);
                else if (cmd1 == "UNION" && cmd2 == "COMPACT")
                    UnionCompact(cmd3);
                else if (cmd1 == "UNION" || cmd1 == "U")
                    Union(cmd2, cmd3);
                else if (cmd1 == "UC")
                    UnionCompact(cmd2, cmd3);
                else if (cmd1 == "VALIDATE" && cmd2 == "ROW" && cmd3 == "GENERATOR")
                    IsValidRowGen();
                else if (cmd[0][0] == 'T' || cmd[0][0] == 'M' || cmd[0][0] == 'I' || cmd[0][0] == 'R' || cmd[0][0] == 'r')
                    Transform(command.Trim());
                else
                    Console.WriteLine("Invalid command. For a list of commands, enter ?.\n");
            }
            else if (cmd.Length == 4)
            {
                string cmd1 = cmd[0].ToUpper().Trim();
                string cmd2 = cmd[1].ToUpper().Trim();
                string cmd3 = cmd[2].ToUpper().Trim();
                string cmd4 = cmd[3].ToUpper().Trim();
                if (cmd1 == "INTERSECT" && cmd2 == "MAX")
                    IntersectMax(cmd[2], cmd[3]);
                else if (cmd1 == "UNION" && cmd2 == "COMPACT")
                    UnionCompact(cmd[2], cmd[3]);
                else if (cmd1 == "VALIDATE" && cmd2 == "ROW" && cmd3 == "GENERATOR")
                    IsValidRowGen(cmd4);
                else if (cmd[0][0] == 'T' || cmd[0][0] == 'M' || cmd[0][0] == 'I' || cmd[0][0] == 'R' || cmd[0][0] == 'r')
                    Transform(command.Trim());
                else
                    Console.WriteLine("Invalid command. For a list of commands, enter ?.\n");
            }
            else if (cmd[0][0] == 'T' || cmd[0][0] == 'M' || cmd[0][0] == 'I' || cmd[0][0] == 'R' || cmd[0][0] == 'r')
                Transform(command.Trim());
            else
                Console.WriteLine("Invalid command. For a list of commands, enter ?.\n");
        }

        /// <summary>
        /// Parses the mode command and assigns an appropriate mode from 0-3.
        /// </summary>
        /// <remarks>Mode 0: unordered PC. Mode 1: ordered PC. Mode 2: unordered P. Mode 3: ordered P.</remarks>
        /// <param name="command"></param>
        void Mode(string command)
        {
            command = command.ToUpper().Trim();
            if (command == "C")
                mode = 4;
            else if (command == "F" || command == "FORTE")
                mt[0].DefaultSetName = PrimarySetName.Forte;
            else if (command == "L")
                mt[0].PackFromRight = false;
            else if (command == "N" || command == "PRIME")
                mt[0].DefaultSetName = PrimarySetName.PrimeForm;
            else if (command == "O")
            {
                if (mode == 0 || mode == 1)
                    mode = 1;
                else
                    mode = 3;
            }
            else if (command == "OM")
                mode = 6;
            else if (command == "OP")
                mode = 3;
            else if (command == "OPC")
                mode = 1;
            else if (command == "P")
            {
                if (mode == 0 || mode == 2)
                    mode = 2;
                else
                    mode = 3;
            }
            else if (command == "PC")
            {
                if (mode == 0 || mode == 2)
                    mode = 0;
                else
                    mode = 1;
            }
            else if (command == "R")
                mt[0].PackFromRight = true;
            else if (command == "U")
            {
                if (mode == 0 || mode == 1)
                    mode = 0;
                else
                    mode = 2;
            }
            else if (command == "UM")
                mode = 5;
            else if (command == "UP")
                mode = 2;
            else if (command == "UPC")
                mode = 0;
            else
                Console.WriteLine("Invalid command. For a list of commands, enter ?.\n");
        }

        void Help()
        {
            Console.WriteLine("Here is a list of commands. Commands are followed by a shortcut in parentheses.\n\n" +
                "Angle\nCalculate (CC)\nComplement (C)\nComplement Prime (CP)\nFind K\nFind Kh\nInfo (N)\n" +
                "Intersect (IN)\nIntersect Max (IX)\nInterval-Class Vector (ICV)\nIntervals (INT)\nLoad (L)\nLoad Prime (LP)\n" +
                "Load Random (LR)\nLoad Random AIR (LAIR)\nMatrix (MX)\nSearch (H)\nSubsets (S)\nSubsets Prime (SP)\nUnion (U)\n" +
                "Union Compact (UC)\nZ-Relation (Z)\n\nModes:\nMode F\nMode N\nMode O\nMode P\nMode PC\nMode U\n\n" +
                "Transformations:\nI, Mn, R, rn, Tn\n\nQuit (Q)\n\nAll commands except transformations are not case-sensitive.\n\n"+
                "Sources:\n\n" +
                "Forte, Allen. \"The Structure of Atonal Music.\" New Haven: Yale University Press, 1973.\n\n" +
                "Morris, Robert D. \"Composition with Pitch Classes: A Theory of Compositional Design.\"\n" +
                "     New Haven: Yale University Press, 1987.\n\n" +
                "Straus, Joseph N. \"Introduction to Post-Tonal Theory.\" Third edition. Upper Saddle River:\n" +
                "     Pearson Education Inc., 2005.\n");
        }

        void Angle(string command1 = "", string command2 = "") => mt[mode].Angle(command1, command2);

        void bip(string command = "") => mt[mode].bip(command);

        void Calculate() => mt[mode].Calculate();

        void Complement(string command = "") => mt[mode].Complement(command);

        void ComplementPrime() => mt[mode].ComplementPrime();

        void ComplexK(string command = "") => mt[mode].ComplexK(command);

        void ComplexKh(string command = "") => mt[mode].ComplexKh(command);

        void DerivedCore() => mt[mode].DerivedCore();

        void ICV(string command = "") => mt[mode].ICV(command);

        void imb(string command = "") => mt[mode].imb(command);

        void Info(string command = "") => mt[mode].Info(command);

        void Intersect(string command1 = "", string command2 = "") => mt[mode].Intersect(command1, command2);

        void IntersectMax(string command1 = "", string command2 = "") => mt[mode].IntersectMax(command1, command2);

        void Intervals(string command = "") => mt[mode].Intervals(command);

        void IsValidRowGen(string command = "") => mt[mode].IsValidRowGen(command);

        void Load(string command = "") => mt[mode].Load(command);

        void LoadPrime(string command = "") => mt[mode].LoadPrime(command);

        void LoadRandom() => mt[mode].LoadRandom();

        void LoadRandomAIR() => mt[mode].LoadRandomAIR();

        void Matrix(string command1 = "", string command2 = "") => mt[mode].Matrix(command1, command2);

        void OrderedSearch(string command = "") => mt[mode].OrderedSearch(command);

        void Search(string command = "") => mt[mode].Search(command);

        void Subsets(string command = "") => mt[mode].Subsets(command);

        void Subsegs(string command = "") => mt[mode].Subsegs(command);

        void SubsetsPrime(string command = "") => mt[mode].SubsetsPrime(command);

        void Transform(string command) => mt[mode].Transform(command);

        void Union(string command1 = "", string command2 = "") => mt[mode].Union(command1, command2);

        void UnionCompact(string command1 = "", string command2 = "") => mt[mode].UnionCompact(command1, command2);

        void ZRelation(string command = "") => mt[mode].ZRelation(command);

        void About()
        {
            Console.Write(
                "SetManipulator copyright (c) 2021 by Jeffrey Martin. All rights reserved.\n" +
                "This program is free software: you can redistribute it and/or modify\n" +
                "it under the terms of the GNU General Public License as published by\n" +
                "the Free Software Foundation, either version 3 of the License, or\n" +
                "(at your option) any later version.\n\n" +
                "This program is distributed in the hope that it will be useful,\n" +
                "but WITHOUT ANY WARRANTY; without even the implied warranty of\n" +
                "MERCHANTABILITY or FITNESS FOR A PARTICULAR PURPOSE.See the\n" +
                "GNU General Public License for more details.\n\n" +
                "You should have received a copy of the GNU General Public License\n" +
                "along with this program. If not, see https://www.gnu.org/licenses/.\n\n"
                );
        }
    }
}
