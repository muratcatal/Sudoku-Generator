# Sudoku-Generator
Sudoku Generator Written in C#

## Overview

 This is a library to generate sudoku which has only one result. Library uses *Back-Track Algorithm* to find exact solution.
 
 **Sudoku Generator has below functionalities**
   1. Create sudoku in 5 different level difficulty
   2. Create sudoku in desired size (3x3, 4x4, 5x5 ..)

## Class Overview
 **Sudoku Class has below functions which you can use**

      1. SudokuGrid Generate(int n, LevelDifficulty levelDifficulty)
      2. int GetGridSize()
      3. int GetNumberOfSectors()
      4. List<SudokuCell> GetSudokuCells()
      5. SudokuCell GetCellAtIndex(int Index) 
      6. SudokuCell GetCellAtPoint(int row, int column)
      7. List<SudokuCell> GetCellsInSector(int sector)
      8. List<SudokuCell> GetCellsInRow(int row)
      9. List<SudokuCell> GetCellsInColumn(int column)
      10. void Print(bool HideLevelValues = false)

## Usage
  **To generate a valid sudoku**
  
    SudokuGrid sudokuGrid = SudokuGenerator.Generate(9, SudokuGrid.LevelDifficulty.Easy);
    
  **To get generated sudoku values**
  
    List<SudokuGrid.SudokuCell> sudokuCells = sudokuGrid.GetSudokuCells();
    
  **To get values on a sector**
  
    List<SudokuGrid.SudokuCell> cellsInSector = sudokuGrid.GetCellsInSector(0);
    
  **To get value from a point**
  
    SudokuGrid.SudokuCell cellAtPoint = sudokuGrid.GetCellAtPoint(0, 1);
    
  **To get value at index**
  
  SudokuGrid.SudokuCell cellAtIndex = sudokuGrid.GetCellAtIndex(0);
  
  **To get values from column**
  
  List<SudokuGrid.SudokuCell> cellsInColumn = sudokuGrid.GetCellsInColumn(0);
  
  **To get values from row**
  
  List<SudokuGrid.SudokuCell> cellsInRow = sudokuGrid.GetCellsInRow(0);
  
  **To get grid size**
  
  int gridSize = sudokuGrid.GetGridSize();
  
  **To get number of sectors**
  
  int numberOfSectors = sudokuGrid.GetNumberOfSectors();
  
  **Print generated values to output window of Visual Studio**
  
  sudokuGrid.Print();
  
## Contributions

All pull request are welcome to make this library more useful
        
