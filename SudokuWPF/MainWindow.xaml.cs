using SudokuLib;
using System.Windows;

namespace SudokuWPF
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        public MainWindow()
        {
            InitializeComponent();
            Sudoku = new Sudoku(5, 2);
            this.DataContext = this;
        }

        public Sudoku Sudoku { get; set; }
    }
}
