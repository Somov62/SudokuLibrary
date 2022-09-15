using SudokuLib.Entities;
using System;
using System.Collections.Generic;

namespace SudokuLib.GeneratorTools
{
    internal class ChunksArchiver
    {
        /// <summary>
        /// Упаковка матрицы чисел в матрицу чанков
        /// </summary>
        /// <returns>Матрица чанков</returns>
        public List<Chunk> PackInChunks(int[,] matrix)
        {
            if (matrix.GetLength(0) != matrix.GetLength(1)) return null;
            int chunkSize = (int)Math.Sqrt(matrix.GetLength(0));

            List<Chunk> chunks = new (chunkSize * chunkSize);

            for (int i = 0; i < chunkSize; i++)
                for (int j = 0; j < chunkSize; j++)
                    chunks.Add(PackInChunk(i * chunkSize, j * chunkSize));
            return chunks;

            Chunk PackInChunk(int chunkRow, int chunkColumn)
            {
                Chunk chunk = new Chunk(chunkSize);
                for (int j = 0; j < chunkSize; j++)
                    for (int i = 0; i < chunkSize; i++)
                        chunk[i, j].Value = matrix[i + chunkRow, j + chunkColumn];
                return chunk;
            }
        }

        /// <summary>
        /// Преобразование коллекции чанков в матрицу чисел
        /// </summary>
        /// <param name="chunks">Коллекция чанков</param>
        /// <returns>Матрица чисел</returns>
        public int[,] ExtractChunks(List<Chunk> chunks)
        {
            int chunkSize = (int)Math.Sqrt(chunks.Count);

            int[,] matrix = new int[chunkSize * chunkSize, chunkSize * chunkSize];

            int row = -1;
            int column = 0 - chunkSize;

            for (int i = 0; i < chunks.Count; i++)
            {
                if (i % chunkSize == 0)
                {
                    if (column == matrix.GetLength(0) - 1)
                    {
                        column = 0;
                        row += chunkSize;
                    }
                    else column += chunkSize;

                }
                for (int j = 0; j < chunks[i].ChunkData.Count; j++)
                {
                    if (j % chunkSize == 0)
                    {
                        row++;
                        column -= chunkSize;
                    }
                    matrix[row, column] = chunks[i].ChunkData[j].Value;
                    column++;
                }
                row -= chunkSize;
                column -= chunkSize;

            }

            return matrix;
        }

    }
}
