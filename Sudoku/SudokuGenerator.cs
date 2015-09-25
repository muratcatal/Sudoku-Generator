namespace Sudoku
{
    public class SudokuGenerator
    {
        /// <summary>
        /// Generates sudoku grid
        /// </summary>
        /// <param name="n">Number of sectors in row and column. for n = 9 means a 3x3 sudoku grid</param>
        /// <returns></returns>
        public static SudokuGrid Generate(int n,SudokuGrid.LevelDifficulty levelDifficulty)
        {
            return new SudokuGrid().Generate(n, levelDifficulty);
        }
    }
}