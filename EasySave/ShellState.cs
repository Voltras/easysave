using System.Globalization;

namespace EasyCli;

public sealed class ShellState
{
    public bool Running { get; set; } = true;

    /// <summary>
    /// When set by a command (e.g. settings), the shell will rebuild its text resources and context.
    /// </summary>
    public CultureInfo? RequestedCulture { get; set; }
}
