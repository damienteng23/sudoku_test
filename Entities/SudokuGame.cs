using sudoku.Enum;
using sudoku.ValueObject;

namespace sudoku.Entities
{
    /// <summary>Protects the puzzle clues and controls changes to a Sudoku game.</summary>
    public class SudokuGame
    {
        private readonly SudokuPuzzle puzzle;

        public SudokuGrid Grid { get; private set; }

        public bool IsComplete(SudokuGrid Grid)
        {
            return Grid.GetFilledCount() == SudokuGrid.CellCount && Check().Count == 0;
        }

        public SudokuGame(SudokuPuzzle puzzle)
        {
            this.puzzle = puzzle ?? throw new ArgumentNullException(nameof(puzzle));
            Grid = new SudokuGrid(puzzle.Clues.GetArray());
        }

        public MoveResult Place(CellPosition position, int number)
        {
            if (puzzle.Clues.GetValue(position) != 0)
                return new MoveResult(MoveErrorEnum.PreFilledCell, position);

            if (number < 1 || number > 9)
                return new MoveResult(MoveErrorEnum.NumberOutOfRange, position);

            Grid.SetValue(position, number);
            return new MoveResult(MoveErrorEnum.None, position, number);
        }

        public MoveResult Clear(CellPosition position)
        {
            if (puzzle.Clues.GetValue(position) != 0)
                return new MoveResult(MoveErrorEnum.PreFilledCell, position);

            Grid.SetValue(position, 0);
            return new MoveResult(MoveErrorEnum.None, position);
        }

        public IReadOnlyList<RuleViolation> Check()
        {
            var violations = new List<RuleViolation>();

            for (int row = 0; row < SudokuGrid.Size; row++)
                AddViolation(violations, UnitTypeEnum.Row, row, GetRow(row));

            for (int column = 0; column < SudokuGrid.Size; column++)
                AddViolation(violations, UnitTypeEnum.Column, column, GetColumn(column));

            for (int box = 0; box < SudokuGrid.Size; box++)
                AddViolation(violations, UnitTypeEnum.Box, box, GetBox(box));

            return violations;
        }

        public MoveResult Hint()
        {
            for (int index = 0; index < SudokuGrid.CellCount; index++)
            {
                var position = new CellPosition(index / SudokuGrid.Size, index % SudokuGrid.Size);

                if (puzzle.Clues.GetValue(position) == 0 &&
                    Grid.GetValue(position) != puzzle.Solution.GetValue(position))
                {
                    return Place(position, puzzle.Solution.GetValue(position));
                }
            }

            return new MoveResult(MoveErrorEnum.AlreadyComplete);
        }

        private int[] GetRow(int row)
        {
            var values = new int[SudokuGrid.Size];
            for (int column = 0; column < SudokuGrid.Size; column++)
                values[column] = Grid.GetValue(new CellPosition(row, column));
            return values;
        }

        private int[] GetColumn(int column)
        {
            var values = new int[SudokuGrid.Size];
            for (int row = 0; row < SudokuGrid.Size; row++)
                values[row] = Grid.GetValue(new CellPosition(row, column));
            return values;
        }

        private int[] GetBox(int box)
        {
            var values = new int[SudokuGrid.Size];
            int startRow = box / 3 * 3;
            int startColumn = box % 3 * 3;
            int index = 0;

            for (int row = startRow; row < startRow + 3; row++)
                for (int column = startColumn; column < startColumn + 3; column++)
                    values[index++] = Grid.GetValue(new CellPosition(row, column));

            return values;
        }

        private static void AddViolation(List<RuleViolation> violations, UnitTypeEnum unit, int unitIndex, int[] values)
        {
            for (int number = 1; number <= 9; number++)
            {
                int occurrences = values.Count(value => value == number);
                if (occurrences > 1)
                    violations.Add(new RuleViolation(unit, unitIndex, number));
            }
        }
    }
}
