using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace SudokuSolver
{
    class Program
    {
        /// <summary>
        /// Solves the grids given in input.txt, in the same folder as the executable.
        /// </summary>
        /// <param name="args">Parameters. Not used.</param>
        static void Main(string[] args)
        {
            DateTime start = DateTime.Now;
            
            string[] grids = System.IO.File.ReadAllLines("input.txt");

            foreach (string grid in grids)
                SudokuGrid.SolveAndPrint(grid);

            Console.WriteLine("Execution took {0} seconds.", DateTime.Now.Subtract(start).TotalSeconds);
            Console.ReadLine();
        }
    }
}
