using sudoku.ValueObject;

namespace sudoku.Model
{
    public sealed record GameState(SudokuGrid Grid, bool IsComplete);
}