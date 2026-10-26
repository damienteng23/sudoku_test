using sudoku.Entities;
using sudoku.Enum;
using sudoku.Model;
using sudoku.Service;
using sudoku.ValueObject;

namespace Sudoku.Tests;

public class GameTests
{
    private static CellPosition Position(int index) => new CellPosition(index / 9, index % 9);

    [Fact]
    public void GeneratedPuzzlesHaveThirtyMatchingCluesAndLegalSolutions()
    {
        for (int seed = 0; seed < 100; seed++)
        {
            var puzzle = SudokuPuzzleFactory.CreateRandom(30, new Random(seed));
            Assert.Equal(30, puzzle.Clues.GetFilledCount());
            int[] solution = puzzle.Solution.GetArray();
            int[] expected = Enumerable.Range(1, 9).ToArray();
            for (int unit = 0; unit < 9; unit++)
            {
                Assert.Equal(expected, Enumerable.Range(0, 9).Select(i => solution[unit * 9 + i]).OrderBy(n => n));
                Assert.Equal(expected, Enumerable.Range(0, 9).Select(i => solution[i * 9 + unit]).OrderBy(n => n));
                Assert.Equal(expected, Enumerable.Range(0, 9).Select(i => solution[(unit / 3 * 3 + i / 3) * 9 + unit % 3 * 3 + i % 3]).OrderBy(n => n));
            }
            for (int i = 0; i < 81; i++)
                Assert.True(puzzle.Clues.GetValue(Position(i)) == 0 || puzzle.Clues.GetValue(Position(i)) == solution[i]);
        }
    }

    [Fact]
    public void CluesCannotBePlacedOrCleared()
    {
        var puzzle = SudokuPuzzleFactory.CreateRandom(30, new Random(1));
        var game = new SudokuGame(puzzle);
        for (int i = 0; i < 81; i++)
        {
            var position = Position(i);
            int clue = puzzle.Clues.GetValue(position);
            if (clue == 0) continue;
            Assert.Equal(MoveErrorEnum.PreFilledCell, game.Place(position, clue % 9 + 1).Error);
            Assert.Equal(MoveErrorEnum.PreFilledCell, game.Clear(position).Error);
            Assert.Equal(clue, game.Grid.GetValue(position));
        }
    }

    [Fact]
    public void GridPropertyReturnsSnapshotThatCannotBypassMoveRules()
    {
        var puzzle = SudokuPuzzleFactory.CreateRandom(30, new Random(1));
        var game = new SudokuGame(puzzle);
        int clueIndex = Enumerable.Range(0, 81)
            .First(index => puzzle.Clues.GetValue(Position(index)) != 0);
        var position = Position(clueIndex);
        int originalValue = game.Grid.GetValue(position);

        SudokuGrid exposedGrid = game.Grid;
        exposedGrid.SetValue(position, 1);

        Assert.Equal(originalValue, game.Grid.GetValue(position));
    }

    [Theory]
    [InlineData(-1)]
    [InlineData(0)]
    [InlineData(10)]
    public void InvalidNumbersLeaveBoardUnchanged(int number)
    {
        var game = new SudokuGame(SudokuPuzzleFactory.CreateRandom(0, new Random(1)));
        Assert.Equal(MoveErrorEnum.NumberOutOfRange, game.Place(Position(0), number).Error);
        Assert.Equal(0, game.Grid.GetFilledCount());
    }

    [Fact]
    public void PlaceAndClearChangeOnlyWorkingGrid()
    {
        var puzzle = SudokuPuzzleFactory.CreateRandom(0, new Random(1));
        var game = new SudokuGame(puzzle);
        Assert.True(game.Place(Position(10), 7).Accepted);
        Assert.Equal(7, game.Grid.GetValue(Position(10)));
        Assert.Equal(0, puzzle.Clues.GetValue(Position(10)));
        Assert.True(game.Clear(Position(10)).Accepted);
        Assert.Equal(0, game.Grid.GetFilledCount());
    }

    [Theory]
    [InlineData(0, 4, UnitTypeEnum.Row)]
    [InlineData(0, 36, UnitTypeEnum.Column)]
    [InlineData(0, 10, UnitTypeEnum.Box)]
    public void BothValidatorsDetectDuplicates(int first, int second, UnitTypeEnum unit)
    {
        var game = new SudokuGame(SudokuPuzzleFactory.CreateRandom(0, new Random(1)));
        game.Place(Position(first), 5);
        game.Place(Position(second), 5);
        var expected = new RuleViolation(unit, 0, 5);
        Assert.Equal(new[] { expected }, game.Check());
        Assert.Equal(new[] { expected }, GridValidator.Validate(game.Grid));
    }

    [Fact]
    public void HintsFillOneCellAtATimeAndCompleteThePuzzle()
    {
        var puzzle = SudokuPuzzleFactory.CreateRandom(30, new Random(1));
        var game = new SudokuGame(puzzle);
        Assert.False(game.IsComplete());
        for (int count = 31; count <= 81; count++)
        {
            var result = game.Hint();
            Assert.True(result.Accepted);
            Assert.NotNull(result.Position);
            Assert.Equal(puzzle.Solution.GetValue(result.Position), result.Number);
            Assert.Equal(count, game.Grid.GetFilledCount());
            Assert.Empty(game.Check());
        }
        Assert.True(game.IsComplete());
        Assert.Equal(MoveErrorEnum.AlreadyComplete, game.Hint().Error);
    }

    [Fact]
    public void AnyLegalCompletionIsAcceptedButFullInvalidGridIsNot()
    {
        var puzzle = SudokuPuzzleFactory.CreateRandom(0, new Random(1));
        var game = new SudokuGame(puzzle);
        for (int i = 0; i < 81; i++)
            game.Place(Position(i), puzzle.Solution.GetValue(Position(i)) % 9 + 1);
        Assert.True(game.IsComplete());
        game.Place(Position(0), game.Grid.GetValue(Position(1)));
        Assert.False(game.IsComplete());
    }

    [Theory]
    [InlineData("B3 7", CommandTypeEnum.Place)]
    [InlineData("c5 CLEAR", CommandTypeEnum.Clear)]
    [InlineData("hint", CommandTypeEnum.Hint)]
    [InlineData("check", CommandTypeEnum.Check)]
    [InlineData("quit", CommandTypeEnum.Quit)]
    [InlineData("help", CommandTypeEnum.Help)]
    public void ParserRecognizesCommands(string input, CommandTypeEnum expected)
    {
        Assert.Equal(expected, new CommandParser().Parse(input)!.Type);
    }

    [Theory]
    [InlineData("")]
    [InlineData("Z1 5")]
    [InlineData("A10 5")]
    [InlineData("A1 banana")]
    [InlineData("clear I9")]
    public void ParserRejectsMalformedInput(string input)
    {
        Assert.Null(new CommandParser().Parse(input));
    }

    [Fact]
    public void BoardRendererUsesUnderscoreForEmptyCells()
    {
        var originalOutput = Console.Out;
        using var output = new StringWriter();

        try
        {
            Console.SetOut(output);
            ConsoleBoardRenderer.Display(new SudokuGrid(new int[SudokuGrid.CellCount]));
        }
        finally
        {
            Console.SetOut(originalOutput);
        }

        string board = output.ToString();
        Assert.Contains("A | _ _ _ | _ _ _ | _ _ _ |", board);
        Assert.DoesNotContain(".", board);
    }
}
