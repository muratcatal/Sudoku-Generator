using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text.RegularExpressions;

namespace Sudoku
{
    public class SudokuGrid
    {
        private int _gridSize;
        private int _rowSize;
        private int _sectorRowSize;
        private List<SudokuCell> _sudokuCellList = new List<SudokuCell>();

        public long ExecutionTime { get; set; }

        /// <summary>
        /// Generates sudoku grid
        /// </summary>
        /// <param name="n">Number of sectors in row and column. for n = 9 means a 3x3 sudoku grid</param>
        /// <returns></returns>
        public SudokuGrid Generate(int n, LevelDifficulty levelDifficulty)
        {
            _rowSize = n;
            _sectorRowSize = Convert.ToInt32(Math.Sqrt(_rowSize));
            _gridSize = _rowSize * _rowSize;

            InitSudokuCells();

            Stopwatch time = Stopwatch.StartNew();

            foreach (SudokuCell sudokuCell in _sudokuCellList)
                GenerateRandomValue(sudokuCell);

            ApplyLevelDifficulty(levelDifficulty);

            time.Stop();

            ExecutionTime = time.ElapsedMilliseconds;

            return this;
        }

        private void ApplyLevelDifficulty(LevelDifficulty levelDifficulty)
        {
            int refVal = 81; // Ref value for 3x3 grid.
            int valToHide = 0; // This amount of cell value will be hidden
            int lowerBoundForExtremelyEasy = (_gridSize * 50) / refVal; // Ref value for lower bound 
            int lowerBoundForEasy = (_gridSize * 36) / refVal; // Ref value for lower bound 
            int lowerBoundForMedium = (_gridSize * 32) / refVal; // Ref value for lower bound 
            int lowerBoundForDifficult = (_gridSize * 28) / refVal; // Ref value for lower bound 
            int lowerBoundForEvil = (_gridSize * 22) / refVal; // Ref value for lower bound 

            switch (levelDifficulty)
            {
                case LevelDifficulty.ExtremelyEasy:
                    valToHide = _gridSize - new Random().Next(lowerBoundForExtremelyEasy, _gridSize - 1);
                    break;
                case LevelDifficulty.Easy:
                    valToHide = _gridSize - new Random().Next(lowerBoundForEasy, lowerBoundForExtremelyEasy);
                    break;
                case LevelDifficulty.Medium:
                    valToHide = _gridSize - new Random().Next(lowerBoundForMedium, lowerBoundForEasy);
                    break;
                case LevelDifficulty.Difficult:
                    valToHide = _gridSize - new Random().Next(lowerBoundForDifficult, lowerBoundForMedium);
                    break;
                case LevelDifficulty.Evil:
                    valToHide = _gridSize - new Random().Next(lowerBoundForEvil, lowerBoundForDifficult);
                    break;
            }

            int sector, index;
            SudokuCell sudokuCell;
            for (int i = 0; i < valToHide; i++)
            {
                sector = SelectSection();

                index = GetRandomIndexInSector(sector);

                sudokuCell = _sudokuCellList.SingleOrDefault(x => x.Index == index);

                sudokuCell.Hidden = true;
            }

        }

        private int GetRandomIndexInSector(int sector)
        {
            List<SudokuCell> temp = _sudokuCellList.Where(x => x.Sector == sector && !x.Hidden).ToList();

            return temp[new Random().Next(0, temp.Count - 1)].Index;
        }

        private int SelectSection()
        {
            List<SectorRate> groupedSectors = new List<SectorRate>();

            SectorRate sectorRate = null;

            for (int i = 1; i <= GetNumberOfSectors(); i++)
            {
                sectorRate = new SectorRate();

                sectorRate.Sector = i;
                sectorRate.Rate = Convert.ToDouble(_sudokuCellList.Where(x => x.Sector == i && !x.Hidden).ToList().Count) / Convert.ToDouble(_rowSize);

                groupedSectors.Add(sectorRate);
            }

            return groupedSectors.OrderByDescending(x => x.Rate).First().Sector;
        }

        private class SectorRate
        {
            public int Sector { get; set; }
            public double Rate { get; set; }
        }

        /// <summary>
        /// A recursive function to genarete random values for each cell.
        /// In each recall, it is called with the previous SudokuCell in sudokuCellList.
        /// This function uses BackTrack Algorithm to fill each cell when a conflict occurs.
        /// </summary>
        /// <param name="sudokuCell">When NextIndex is null, current cell. Otherwise previous element that points to current cell</param>
        /// <param name="NextIndex">When this function is called recursively, NextIndex points the current cell's index.</param>
        private void GenerateRandomValue(SudokuCell sudokuCell, int? NextIndex = null)
        {
            do
            {
                if (sudokuCell.AvaliableIntList.Count(x => !x.Deleted) == 0)
                {
                    //We need a reset for the current cell because we used all available numbers
                    sudokuCell.Reset();

                    if (sudokuCell.Index == 0) // Worst case; we are starting to fill each cell from the beginning
                        GenerateRandomValue(_sudokuCellList[0], 1);
                    else
                        GenerateRandomValue(_sudokuCellList[sudokuCell.Index - 1], sudokuCell.Index);
                }

                sudokuCell.Value = PickNextAvailableValue(sudokuCell.AvaliableIntList);
            } while (!CheckConstraints(sudokuCell) || sudokuCell.Value == 0);
        }

        /// <summary>
        /// Checks sudoku constraints for the Sudoku Cell.
        /// Constraints are;
        ///     1. The value must be unique in row
        ///     2. The value must be unique in sector
        ///     3. The value must be unique in column
        /// </summary>
        /// <param name="sudokuCell"></param>
        /// <returns></returns>
        private bool CheckConstraints(SudokuCell sudokuCell)
        {
            return !IfExistsInSector(sudokuCell) && !IfExistsInRow(sudokuCell) && !IfExistsInColumn(sudokuCell);
        }

        private bool IfExistsInColumn(SudokuCell sudokuCell)
        {
            return _sudokuCellList.Count(x => x.Value > 0 && x.Index != sudokuCell.Index && x.Column == sudokuCell.Column && x.Value == sudokuCell.Value) > 0;
        }

        private bool IfExistsInRow(SudokuCell sudokuCell)
        {
            return _sudokuCellList.Count(x => x.Value > 0 && x.Index != sudokuCell.Index && x.Row == sudokuCell.Row && x.Value == sudokuCell.Value) > 0;
        }

        private bool IfExistsInSector(SudokuCell sudokuCell)
        {
            return _sudokuCellList.Count(x => x.Value > 0 && x.Index != sudokuCell.Index && x.Sector == sudokuCell.Sector && x.Value == sudokuCell.Value) > 0;
        }

        private int PickNextAvailableValue(List<AvaliableInt> avaliableIntList)
        {
            int value;

            if (avaliableIntList.Count == 0)
            {
                ResetAvaliableList(avaliableIntList);
                value = 0;
            }
            else
            {
                List<AvaliableInt> dummyList = avaliableIntList.Where(x => !x.Deleted).ToList();
                AvaliableInt avaliableInt = dummyList[new Random().Next(0, dummyList.Count - 1)];
                avaliableInt.Deleted = true;
                value = avaliableInt.Value;
            }

            return value;
        }

        private void ResetAvaliableList(List<AvaliableInt> avaliableIntList)
        {
            foreach (AvaliableInt avaliableInt in avaliableIntList)
                avaliableInt.Deleted = false;
        }

        /// <summary>
        /// Creates a list of Sudoku Cells and determines each cell's sector,
        /// index, row and column values. List of available integer values
        /// and the initial value of Value property also set in that function.
        /// </summary>
        private void InitSudokuCells()
        {
            SudokuCell sudokuCell = null;

            int row = 0, column = 0, sector = 0, count = 1, startSector = 0;

            for (int i = 0; i < _gridSize; i++)
            {
                sudokuCell = new SudokuCell(_rowSize);

                if (i % _rowSize == 0 && i > 0)
                {
                    column = 0;
                    row++;

                    if (row % _sectorRowSize != 0 && count <= _sectorRowSize)
                        sector = startSector;
                    else
                    {
                        startSector = sector;
                        count = 1;
                    }
                }

                if (column % _sectorRowSize == 0)
                    sector++;

                sudokuCell.Value = 0;
                sudokuCell.Column = column;
                sudokuCell.Row = row;
                sudokuCell.Sector = sector;
                sudokuCell.Index = i;

                column++;

                _sudokuCellList.Add(sudokuCell);
            }
        }

        public int GetGridSize()
        {
            return _gridSize;
        }

        /// <summary>
        /// Returns number of Sectors
        /// </summary>
        /// <returns></returns>
        public int GetNumberOfSectors()
        {
            return _gridSize / _rowSize;
        }

        /// <summary>
        /// Returns the Generated Sudoku Cells
        /// </summary>
        /// <returns></returns>
        public List<SudokuCell> GetSudokuCells()
        {
            return _sudokuCellList;
        }

        public SudokuCell GetCellAtIndex(int Index)
        {
            return _sudokuCellList.SingleOrDefault(x => x.Index == Index);
        }

        public SudokuCell GetCellAtPoint(int row, int column)
        {
            return _sudokuCellList.SingleOrDefault(x => x.Row == row && x.Column == column);
        }

        public List<SudokuCell> GetCellsInSector(int sector)
        {
            return _sudokuCellList.Where(x => x.Sector == sector).ToList();
        }

        public List<SudokuCell> GetCellsInRow(int row)
        {
            return _sudokuCellList.Where(x => x.Row == row).ToList();
        }

        public List<SudokuCell> GetCellsInColumn(int column)
        {
            return _sudokuCellList.Where(x => x.Column == column).ToList();
        }

        /// <summary>
        /// Outputs generated sudoku to output window
        /// </summary>
        /// <param name="HideLevelValues">Hides values which player should find</param>
        public void Print(bool HideLevelValues = false)
        {
            List<SudokuCell> sectorList = null;

            for (int i = 1; i <= GetNumberOfSectors(); i++)
            {
                sectorList = _sudokuCellList.Where(x => x.Sector == i).ToList();

                System.Diagnostics.Debug.Write(String.Format("\n{0}. SECTOR\n", sectorList[0].Sector));

                for (int j = 1; j <= _rowSize; j++)
                {
                    System.Diagnostics.Debug.Write(String.Format("{0} ", HideLevelValues && sectorList[j - 1].Hidden ? "*" : sectorList[j - 1].Value.ToString()));

                    if (j % _sectorRowSize == 0)
                        System.Diagnostics.Debug.Write("\n");
                }
            }

            foreach (var sudokuCell in _sudokuCellList)
                System.Diagnostics.Debug.Write(sudokuCell.Value + " ");
        }

        public class SudokuCell
        {
            public int Value { get; set; }

            public int Column { get; set; }

            public int Row { get; set; }

            public int Sector { get; set; }

            public int Index { get; set; }

            /// <summary>
            /// This property is used to hide value in levels
            /// </summary>
            public bool Hidden { get; set; }

            /// <summary>
            /// This property is used when values are generated.
            /// </summary>
            public List<AvaliableInt> AvaliableIntList { get; set; }

            public SudokuCell(int rowSize)
            {
                AvaliableIntList = new List<AvaliableInt>();

                for (int i = 1; i <= rowSize; i++)
                {
                    AvaliableIntList.Add(new AvaliableInt()
                    {
                        Value = i,
                        Deleted = false
                    });
                }
            }

            public void Reset()
            {
                Value = 0;

                foreach (AvaliableInt avaliableInt in AvaliableIntList)
                    avaliableInt.Deleted = false;
            }
        }

        public class AvaliableInt
        {
            public int Value { get; set; }

            public bool Deleted { get; set; }
        }

        public enum LevelDifficulty
        {
            ExtremelyEasy = 1,
            Easy,
            Medium,
            Difficult,
            Evil
        }
    }
}