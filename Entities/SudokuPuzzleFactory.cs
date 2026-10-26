using sudoku.ValueObject;

namespace sudoku.Entities
{
    public static class SudokuPuzzleFactory
    {
        public static SudokuPuzzle CreateRandom(int clueCount = 30, Random? random = null)
        {
            if (clueCount < 0 || clueCount > SudokuGrid.CellCount)
                throw new ArgumentOutOfRangeException(nameof(clueCount));

            random ??= Random.Shared;

            int[] solution = CreateSolution(random);
            int[] clues = (int[])solution.Clone();
            int cellsToRemove = SudokuGrid.CellCount - clueCount;

            foreach (int index in Enumerable.Range(0, SudokuGrid.CellCount)
                         .OrderBy(_ => random.Next())
                         .Take(cellsToRemove))
            {
                clues[index] = 0;
            }

            return new SudokuPuzzle(new SudokuGrid(clues), new SudokuGrid(solution));
        }

        private static int[] CreateSolution(Random random)
        {
            int[] digits = Enumerable.Range(1, 9).OrderBy(_ => random.Next()).ToArray();
            int[] rows = CreateIndexes(random);
            int[] columns = CreateIndexes(random);
            int[] solution = new int[SudokuGrid.CellCount];

            for (int row = 0; row < SudokuGrid.Size; row++)
                for (int column = 0; column < SudokuGrid.Size; column++)
                {
                    int pattern = (rows[row] * 3 + rows[row] / 3 + columns[column]) % 9;
                    solution[row * SudokuGrid.Size + column] = digits[pattern];
                }

            return solution;
        }

        private static int[] CreateIndexes(Random random)
        {
            int[] bands = Enumerable.Range(0, 3).OrderBy(_ => random.Next()).ToArray();
            var indexes = new List<int>();

            foreach (int band in bands)
            {
                int[] group = Enumerable.Range(0, 3)
                    .OrderBy(_ => random.Next())
                    .Select(offset => band * 3 + offset)
                    .ToArray();
                indexes.AddRange(group);
            }

            return indexes.ToArray();
        }
    }
}
