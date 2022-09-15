using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using SudokuLib.Entities;

namespace SudokuLib
{
    internal class SudokuSolver
    {
        private readonly Grid _grid;

        public SudokuSolver(Grid grid)
        {
            _grid = grid;
            _grid.Validate();
        }

        public int[,] SolvePuzzle()
        {
            Solve();
            return _grid.Data;
        }

        private bool Solve()
        {
            int row, col;
            if (!_grid.FindUnassignedLoc(out row, out col))
            {
                return true;
            }

            for (int num = 1; num <= _grid.Data.GetLength(0); num++)
            {
                if (_grid.NoConflicts(row, col, num))
                {
                    _grid.Assign(row, col, num);

                    if (Solve())
                    {
                        return true;
                    }
                    _grid.Unassign(row, col);
                }
            }

            return false;
        }

        public int[,] Data
        {
            get { return _grid.Data; }
        }
    }

    internal class Grid
    {
        public int[,] Data { get; private set; }
        private int _curC = 0;
        private int _curR = 0;
        private int _assigns = 0;
        private int _chunkSize = 0;

        public Grid(int[,] matrix)
        {
            _chunkSize = (int)Math.Sqrt(matrix.GetLength(0));
            int[,] data = new int[matrix.GetLength(0), matrix.GetLength(1)];
            for (int i = 0; i < data.GetLength(0); i++)
            {
                for (int j = 0; j < data.GetLength(1); j++)
                {
                    data[i, j] = matrix[i, j];
                }
            }
            Data = data;
        }

        public bool FindUnassignedLoc(out int row, out int col)
        {
            while (Data[_curR, _curC] != 0)
            {
                _curC++;

                if (_curC == Data.GetLength(1))
                {
                    _curR++;
                    _curC = 0;
                }

                if (_curR == Data.GetLength(0))
                {
                    row = -1;
                    col = -1;
                    return false;
                }
            }

            row = _curR;
            col = _curC;

            return true;
        }

        public bool NoConflicts(int row, int col, int num)
        {
            for (int r = 0; r < Data.GetLength(0); ++r)
            {
                if (Data[r, col] == num)
                {
                    return false;
                }
            }

            for (int c = 0; c < Data.GetLength(0); c++)
            {
                if (Data[row, c] == num)
                {
                    return false;
                }
            }

            int fromC = _chunkSize * (col / _chunkSize);
            int fromR = _chunkSize * (row / _chunkSize);

            for (int c = fromC; c < fromC + _chunkSize; c++)
            {
                for (int r = fromR; r < fromR + _chunkSize; r++)
                {
                    if (Data[r, c] == num)
                    {
                        return false;
                    }
                }
            }

            return true;
        }

        public void Assign(int row, int col, int num)
        {
            _assigns++;
            Data[row, col] = num;
        }

        public void Unassign(int row, int col)
        {
            Data[row, col] = 0;
            _curC = col;
            _curR = row;
        }

        public int Assigns
        {
            get { return _assigns; }
        }

        public void Validate()
        {
            if (Data.GetLength(0) != Data.GetLength(1))
            {
                throw new Exception("Invalid dimentions!");
            }

            if (!IsLegal())
            {
                throw new Exception("Illigal numbers populated!");
            }
        }

        public bool IsLegal()
        {
            var container = new HashSet<int>();
            //vertical check 
            for (int c = 0; c < Data.GetLength(1); ++c)
            {
                container.Clear();
                for (int r = 0; r < Data.GetLength(0); ++r)
                {
                    if (Data[r, c] != 0)
                    {
                        if (container.Contains(Data[r, c]))
                        {
                            return false;
                        }
                        container.Add(Data[r, c]);
                    }
                }
            }
            // horizontal check
            for (int r = 0; r < Data.GetLength(0); ++r)
            {
                container.Clear();
                for (int c = 0; c < Data.GetLength(1); ++c)
                {
                    if (Data[r, c] != 0)
                    {
                        if (container.Contains(Data[r, c]))
                        {
                            return false;
                        }
                        container.Add(Data[r, c]);
                    }
                }
            }

            // square check
            for (int fromC = 0; fromC < _chunkSize * _chunkSize; fromC += _chunkSize)
            {
                for (int fromR = 0; fromR < _chunkSize * _chunkSize; fromR += _chunkSize)
                {
                    container.Clear();

                    for (int c = fromC; c < fromC + 3; c++)
                    {
                        for (int r = fromR; r < fromR + 3; r++)
                        {
                            if (Data[r, c] != 0)
                            {
                                if (container.Contains(Data[r, c]))
                                {
                                    return false;
                                }
                                container.Add(Data[r, c]);
                            }
                        }
                    }
                }
            }

            return true;
        }
    }
}
