using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace SudokuSolver
{
    /// <summary>
    /// Represents a Sudoku Grid, with methods to create, solve, and check the grid.
    /// </summary>
    class SudokuGrid
    {
        /// <summary>
        /// The list of acceptable digits in a SudokuGrid
        /// </summary>
        private static int[] digits = new int[] { 1, 2, 3, 4, 5, 6, 7, 8, 9 };

        /// <summary>
        /// An array of SudokuPoint objects, each representing a point in the grid
        /// </summary>
        private SudokuPoint[] points { get; set; }

        /// <summary>
        /// Creates a new Sudoku Grid with using an array of point values
        /// </summary>
        /// <param name="pointValues">An array of integers representing the starting values in the grid. Blank spaces in the grid are represented by the charachter '0'</param>
        private SudokuGrid(int[] pointValues)
        {
            SetPoints(pointValues);
        }

        /// <summary>
        /// Creates a Sudoku Grid using a string of 80 characters containing the starting values in the grid
        /// </summary>
        /// <param name="values">A string of 80 characters containing the starting values in the grid. Blank spaces in the grid are represented by the character '0'.</param>
        private SudokuGrid(string values)
        {
            SetPoints(values);
        }

        /// <summary>
        /// Sets the point values in the grid to the values in a given string
        /// </summary>
        /// <param name="values">An 81-character string showing the current digit at each place in the grid. Blank spaces are represented by the character '0'.</param>
        private void SetPoints(string values)
        {
            int[] pointValues = new int[values.Length];

            for (int i = 0; i < values.Length; i++)
                pointValues[i] = Convert.ToInt32(values.Substring(i, 1));

            SetPoints(pointValues);
        }

        /// <summary>
        /// Sets all points in the grid to values given
        /// </summary>
        /// <param name="pointValues">An array of integers containing the values to go in the grid, numbered from 0 (top left) to 80 (bottom right)</param>
        private void SetPoints(int[] pointValues)
        {
            if (pointValues.Length != 81)
                return;

            points = new SudokuPoint[81];

            for (int i = 0; i < pointValues.Length; i++)
                points[i] = new SudokuPoint(i, pointValues[i]);
        }

        /// <summary>
        /// Prints the grid in a readable way to the command line
        /// </summary>
        private void Print()
        {
            Console.WriteLine();

            for (int i = 0; i < 81; i++)
            {
                if (i % 27 == 0 && i > 0)
                    Console.Write("-----------------------\n");

                Console.Write(points[i].value + " ");

                if (i % 3 == 2)
                    Console.Write("| ");

                if (i % 9 == 8)
                    Console.Write("\n");
            }
        }

        /// <summary>
        /// Returns the three-digit number formed by the three digits in the top left of the grid, read from left to right
        /// </summary>
        /// <returns>The three-digit number used to answer Project Euler problem 96</returns>
        private int EulerAnswer()
        {
            return points[0].value * 100 + points[1].value * 10 + points[2].value;
        }

        /// <summary>
        /// Returns a string representing the current grid
        /// </summary>
        /// <returns>An 81-character string showing the current digit at each place in the grid. Blank spaces are represented by the character '0'.</returns>
        private string GridString()
        {
            string gridString = "";

            for (int i = 0; i < 81; i++)
                gridString += points[i].value;

            return gridString;
        }

        /// <summary>
        /// Returns whether the grid has been successfully solved
        /// </summary>
        /// <returns>True if the grid is (correctly) solved; false otherwise</returns>
        private bool IsSolved()
        {
            //return IsValid() && IsComplete();

            
            if (points.Count(p => p.value == 0) > 0)
                return false;

            for (int i = 0; i < 9; i++)
            {
                if (points.Where(p => p.row == i).GroupBy(p => p.value).Count() < 9)
                    return false;
                if (points.Where(p => p.column == i).GroupBy(p => p.value).Count() < 9)
                    return false;
                if (points.Where(p => p.box == i).GroupBy(p => p.value).Count() < 9)
                    return false;
            }

            return true;
             
        }

        /// <summary>
        /// Returns whether all points in the grid have been solved (but not necessarily correctly)
        /// </summary>
        /// <returns>True if all points in the grid have a value other than 0; false otherwise.</returns>
        private bool IsComplete()
        {
            return (points.Count(p => p.value == 0) == 0);
        }

        /// <summary>
        /// Returns whether the grid is still valid, i.e., whether none of the Sudoku rules have been broken
        /// </summary>
        /// <returns>False if any row, column or box has a digit repeated; true otherwise.</returns>
        private bool IsValid()
        {
            //If any row, column, or box has a digit repeated, the grid is not valid
            for (int i = 0; i < 9; i++)
            {
                if (points.Where(p => p.row == i && p.value != 0).GroupBy(p => p.value).Where(g => g.Count() > 1).FirstOrDefault() != null)
                    return false;
                if (points.Where(p => p.column == i && p.value != 0).GroupBy(p => p.value).Where(g => g.Count() > 1).FirstOrDefault() != null)
                    return false;
                if (points.Where(p => p.box == i && p.value != 0).GroupBy(p => p.value).Where(g => g.Count() > 1).FirstOrDefault() != null)
                    return false;
            }

            //If the grid passed the previous tests, it is valid
            return true;
        }

        /// <summary>
        /// Recalculates the possible values for a given point, checking all used values in that point's row, column, and box
        /// </summary>
        /// <param name="point">The point to recalculate possible values for</param>
        private void RecalculatePossibleValues(SudokuPoint point)
        {
            if (point.value != 0)
            {
                point.possibleValues = new List<int>();
                return;
            }

            var impossibleValues = points.Where(p => p.box == point.box || p.column == point.column || p.row == point.row).Select(p => p.value).Distinct();
            point.possibleValues = digits.Where(d => !impossibleValues.Contains(d)).ToList();
        }

        /// <summary>
        /// Recalculates possible values for all points in the grid.
        /// </summary>
        private void RecalculatePossibleValues()
        {
            foreach (SudokuPoint point in points)
                RecalculatePossibleValues(point);
        }

        /// <summary>
        /// Tries to brute-force solve one point in the grid, just checking each point for whether it has exactly 1 possible value remaining.
        /// </summary>
        /// <returns>True if a point could be solved; false otherwise</returns>
        private bool SolveNextPoint()
        {
            //First, get the possible values for all points
            RecalculatePossibleValues();
            
            //Try to find a point that only has one possible value
            foreach (SudokuPoint point in points)
            {
                if (point.value == 0)
                {
                    if (point.possibleValues.Count == 1)
                    {
                        point.value = point.possibleValues.FirstOrDefault();
                        return true;
                    }
                }
            }

            //No point could be solved based on this test
            return false;
        }

        /// <summary>
        /// Attempts to solve a row by finding a digit not already in the row that has exactly one point with that digit in its list of possible values
        /// </summary>
        /// <returns>True if a point was solved. False otherwise.</returns>
        private bool SolveNextEntity()
        {
            //Check each row for a point to solve
            for (int i = 0; i < 9; i++)
            {
                //Get the digits needed for the row
                var usedValues = points.Where(p => p.row == i).Select(p => p.value).Distinct();
                var possibleValues = digits.Where(d => !usedValues.Contains(d));

                //Check each digit needed to see if exactly one point has that digit as a possible value
                foreach (var val in possibleValues)
                {
                    var possiblePoints = points.Where(p => p.row == i && p.possibleValues.Contains(val));
                    if (possiblePoints.Count() == 1)
                    {
                        //Set the point to the value found and exit out of the method
                        possiblePoints.FirstOrDefault().value = val;
                        return true;
                    }
                }
            }

            //Check each column for a point to solve
            for (int i = 0; i < 9; i++)
            {
                //Get the digits needed for the column
                var usedValues = points.Where(p => p.column == i).Select(p => p.value).Distinct();
                var possibleValues = digits.Where(d => !usedValues.Contains(d));

                //Check each digit needed to see if exactly one point has that digit as a possible value
                foreach (var val in possibleValues)
                {
                    var possiblePoints = points.Where(p => p.column == i && p.possibleValues.Contains(val));
                    if (possiblePoints.Count() == 1)
                    {
                        //Set the point to the value found and exit out of the method
                        possiblePoints.FirstOrDefault().value = val;
                        return true;
                    }
                }
            }

            //Check each box for a point to solve
            for (int i = 0; i < 9; i++)
            {
                //Get the digits needed for the box
                var usedValues = points.Where(p => p.box == i).Select(p => p.value).Distinct();
                var possibleValues = digits.Where(d => !usedValues.Contains(d));

                //Check each digit needed to see if exactly one point has that digit as a possible value
                foreach (var val in possibleValues)
                {
                    //Set the point to the value found and exit out of the method
                    var possiblePoints = points.Where(p => p.box == i && p.possibleValues.Contains(val));
                    if (possiblePoints.Count() == 1)
                    {
                        possiblePoints.FirstOrDefault().value = val;
                        return true;
                    }
                }
            }

            return false;
        }

        /// <summary>
        /// This method takes a point that has two possible values, picks the first value, and tries to solve the grid using that value. If that value is wrong, the method goes back and tries the other value.
        /// </summary>
        /// <returns>True if the grid was solved by this method; false otherwise.</returns>
        private bool Guess()
        {
            //If there are no points with at least 1 possible value left, a successful guess cannot be made
            if (points.Where(p => p.possibleValues.Count > 0).FirstOrDefault() == null)
                return false;

            //Make a "copy" of the grid before guessing, to reset if needed later
            string gridString = GridString();

            //Select the first point that has the minimal number of possible values left
            int minPossible = points.Where(p => p.possibleValues.Count > 0).Select(p => p.possibleValues.Count).Min();
            SudokuPoint toGuess = points.Where(p => p.possibleValues.Count == minPossible).FirstOrDefault();

            //If there are no points with possible values left, that means the grid is no longer valid, and the last guess was unsuccessful
            if (toGuess == null)
                return false;

            //Save the index of the point guessed, the value guessed, and the value not guessed
            int indexGuessed = toGuess.index;

            //Create a list of possible guesses (the same list as the possible values for the point being guessed)
            List<int> possibleGuesses = new List<int>();
            foreach (int i in toGuess.possibleValues)
                possibleGuesses.Add(i);

            //Continue to guess until all options for the point being guessed have been exhausted
            while (possibleGuesses.Count > 0)
            {
                points[indexGuessed].value = possibleGuesses.FirstOrDefault();
                if (SolveAllPoints())
                {
                    //If the grid could be solved from this guess, the guess was successful
                    return true;
                }
                else
                {
                    //If the grid could not be solved from this guess, reset the grid to before the guess,
                    //remove the last value guessed, and try again
                    SetPoints(gridString);
                    possibleGuesses.Remove(possibleGuesses.FirstOrDefault());
                }
            }

            //If the grid could not be solved from either guess, the guess is unsuccessful
            return false;
        }

        /// <summary>
        /// Keeps trying to solve all the points in the grid, until all points are solved or no more points can be solved
        /// </summary>
        /// <returns>True if the grid is (correctly) solved; false otherwise</returns>
        private bool SolveAllPoints()
        {
            while (!IsComplete()) // && IsValid()
            {
                //First try to brute force solve the next point.
                //If no points can be solved, try to solve the next column, row, or box
                //If no columns, rows, or boxes can be solved, try to guess a value
                //If no points are successfully guessed, this solve attempt is unsuccessful
                if (!SolveNextPoint())
                    if (!SolveNextEntity())
                        if (!Guess())
                            break;
            }

            return IsSolved();
        }

        /// <summary>
        /// Solves the grid and then prints the solution.
        /// If no solution can be found, a message indicating such is shown and then the last state of the grid is printed.
        /// </summary>
        private void SolveAndPrint()
        {
            if (!SolveAllPoints())
                Console.WriteLine("Couldn't solve this grid.");

            Print();
            Console.WriteLine("\n\n\n");
        }

        /// <summary>
        /// Creates a grid based on an input string, then solves and prints the grid.
        /// </summary>
        /// <param name="start">The 81-character string representing the starting grid. Blank spaces are represented by the character '0'.</param>
        public static void SolveAndPrint(string start)
        {
            try
            {
                SudokuGrid grid = new SudokuGrid(start);
                grid.SolveAndPrint();
            }
            catch
            {
                Console.WriteLine("Invalid grid: {0}", start);
            }
        }
    }
}
