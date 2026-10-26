namespace sudoku.ValueObject
{
    public sealed class SudokuGrid
    {
        private readonly int[] cells;
        public const int Size = 9;
        public const int CellCount = Size * Size;

        public SudokuGrid(IEnumerable<int> values)
        {
            ArgumentNullException.ThrowIfNull(values);
            cells = values.ToArray();
            if (cells.Length != CellCount || cells.Any(value => value is < 0 or > 9))
                throw new ArgumentException("A grid must contain exactly 81 values between 0 and 9.", nameof(values));
        }

        public void SetValue(CellPosition position, int value)
        {
            if (value < 0 || value > 9)
                throw new ArgumentOutOfRangeException(nameof(value));

            cells[position.Index] = value;
        }

        public int GetValue(CellPosition position)
        {
            return cells[position.Index];
        }

        public int GetFilledCount()
        {
            int count = 0;

            foreach (int value in cells)
            {
                if (value != 0)
                    count++;
            }

            return count;
        }

        public int[] GetArray()
        {
            return (int[])cells.Clone();
        }
    }
}
