using System;

namespace SudokuLib.GeneratorTools
{
    internal static class SigletonRandom
    {
        private static Random _random;

        public static Random GetInstance()
        {
            return _random ??= new Random();
        }
    }
}
