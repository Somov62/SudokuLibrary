namespace SudokuLib.Entities
{
    public class Number
    {
        public Number()
        {
            IsDefault = true;
        }

        /// <summary>
        /// Хранит верное значение ячейки
        /// </summary>
        public int Value { get; set; }

        /// <summary>
        /// Метка стандартной цифры.
        /// Такие цифры расставлены на сетке при старте игры.
        /// Запрещает пользователю удалять значение
        /// </summary>
        public bool IsDefault { get; set; }

    }
}
