/* File: Pair.cs
** Project: MusicTheory
** Author: Jeffrey Martin
**
** This file contains the implementation of the Pair class.
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

namespace MusicTheory
{
    /// <summary>
    /// An editable pair
    /// </summary>
    /// <typeparam name="T">Data type 1</typeparam>
    /// <typeparam name="U">Data type 2</typeparam>
    public class Pair<T, U>
    {
        /// <summary>
        /// Creates a new Pair
        /// </summary>
        public Pair() { }
        
        /// <summary>
        /// Creates a new Pair
        /// </summary>
        /// <param name="item1">The first item</param>
        /// <param name="item2">The second item</param>
        public Pair(T item1, U item2)
        {
            _item1 = item1;
            _item2 = item2;
        }

        /// <summary>
        /// The first item
        /// </summary>
        public T Item1
        {
            get { return _item1; }
            set { _item1 = value; }
        }

        /// <summary>
        /// The second item
        /// </summary>
        public U Item2
        {
            get { return _item2; }
            set { _item2 = value; }
        }

        T _item1;
        U _item2;
    }
}
