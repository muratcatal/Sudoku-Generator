using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Sudoku;

namespace Sample
{
    class Program
    {
        static void Main(string[] args)
        {
            var sudokuGrid = SudokuGenerator.Generate(9, SudokuGrid.LevelDifficulty.Easy);

            sudokuGrid.Print();

            Console.Read();
            Console.Read();
        }
    }
}
