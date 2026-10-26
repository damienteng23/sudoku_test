# Sudoku Command-Line Game

This project is a Sudoku game that runs in a command-line terminal. Each time the application starts, it generates a new Sudoku puzzle, displays the board, and lets the player place numbers, clear entries, request hints, check for rule violations, or start another game.

## Run the game with Docker

Start Docker Desktop or the Docker Engine, open a terminal in the project folder, and run:

```bash
docker compose build --no-cache
docker compose run --rm sudoku
```

The first command builds the Docker image. The second starts the interactive Sudoku game.

The project is written in C# and targets .NET 11. It can run directly with the .NET SDK or inside Docker.

## Requirements

To run it directly, install the .NET SDK version that supports the target framework in `sudoku.csproj`.

To run it in Docker, install Docker Desktop on Windows or macOS, or Docker Engine with Docker Compose on Linux.

## Run locally

From the project directory, run:

```bash
dotnet run
```

The application generates a puzzle and displays a board similar to:

```text
    1 2 3   4 5 6   7 8 9
  +-------+-------+-------+
A | _ _ _ | _ 9 7 | _ _ _ |
B | _ 4 _ | _ 1 _ | 9 5 _ |
C | 5 9 _ | 6 _ _ | _ 2 8 |
  +-------+-------+-------+
```

Rows are labelled `A` through `I`, columns are numbered `1` through `9`, and `_` represents an empty cell.

## Commands

Enter commands at the `>` prompt:

| Command | Description |
| --- | --- |
| `A1 5` | Place the number `5` in cell `A1` |
| `A1 clear` | Clear a player-entered value from `A1` |
| `hint` | Fill one editable cell using the generated solution |
| `check` | Report duplicate numbers in rows, columns, and boxes |
| `help` | Display the command list |
| `quit` | Exit the game |

The position must come first for placement and clearing commands. For example, use `A1 clear`, not `clear A1`.

Original puzzle clues cannot be changed or cleared. Numbers must be between `1` and `9` when placing a value.

## How the game works

The application follows this flow:

1. `SudokuPuzzleFactory.CreateRandom(30)` generates a complete valid Sudoku solution.
2. The factory copies the solution and removes cells until 30 clues remain.
3. `SudokuPuzzle` stores both the clue grid and the complete solution.
4. `SudokuGame` creates a working grid from the clues.
5. `ConsoleBoardRenderer` prints the working grid.
6. `CommandParser` converts terminal input into a `GameCommand`.
7. `SudokuGame` executes the requested operation.
8. `GridValidator` checks rows, columns, and 3×3 boxes for duplicate values.
9. The board is printed again after a placement, clear, or hint.
10. When the board is full and has no rule violations, the game ends and asks whether to start a new puzzle.

The game accepts any full board that follows Sudoku rules. It does not require the player to match the generated solution exactly. The generated solution is used by the hint command.

## Folder structure

```text
Entities/
```

Contains the main game objects:

- `SudokuGame.cs` controls gameplay, placement, clearing, hints, completion, and validation.
- `SudokuPuzzle.cs` stores the original clues and the generated solution.
- `SudokuPuzzleFactory.cs` creates randomized puzzle data.

```text
Enum/
```

Contains enumerations used throughout the application:

- `CommandTypeEnum.cs` lists supported commands.
- `MoveErrorEnum.cs` lists accepted and rejected move outcomes.
- `UnitTypeEnum.cs` identifies rows, columns, and boxes.

```text
Model/
```

Contains command and application input models:

- `GameCommand.cs` represents a parsed command.
- `CommandParser.cs` converts text such as `A1 5` into a `GameCommand`.
- `GameState.cs` represents a grid and completion state.

```text
Service/
```

Contains supporting application services:

- `GridValidator.cs` detects duplicate values in rows, columns, and boxes.
- `ConsoleBoardRenderer.cs` formats and prints the board in the terminal.

```text
ValueObject/
```

Contains small domain types:

- `CellPosition.cs` represents a row and column and calculates the flattened array index.
- `SudokuGrid.cs` stores the 81 cell values and provides grid operations.
- `MoveResult.cs` represents the outcome of placing, clearing, or hinting.
- `RuleViolation.cs` describes a duplicate found during validation.

## Puzzle generation

`SudokuPuzzleFactory.CreateRandom(30)` creates a randomized puzzle with 30 visible clues:

```csharp
SudokuPuzzle puzzle = SudokuPuzzleFactory.CreateRandom(30);
```

The number controls how many cells remain filled in the initial puzzle:

```csharp
SudokuPuzzle easy = SudokuPuzzleFactory.CreateRandom(45);
SudokuPuzzle harder = SudokuPuzzleFactory.CreateRandom(25);
```

More clues generally make a puzzle easier. Fewer clues generally make it harder.

The current generator creates a valid solution and removes values, but it does not count solutions to guarantee that the remaining clues have exactly one possible solution. The game still accepts any completed board that passes Sudoku validation.

## Docker
 
## Automated tests

With the .NET SDK installed, run from the project folder:

```bash
dotnet test sudoku.slnx
```

The `tests/Sudoku.Tests` project checks puzzle generation, clue protection, placement and clearing, number validation, duplicate detection, hints, completion, and command parsing. An end-to-end test runs the console application with redirected input, completes a puzzle, starts another game, and quits.

## Docker

Build the image from the project directory:

```bash
docker compose build
```

Run the game interactively:

```bash
docker compose run --rm sudoku
```

The `--rm` option removes the temporary container when the game exits.

You can rebuild and run after source changes with:

```bash
docker compose run --build --rm sudoku
```

The Compose service can also be started in the background:

```bash
docker compose up -d
```

Because Sudoku is an interactive program, attach to the running service with:

```bash
docker compose attach sudoku
```

For normal play, `docker compose run --rm sudoku` is simpler because it connects your terminal immediately.

Stop and remove Compose resources with:

```bash
docker compose down
```

## Git and generated files

`.gitignore` excludes generated build output and local IDE files, including:

- `bin/`
- `obj/`
- `.vs/`
- `.idea/`
- test results and coverage files

`.dockerignore` prevents these files from being copied into the Docker build context.
