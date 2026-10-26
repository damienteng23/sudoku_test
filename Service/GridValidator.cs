using sudoku.Enum;
using sudoku.ValueObject;

namespace sudoku.Service
{
    public static class GridValidator
    {
        public static IReadOnlyList<RuleViolation> Validate(SudokuGrid grid)
        {
            var violations = new List<RuleViolation>();
            for (var unit = 0; unit < 9; unit++)
            {
                FindDuplicates(grid, UnitTypeEnum.Row, unit, violations);
                FindDuplicates(grid, UnitTypeEnum.Column, unit, violations);
                FindDuplicates(grid, UnitTypeEnum.Box, unit, violations);
            }
            return violations.AsReadOnly();
        }

        private static void FindDuplicates(
            SudokuGrid grid, UnitTypeEnum type, int unit, List<RuleViolation> violations)
        {
            var counts = new int[10];
            for (var offset = 0; offset < 9; offset++)
            {
                var position = type switch
                {
                    UnitTypeEnum.Row => new CellPosition(unit, offset),
                    UnitTypeEnum.Column => new CellPosition(offset, unit),
                    _ => new CellPosition(unit / 3 * 3 + offset / 3, unit % 3 * 3 + offset % 3)
                };
                var value = grid.GetValue(position);
                if (value != 0 && ++counts[value] == 2)
                    violations.Add(new RuleViolation(type, unit, value));
            }
        }
    }
}
