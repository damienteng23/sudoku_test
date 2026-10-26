using sudoku.Enum;

namespace sudoku.ValueObject
{
    public sealed record RuleViolation(UnitTypeEnum Unit, int UnitIndex, int Number);
}
