namespace sudoku.ValueObject
{
    public class CellPosition
    {
        public int Row { get; }
        public int Column { get; }
        public int Index => Row * 9 + Column;

        public CellPosition(int row, int column)
        {
            if (row is < 0 or > 8) throw new ArgumentOutOfRangeException(nameof(row));
            if (column is < 0 or > 8) throw new ArgumentOutOfRangeException(nameof(column));
            Row = row;
            Column = column;
        }

        public override string ToString() => $"{(char)('A' + Row)}{Column + 1}";
    }
}
