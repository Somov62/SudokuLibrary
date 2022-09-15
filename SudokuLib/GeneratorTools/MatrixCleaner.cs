using System;

namespace SudokuLib.GeneratorTools
{
    internal class MatrixCleaner
    {
        private int[,] _matrix;
        private int _chunkSize;

        /// <summary>
        /// Сокрытие части цифр, которые пользователь будет ставить
        /// </summary>
        /// <param name="difficultyLevel">Уровень сложности 0-4</param>
        public void DeleteNumbers(int[,] matrix, int difficultyLevel)
        {
            _matrix = matrix;
            _chunkSize = (int)Math.Sqrt(matrix.GetLength(0));

            //Количество цифр для удаления
            double countNumbersForDelete = matrix.Length;
            countNumbersForDelete *= difficultyLevel switch
            {
                1 => 0.2,
                2 => 0.4,
                3 => 0.6,
                4 => 0.7,
                _ => 0.1,
            };
            Math.Floor(countNumbersForDelete);

            int[,] filledMatrix = new int[matrix.GetLength(0), matrix.GetLength(1)];

            for (int i = 0; i < _matrix.GetLength(0); i++)
            {
                for (int j = 0; j < _matrix.GetLength(1); j++)
                {
                    filledMatrix[i, j] = _matrix[i, j];
                }
            }

            var rnd = SigletonRandom.GetInstance();

            while (countNumbersForDelete > 0)
            {
                //Получения случайных координат в матрице
                int row = rnd.Next(_matrix.GetLength(0));
                int column = rnd.Next(_matrix.GetLength(0));

                //Получение числа из этих координат
                var number = _matrix[row, column];

                //Если число уже скрыто - пропускаем
                if (number == 0) continue;

                //Детектим и запрещаем скрывать все числа в строке, столбце, чанке
                if (GetCountDeleteInChunk(row / _chunkSize, column / _chunkSize) == _matrix.GetLength(0) - 1) continue;
                if (GetCountDeleteInColumn(column) == _matrix.GetLength(0) - 1) continue;
                if (GetCountDeleteInRow(row) == _matrix.GetLength(0) - 1) continue;

                //Скрываем число
                _matrix[row, column] = 0;

                SudokuSolver solver = new SudokuSolver(new Grid(matrix));
                var solvedMatrix = solver.SolvePuzzle();

                if (!CheckMatrixs(filledMatrix, solvedMatrix))
                {
                    _matrix[row, column] = number;
                    continue;
                }

                countNumbersForDelete--;
            }

            //Сравнение исходной матрицы и матрицей, решенной программным способом
            bool CheckMatrixs(int[,] firstMatrix, int[,] secondMatrix)
            {
                for (int i = 0; i < firstMatrix.GetLength(0); i++)
                {
                    for (int j = 0; j < firstMatrix.GetLength(1); j++)
                    {
                        if (secondMatrix[i, j] != firstMatrix[i, j]) return false;
                    }
                }
                return true;
            }
        }

        private int GetCountDeleteInRow(int row)
        {
            int count = 0;
            for (int column = 0; column < _matrix.GetLength(0); column++)
                if (_matrix[row, column] == 0) count++;
            return count;
        }

        private int GetCountDeleteInColumn(int column)
        {
            int count = 0;
            for (int row = 0; row < _chunkSize; row++)
                if (_matrix[row, column] == 0) count++;
            return count;
        }

        private int GetCountDeleteInChunk(int chunkRow, int chunkColumn)
        {
            int count = 0;
            for (int numberColumn = chunkRow; numberColumn < _chunkSize; numberColumn++)
                for (int numberRow = chunkColumn; numberRow < _chunkSize; numberRow++)
                    if (_matrix[numberRow, numberColumn] == 0) count++;
            return count;
        }

    }
}
