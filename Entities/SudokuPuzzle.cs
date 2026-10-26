using sudoku.ValueObject;

namespace sudoku.Entities
{
    public class SudokuPuzzle
    {
        public SudokuGrid Clues { get; }
        public SudokuGrid Solution { get; }

        public SudokuPuzzle(SudokuGrid clues, SudokuGrid solution)
        {
            Clues = clues ?? throw new ArgumentNullException(nameof(clues));
            Solution = solution ?? throw new ArgumentNullException(nameof(solution));
        }
    }
}
