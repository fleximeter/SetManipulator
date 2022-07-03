/* File: MusicTool.cs
** Project: SetManipulator
** Author: Jeffrey Martin
**
** This file contains the implementation of the MusicTool class. MusicTool is an abstraction of the functionality
** of SetManipulator. It makes it easy to use the same commands to work with different collection types.
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
    class MusicTool
    {
        public virtual bool PackFromRight
        {
            get { return true; }
            set { ; }
        }
        public virtual PrimarySetName DefaultSetName
        {
            get { return 0; }
            set { ; }
        }
        public virtual void Angle(string command1 = "", string command2 = "") => Console.WriteLine("This command is not available in the current mode.");
        public virtual void bip(string command = "") => Console.WriteLine("This command is not available in the current mode.");
        public virtual void Calculate() => Console.WriteLine("This command is not available in the current mode.");
        public virtual void Complement(string command = "") => Console.WriteLine("This command is not available in the current mode.");
        public virtual void ComplementPrime() => Console.WriteLine("This command is not available in the current mode.");
        public virtual void ComplexK(string command = "") => Console.WriteLine("This command is not available in the current mode.");
        public virtual void ComplexKh(string command = "") => Console.WriteLine("This command is not available in the current mode.");
        public virtual void DerivedCore() => Console.WriteLine("This command is not available in the current mode.");
        public virtual void ICV(string command = "") => Console.WriteLine("This command is not available in the current mode.");
        public virtual void imb(string command = "") => Console.WriteLine("This command is not available in the current mode.");
        public virtual void Info(string command = "") => Console.WriteLine("This command is not available in the current mode.");
        public virtual void Intersect(string command1 = "", string command2 = "") => Console.WriteLine("This command is not available in the current mode.");
        public virtual void IntersectMax(string command1 = "", string command2 = "") => Console.WriteLine("This command is not available in the current mode.");
        public virtual void Intervals(string command = "") => Console.WriteLine("This command is not available in the current mode.");
        public virtual void IsValidRowGen(string command = "") => Console.WriteLine("This command is not available in the current mode.");
        public virtual void Load(string command = "") => Console.WriteLine("This command is not available in the current mode.");
        public virtual void LoadPrime(string command = "") => Console.WriteLine("This command is not available in the current mode.");
        public virtual void LoadRandom() => Console.WriteLine("This command is not available in the current mode.");
        public virtual void LoadRandomAIR() => Console.WriteLine("This command is not available in the current mode.");
        public virtual void Matrix(string command1 = "", string command2 = "") => Console.WriteLine("This command is not available in the current mode.");
        public virtual void OrderedSearch(string command = "") => Console.WriteLine("This command is not available in the current mode.");
        public virtual void Search(string command = "") => Console.WriteLine("This command is not available in the current mode.");
        public virtual void Subsets(string command = "") => Console.WriteLine("This command is not available in the current mode.");
        public virtual void Subsegs(string command = "") => Console.WriteLine("This command is not available in the current mode.");
        public virtual void SubsetsPrime(string command = "") => Console.WriteLine("This command is not available in the current mode.");
        public virtual void Transform(string command) => Console.WriteLine("This command is not available in the current mode.");
        public virtual void Union(string command1 = "", string command2 = "") => Console.WriteLine("This command is not available in the current mode.");
        public virtual void UnionCompact(string command1 = "", string command2 = "") => Console.WriteLine("This command is not available in the current mode.");
        public virtual void ZRelation(string command = "") => Console.WriteLine("This command is not available in the current mode.");
    }
}
