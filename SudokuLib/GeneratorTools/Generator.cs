using SudokuLib.Entities;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;

namespace SudokuLib.GeneratorTools
{
    internal class Generator
    {
        private int _countSetNumbers;
        private int[,] _rawMatrixData;
        private readonly Random _rnd = SigletonRandom.GetInstance();

        public Generator() { }

        /// <summary>
        /// Длина одного измерения чанка.
        /// Площадь чанка = _size * _size
        /// </summary>
        public int ChunkSize { get; private set; }

        private void RemixArray(List<int> array)
        {
            for (int i = array.Count - 1; i > 0; i--)
            {
                int j = _rnd.Next(i);
                var t = array[i];
                array[i] = array[j];
                array[j] = t;
            }
        }

        /// <summary>
        /// Генирирует судоку
        /// </summary>
        /// <param name="countChunksInDimension">Количество чанков в одном измерении</param>
        /// <param name="difficultyLevel">Уровень сложности 0-4</param>
        /// <returns></returns>
        public int[,] GenerateSudoku(int countChunksInDimension, int difficultyLevel)
        {
            if (countChunksInDimension < 3) countChunksInDimension = 3;
            ChunkSize = countChunksInDimension;

            int[,] chunks = null;
            while (chunks == null)
            {
                chunks = Generate();
            }
            return chunks;

            int[,] Generate()
            {
                int countNumbers = countChunksInDimension * countChunksInDimension;
                _rawMatrixData = new int[countNumbers, countNumbers];

                List<int> fillNumbers = Enumerable.Range(1, _rawMatrixData.GetLength(0)).ToList();
                RemixArray(fillNumbers);

                _countSetNumbers = 0;

                foreach (var item in fillNumbers)
                {
                    if (SetNumber(item) == false) return null;
                }

                for (int i = 0; i < 9; i++)
                {
                    if (i % 3 == 0) Debug.WriteLine("");
                    for (int j = 0; j < 9; j++)
                    {
                        if (j % 3 == 0) Debug.Write(" ");
                        //if (matrix1[i, j] == 0)
                        //{
                        //    Console.Write(" ");
                        //    continue;
                        //}
                        Debug.Write(_rawMatrixData[i, j]);
                    }
                    Debug.WriteLine("");
                }


                MatrixCleaner cleaner = new MatrixCleaner();
                cleaner.DeleteNumbers(_rawMatrixData, difficultyLevel);

                for (int i = 0; i < 9; i++)
                {
                    if (i % 3 == 0) Debug.WriteLine("");
                    for (int j = 0; j < 9; j++)
                    {
                        if (j % 3 == 0) Debug.Write(" ");
                        //if (matrix1[i, j] == 0)
                        //{
                        //    Console.Write(" ");
                        //    continue;
                        //}
                        Debug.Write(_rawMatrixData[i, j]);
                    }
                    Debug.WriteLine("");
                }


                return _rawMatrixData;
            }
        }

        /// <summary>
        /// Ставит заданную цифру во все чанки, избегая коллизий
        /// </summary>
        /// <param name="number"></param>
        private bool SetNumber(int number)
        {
            //Показатель коллизий
            bool isDirty = false;
            //Хранит историю заполнения матрицы
            List<(int, int)> lastCoordList = new List<(int, int)>();

            for (int j = 0; j < _rawMatrixData.GetLength(0); j += ChunkSize)
            {
                for (int i = 0; i < _rawMatrixData.GetLength(0); i += ChunkSize)
                {
                    //Если были коллизии, чистим матрицу от технических значений
                    if (isDirty)
                    {
                        isDirty = false;
                        Clean(i, j);
                    }

                    //Получаем список свободных мест для заданного числа в чанке
                    var coords = GetFreePlaces(number, i, j);

                    if (coords.Count > 0)
                    {
                        //Выбираем случайное свободное место
                        var coord = coords[_rnd.Next(0, coords.Count)];
                        //Заполняем матрицу
                        _rawMatrixData[coord.Item1, coord.Item2] = number;
                        //Сохраняем в истории
                        lastCoordList.Add(coord);
                        //Очистим массив от полученных коллизий
                        isDirty = true;
                        continue;
                    }

                    //Свободных мест нет, коллизия

                    //Если чанк первый, очищаем его и пробуем заново
                    if (i == 0 && j == 0)
                    {
                        Clean(0, 0);
                        return false;
                    }

                    //Сценарий, последнего чанка, в котором осталось поставить 2 цифры
                    //Коллизии не избежать, устраняем ее специальным методом
                    if (_countSetNumbers + 2 == _rawMatrixData.GetLength(0) && i == _rawMatrixData.GetLength(0) - ChunkSize && j == _rawMatrixData.GetLength(0) - ChunkSize)
                    {
                        Clean(0, 0);
                        NormilazeChunk(i, j, number);
                        continue;
                    }

                    //Чтобы избежать коллизии мы сделаем шаг назад -
                    //1) удалим последний элемент и
                    //2) попробуем поставить его на другое место

                    //1) берем последнюю запись из истории и затираем ее
                    var lastCoord = lastCoordList.Last();
                    _rawMatrixData[lastCoord.Item1, lastCoord.Item2] = -1;
                    lastCoordList.Remove(lastCoord);

                    //2) Смещаемся на чанк назад, чтобы заново поставить элемент
                    if (i == 0)
                    {
                        i = _rawMatrixData.GetLength(0) - 2 * ChunkSize;
                        j -= ChunkSize;
                        continue;
                    }
                    i -= 2 * ChunkSize;
                }
            }
            //Увеличиваем количество поставленных цифр
            _countSetNumbers++;
            return true;
        }


        /// <summary>
        /// Находит свободные места для цифры в чанке, избегая коллизии
        /// </summary>
        /// <param name="number">Заданное число</param>
        /// <param name="chunkRow">Индекс первой строки в чанке</param>
        /// <param name="chunkColumn">Индекс первого столбца в чанке</param>
        /// <returns>Коллекцию координат</returns>
        private List<(int, int)> GetFreePlaces(int number, int chunkRow, int chunkColumn)
        {
            List<(int, int)> list = new();
            for (int i = chunkRow; i < chunkRow + ChunkSize; i++)
            {
                for (int j = chunkColumn; j < chunkColumn + ChunkSize; j++)
                {
                    if (_rawMatrixData[i, j] != 0) continue;
                    //Ищет такую же цифру в строке
                    if (CheckNumberInRow(i, number, -1)) break;
                    //Ищет такую же цифру в столбце
                    if (CheckNumberInColumn(j, number, -1)) continue;

                    //Если нет, следит, чтобы строки заполняли равномерно
                    if (GetCount0InRow(i) < ChunkSize * ChunkSize - _countSetNumbers) continue;
                    if (GetCount0InColumn(j) < ChunkSize * ChunkSize - _countSetNumbers) continue;
                    //При соблюдении всех условий, координата считается свободной
                    list.Add((i, j));
                }
            }
            return list;
        }


        /// <summary>
        /// Находит такую же цифру в заданной строке
        /// </summary>
        /// <param name="row">Индекс строки</param>
        /// <param name="number">Цифра</param>
        /// <param name="skipIndex">Индекс, который следует не учитывать</param>
        /// <returns>true - если цифра есть, false - если цифры нет</returns>
        private bool CheckNumberInRow(int row, int number, int skipIndex)
        {
            for (int column = 0; column < _rawMatrixData.GetLength(0); column++)
            {
                if (column == skipIndex)
                    continue;
                if (_rawMatrixData[row, column] == number)
                    return true;
            }
            return false;
        }

        /// <summary>
        /// Находит такую же цифру в заданном столбце
        /// </summary>
        /// <param name="row">Индекс столбца</param>
        /// <param name="number">Цифра</param>
        /// <param name="skipIndex">Индекс, который следует не учитывать</param>
        /// <returns>true - если цифра есть, false - если цифры нет</returns>
        private bool CheckNumberInColumn(int column, int number, int skipIndex)
        {
            for (int row = 0; row < _rawMatrixData.GetLength(0); row++)
            {
                if (row == skipIndex)
                    continue;
                if (_rawMatrixData[row, column] == number)
                    return true;
            }
            return false;
        }

        /// <summary>
        /// Считает количество нулей в строке
        /// </summary>
        /// <param name="row">Индекс строки</param>
        /// <returns>Количество нулей в строке</returns>
        private int GetCount0InRow(int row)
        {
            int counter = 0;
            for (int column = 0; column < _rawMatrixData.GetLength(0); column++)
            {
                if (_rawMatrixData[row, column] < 1)
                    counter++;
            }
            return counter;
        }

        /// <summary>
        /// Считает количество нулей в столбце
        /// </summary>
        /// <param name="column">Индекс столбца</param>
        /// <returns>Количество нулей в столбце</returns>
        private int GetCount0InColumn(int column)
        {
            int counter = 0;
            for (int row = 0; row < _rawMatrixData.GetLength(0); row++)
            {
                if (_rawMatrixData[row, column] < 1)
                    counter++;
            }
            return counter;
        }

        /// <summary>
        /// Чистит чанки, начиная с указанного
        /// </summary>
        /// <param name="startChunkRow">Индекс строки начала первого чанка</param>
        /// <param name="startChunkColumn">Индекс столбца начала первого чанка</param>
        private void Clean(int startChunkRow, int startChunkColumn)
        {
            for (int j = startChunkColumn; j < _rawMatrixData.GetLength(0); j += ChunkSize)
            {
                for (int i = startChunkRow; i < _rawMatrixData.GetLength(0); i += ChunkSize)
                {
                    startChunkRow = 0;
                    CleanChunk(i, j);
                }
            }
        }

        /// <summary>
        /// Чистит чанк от заглушек коллизии
        /// </summary>
        /// <param name="chunkRow">Индекс строки начала чанка</param>
        /// <param name="chunkColumn">Индекст столбца начала чанка</param>
        private void CleanChunk(int chunkRow, int chunkColumn)
        {
            for (int i = chunkRow; i < chunkRow + ChunkSize; i++)
                for (int j = chunkColumn; j < chunkColumn + ChunkSize; j++)
                    if (_rawMatrixData[i, j] == -1) _rawMatrixData[i, j] = 0;
        }

        /// <summary>
        /// Убирает колллизию в последнем чанке путём прямым перестановок
        /// </summary>
        /// <param name="number">Число </param>
        /// <param name="chunkRow">Индекс строки начала чанка</param>
        /// <param name="chunkColumn">Индекст столбца начала чанка</param>
        private void NormilazeChunk(int chunkRow, int chunkColumn, int number)
        {
            //Ищем подходящий ноль и устраиваем перестановки
            for (int j = chunkColumn; j < chunkColumn + ChunkSize; j++)
            {
                for (int i = chunkRow; i < chunkRow + ChunkSize; i++)
                {
                    if (_rawMatrixData[i, j] != 0) continue;

                    //Ищем подходящий ноль
                    //Цифра, установленная в него (number)
                    //должна создавать одну коллизаию, а не две
                    bool numberInColumn = CheckNumberInColumn(j, number, i);
                    bool numberInRow = CheckNumberInRow(i, number, j);
                    //Если цифра создаст две коллизии - пропускам
                    if (numberInColumn && numberInRow) continue;

                    _rawMatrixData[i, j] = number;
                    //Если коллизия в столбце, меняем столбцы
                    if (numberInColumn)
                    {
                        for (int column = chunkColumn; column < chunkColumn + ChunkSize; column++)
                        {
                            if (column == j)
                                continue;
                            if (GetCount0InColumn(column) > 1)
                            {
                                NormalizeColumns(column, j, -1, 0);
                                break;
                            }
                        }
                        return;
                    }

                    for (int row = chunkRow; row < chunkRow + ChunkSize; row++)
                    {
                        if (row == i)
                            continue;
                        if (GetCount0InRow(row) > 1)
                        {
                            NormalizeRows(row, i, -1, 0);
                            break;
                        }
                    }
                    return;
                }
            }
        }

        /// <summary>
        /// Убирает повторения в столбце
        /// </summary>
        /// <param name="firstColumn">Индекс первого столбца</param>
        /// <param name="secondColumn">Индекс второго столбца</param>
        /// <param name="skipIndex">Индекс, который следует пропустить</param>
        /// <param name="number">Число, создвашее колллизию</param>
        private void NormalizeColumns(int firstColumn, int secondColumn, int skipIndex, int number)
        {
            for (int i = 0; i < _rawMatrixData.GetLength(0); i++)
            {
                if (i == skipIndex) continue;
                if (_rawMatrixData[i, firstColumn] != number) continue;
                Swap(ref _rawMatrixData[i, firstColumn], ref _rawMatrixData[i, secondColumn]);
                NormalizeColumns(firstColumn, secondColumn, i, _rawMatrixData[i, firstColumn]);
                return;
            }
        }

        /// <summary>
        /// Убирает повторения в строке
        /// </summary>
        /// <param name="firstRow">Индекс первого строки</param>
        /// <param name="secondRow">Индекс второго строки</param>
        /// <param name="skipIndex">Индекс, который следует пропустить</param>
        /// <param name="number">Число, создвашее колллизию</param>
        private void NormalizeRows(int firstRow, int secondRow, int skipIndex, int number)
        {
            for (int j = 0; j < _rawMatrixData.GetLength(0); j++)
            {
                if (j == skipIndex) continue;
                if (_rawMatrixData[firstRow, j] != number) continue;
                Swap(ref _rawMatrixData[firstRow, j], ref _rawMatrixData[secondRow, j]);
                NormalizeRows(firstRow, secondRow, j, _rawMatrixData[firstRow, j]);
                return;
            }
        }

        private void Swap(ref int firstVar, ref int secondVar)
        {
            if (firstVar == secondVar) return;
            int container = firstVar;
            firstVar = secondVar;
            secondVar = container;
        }

       

       
    }
}
