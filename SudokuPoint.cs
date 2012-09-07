using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace SudokuSolver
{
    /// <summary>
    /// Represents a point in a Sudoku grid, including the point's index, row, column, and box numbers, its values, and its possible values
    /// </summary>
    class SudokuPoint
    {
        /// <summary>
        /// The point's index in the grid, from 0 to 80
        /// </summary>
        public int index { get; set; }

        /// <summary>
        /// The point's rwo in the grid, from 0 to 8
        /// </summary>
        public int row { get; set; }

        /// <summary>
        /// The point's column in the grid, from 0 to 8
        /// </summary>
        public int column { get; set; }

        /// <summary>
        /// The point's 3-by-3 box in the grid, from 0 to 8
        /// </summary>
        public int box { get; set; }

        /// <summary>
        /// The point's current value. This is 0 if the point's value hasn't yet been determined
        /// </summary>
        public int value { get; set; }

        /// <summary>
        /// The point's possible values; i.e., the values not already present in the point's row, column, or box
        /// </summary>
        public List<int> possibleValues { get; set; }

        /// <summary>
        /// Creates a Sudoku point with a given index and given value
        /// </summary>
        /// <param name="index">The index where the point will be placed in the grid.</param>
        /// <param name="value">The value to give to the point. This will be 0 if the point does not yet have a value set</param>
        public SudokuPoint(int index, int value)
        {
            //Set the point's value
            this.value = value;

            //Set the point's possible values
            possibleValues = new List<int>();
            if (value == 0)
                for (int i = 1; i < 10; i++)
                    possibleValues.Add(i);

            //Set the point's index, row, and column
            this.index = index;
            this.row = index / 9;
            this.column = index % 9;

            //Calculate and set the point's box
            this.box = (this.row / 3 * 3) + (this.column / 3);
        }
    }
}
