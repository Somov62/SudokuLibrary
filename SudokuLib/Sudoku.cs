using System;
using System.Collections.Generic;
using System.Linq;
using SudokuLib.Entities;
using SudokuLib.GeneratorTools;

namespace SudokuLib
{
    public class Sudoku
    {
        public Sudoku(int countChunksInDimension = 3, int difficultyLevel = 2)
        {
            CountChunksInDimension = countChunksInDimension;
            Matrix = new Generator().GenerateSudoku(CountChunksInDimension, difficultyLevel);

            ChunksArchiver archiver = new();
            Chunks = archiver.PackInChunks(Matrix);
        }
        public int CountChunksInDimension { get; set; }
        public int[,] Matrix { get; set; }
        public List<Chunk> Chunks { get; set; }

        public int FreeSeatsCount()
        {
            int count = 0;
            foreach (var item in Chunks)
            {
                count += item.FreeSeatsCount();
            }
            return count;
        }

        public bool Validate()
        {
            SudokuChecker checker = new();
            return checker.ValidateSudoku(Chunks);
        }
    }
}
