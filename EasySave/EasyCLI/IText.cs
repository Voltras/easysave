using System;
using System.Collections.Generic;
using System.Text;

namespace EasyCli;
public interface IText
{
    string this[string key] { get; }
}
