using sudoku.Enum;

namespace sudoku.ValueObject
{
    public sealed record MoveResult(MoveErrorEnum Error, CellPosition? Position = null, int? Number = null)
    {
        public bool Accepted => Error == MoveErrorEnum.None;
    }
}
