using SudokuLib.Entities;
using SudokuLib.GeneratorTools;
using System;
using System.Collections.Generic;
using System.Linq;

namespace SudokuLib
{
    internal class SudokuChecker
    {
        private int[,] _matrix;


        public bool ValidateSudoku(List<Chunk> chunks)
        {
            ChunksArchiver archiver = new ChunksArchiver();
            _matrix = archiver.ExtractChunks(chunks);

            if (!ValidateRows()) return false;
            if (!ValidateColumns()) return false;
            if (!ValidateChunks()) return false;

            return true;
        }


        private bool ValidateRows()
        {
            for (int i = 0; i < _matrix.GetLength(0); i++)
            {
                if (!ValidateRow(i)) return false;
            }
            return true;

            //Проверка на коллизию в строке
            bool ValidateRow(int row)
            {
                var digits = Enumerable.Range(1, _matrix.GetLength(0)).ToList();
                for (int column = 0; column < _matrix.GetLength(1); column++)
                {
                    if (!digits.Contains(_matrix[row, column])) return false;
                    digits.Remove(_matrix[row, column]);
                }
                return true;
            }
        }

        private bool ValidateColumns()
        {
            for (int j = 0; j < _matrix.GetLength(0); j++)
            {
                if (!ValidateColumn(j)) return false;
            }
            return true;

            //Проверка на коллизию в столбце
            bool ValidateColumn(int column)
            {
                var digits = Enumerable.Range(1, _matrix.GetLength(0)).ToList();
                for (int row = 0; row < _matrix.GetLength(1); row++)
                {
                    if (!digits.Contains(_matrix[row, column])) return false;
                    digits.Remove(_matrix[row, column]);
                }
                return true;
            }
        }

        private bool ValidateChunks()
        {
            int chunkSize = (int)Math.Sqrt(_matrix.GetLength(0));

            for (int chunkRow = 0; chunkRow < chunkSize; chunkRow++)
            {
                for (int chunkColumn = 0; chunkColumn < chunkSize; chunkColumn++)
                {
                    if (!ValidateChunk(chunkRow, chunkColumn)) return false;
                }
            }
            return true;

            //Проверка на коллизию в чанке
            bool ValidateChunk(int chunkRow, int chunkColumn)
            {
                var digits = Enumerable.Range(1, _matrix.GetLength(0)).ToList();
                for (int row = chunkRow; row < chunkSize; row++)
                {
                    for (int column = chunkColumn; column < chunkSize; column++)
                    {
                        if (!digits.Contains(_matrix[row, column])) return false;
                        digits.Remove(_matrix[row, column]);
                    }
                }

                return true;
            }
        }
    }
}
