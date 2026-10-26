using sudoku.Enum;
using sudoku.ValueObject;

namespace sudoku.Model
{
    public sealed record GameCommand(
        CommandTypeEnum Type,
        CellPosition? Position = null,
        int Number = 0);
}
