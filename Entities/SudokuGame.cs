using sudoku.Enum;
using sudoku.Service;
using sudoku.ValueObject;

namespace sudoku.Entities
{
    /// <summary>Protects the puzzle clues and controls changes to a Sudoku game.</summary>
    public class SudokuGame
    {
        private readonly SudokuPuzzle puzzle;

        private SudokuGrid grid;

        public SudokuGrid Grid
        {
            get
            {
                return new SudokuGrid(grid.GetArray());
            }
        }

        public bool IsComplete()
        {
            return grid.GetFilledCount() == SudokuGrid.CellCount && Check().Count == 0;
        }

        public SudokuGame(SudokuPuzzle puzzle)
        {
            this.puzzle = puzzle ?? throw new ArgumentNullException(nameof(puzzle));
            grid = new SudokuGrid(puzzle.Clues.GetArray());
        }

        public MoveResult Place(CellPosition position, int number)
        {
            if (puzzle.Clues.GetValue(position) != 0)
                return new MoveResult(MoveErrorEnum.PreFilledCell, position);

            if (number < 1 || number > 9)
                return new MoveResult(MoveErrorEnum.NumberOutOfRange, position);

            grid.SetValue(position, number);
            return new MoveResult(MoveErrorEnum.None, position, number);
        }

        public MoveResult Clear(CellPosition position)
        {
            if (puzzle.Clues.GetValue(position) != 0)
                return new MoveResult(MoveErrorEnum.PreFilledCell, position);

            grid.SetValue(position, 0);
            return new MoveResult(MoveErrorEnum.None, position);
        }

        public IReadOnlyList<RuleViolation> Check()
        {
            return GridValidator.Validate(grid);
        }

        public MoveResult Hint()
        {
            for (int index = 0; index < SudokuGrid.CellCount; index++)
            {
                var position = new CellPosition(index / SudokuGrid.Size, index % SudokuGrid.Size);

                if (puzzle.Clues.GetValue(position) == 0 &&
                    grid.GetValue(position) != puzzle.Solution.GetValue(position))
                {
                    return Place(position, puzzle.Solution.GetValue(position));
                }
            }

            return new MoveResult(MoveErrorEnum.AlreadyComplete);
        }

    }
}
