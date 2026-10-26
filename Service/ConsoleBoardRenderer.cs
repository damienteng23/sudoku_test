using sudoku.ValueObject;

namespace sudoku.Service
{
    public static class ConsoleBoardRenderer
    {
        public static void Display(SudokuGrid grid)
        {
            Console.WriteLine();
            Console.WriteLine("    1 2 3   4 5 6   7 8 9");
            Console.WriteLine("  +-------+-------+-------+");

            for (int row = 0; row < SudokuGrid.Size; row++)
            {
                Console.Write((char)('A' + row) + " | ");

                for (int column = 0; column < SudokuGrid.Size; column++)
                {
                    int value = grid.GetValue(new CellPosition(row, column));
                    Console.Write(value == 0 ? "." : value.ToString());

                    if (column == 2 || column == 5)
                        Console.Write(" | ");
                    else if (column < 8)
                        Console.Write(" ");
                }

                Console.WriteLine(" |");

                if (row == 2 || row == 5 || row == 8)
                    Console.WriteLine("  +-------+-------+-------+");
            }

            Console.WriteLine();
        }
    }
}
