using sudoku.Entities;
using sudoku.Enum;
using sudoku.Model;
using sudoku.Service;
using sudoku.ValueObject;

while (true)
{
    SudokuPuzzle puzzle = SudokuPuzzleFactory.CreateRandom(30);
    SudokuGame game = new SudokuGame(puzzle);
    CommandParser parser = new CommandParser();

    Console.WriteLine();
    Console.WriteLine("Sudoku command-line game");
    Console.WriteLine("Type 'help' for commands.");
    ConsoleBoardRenderer.Display(game.Grid);

    bool gameEnded = false;

    while (!gameEnded)
    {
        Console.Write("> ");
        string? input = Console.ReadLine();

        if (input == null)
        {
            return;
        }

        GameCommand? command = parser.Parse(input);

        if (command == null)
        {
            Console.WriteLine("Invalid command. Type 'help' for examples.");
            continue;
        }

        if (command.Type == CommandTypeEnum.Quit)
        {
            return;
        }

        if (command.Type == CommandTypeEnum.Help)
        {
            DisplayHelp();
            continue;
        }

        if (command.Type == CommandTypeEnum.Place && command.Position is { } placePosition)
        {
            DisplayMoveResult(game.Place(placePosition, command.Number));
            ConsoleBoardRenderer.Display(game.Grid);
        }
        else if (command.Type == CommandTypeEnum.Clear && command.Position is { } clearPosition)
        {
            DisplayMoveResult(game.Clear(clearPosition));
            ConsoleBoardRenderer.Display(game.Grid);
        }
        else if (command.Type == CommandTypeEnum.Hint)
        {
            DisplayHintResult(game.Hint());
            ConsoleBoardRenderer.Display(game.Grid);
        }
        else if (command.Type == CommandTypeEnum.Check)
        {
            DisplayViolations(game.Check());
        }

        if (game.IsComplete())
        {
            Console.WriteLine("Congratulations! You completed a valid Sudoku.");
            gameEnded = true;
        }
    }

    Console.Write("Start a new game? (y/n): ");
    string? answer = Console.ReadLine();

    if (!string.Equals(answer, "y", StringComparison.OrdinalIgnoreCase))
    {
        break;
    }
}

static void DisplayHelp()
{
    Console.WriteLine("Commands:");
    Console.WriteLine("  A1 5       Place number 5 in cell A1");
    Console.WriteLine("  A1 clear   Clear cell A1");
    Console.WriteLine("  hint       Fill one cell with a hint");
    Console.WriteLine("  check      Check for duplicate numbers");
    Console.WriteLine("  help       Show these commands");
    Console.WriteLine("  quit       Exit the game");
}

static void DisplayMoveResult(MoveResult result)
{
    if (result.Accepted)
    {
        Console.WriteLine("Move accepted.");
        return;
    }

    string location = result.Position is { } position ? $" at {position}" : string.Empty;
    string message = result.Error switch
    {
        MoveErrorEnum.PreFilledCell => $"Invalid move. {result.Position} is pre-filled.",
        MoveErrorEnum.NumberOutOfRange => $"Invalid move{location}. Enter a number from 1 to 9.",
        MoveErrorEnum.AlreadyComplete => "The puzzle is already complete.",
        _ => $"Move rejected{location}: {result.Error}."
    };
    Console.WriteLine(message);
}

static void DisplayHintResult(MoveResult result)
{
    if (result.Accepted && result.Position is { } position && result.Number is { } number)
    {
        Console.WriteLine($"Hint: Cell {position} = {number}");
        return;
    }

    DisplayMoveResult(result);
}

static void DisplayViolations(IReadOnlyList<RuleViolation> violations)
{
    if (violations.Count == 0)
    {
        Console.WriteLine("No rule violations found.");
        return;
    }

    foreach (RuleViolation violation in violations)
    {
        string unit = violation.Unit switch
        {
            UnitTypeEnum.Row => $"Row {(char)('A' + violation.UnitIndex)}",
            UnitTypeEnum.Column => $"Column {violation.UnitIndex + 1}",
            UnitTypeEnum.Box => $"Box {violation.UnitIndex + 1}",
            _ => violation.Unit.ToString()
        };
        Console.WriteLine($"Number {violation.Number} already exists in {unit}.");
    }
}
