using sudoku.Enum;
using sudoku.ValueObject;
using System.Globalization;

namespace sudoku.Model
{
    public class CommandParser
    {
        public GameCommand? Parse(string input)
        {
            // Some terminals include a UTF-8 BOM in the first redirected input line.
            var parts = input.TrimStart('\uFEFF').Split((char[]?)null, StringSplitOptions.RemoveEmptyEntries);
            if (parts.Length == 1)
                return parts[0].ToLowerInvariant() switch
                {
                    "hint" => new(CommandTypeEnum.Hint),
                    "check" => new(CommandTypeEnum.Check),
                    "quit" => new(CommandTypeEnum.Quit),
                    "help" => new(CommandTypeEnum.Help),
                    _ => null
                };

            if (parts.Length != 2 || parts[0].Length != 2) return null;
            var row = char.ToUpperInvariant(parts[0][0]) - 'A';
            var column = parts[0][1] - '1';
            if (row is < 0 or > 8 || column is < 0 or > 8) return null;
            var position = new CellPosition(row, column);
            if (parts[1].Equals("clear", StringComparison.OrdinalIgnoreCase))
                return new(CommandTypeEnum.Clear, position);
            return int.TryParse(parts[1], NumberStyles.Integer, CultureInfo.InvariantCulture, out var number)
                ? new GameCommand(CommandTypeEnum.Place, position, number)
                : null;
        }
    }
}
