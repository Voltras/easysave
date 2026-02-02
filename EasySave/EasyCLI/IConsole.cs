using System;
using System.Collections.Generic;
using System.Text;

namespace EasyCli;

public interface IConsole
{
    void Write(string message);
    void WriteLine(string message = "");
    void WriteError(string message);
    string? ReadLine();
}