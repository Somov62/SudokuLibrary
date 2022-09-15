// See https://aka.ms/new-console-template for more information
using SudokuLib;

Sudoku sudoku = new Sudoku(3, 1);

//Print(sudoku.Matrix);

void Print(int[,] matrix1)
{
    for (int i = 0; i < 9; i++)
    {
        if (i % 3 == 0) Console.WriteLine();
        for (int j = 0; j < 9; j++)
        {
            if (j % 3 == 0) Console.Write(" ");
            //if (matrix1[i, j] == 0)
            //{
            //    Console.Write(" ");
            //    continue;
            //}
            Console.Write(matrix1[i, j]);
        }
        Console.WriteLine();
    }
}