using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Checkers.CheckersGame.Validation
{
    /// <summary>
    /// Exception som kastas när RuleSet inte kan laddas korrekt
    /// </summary>
    public sealed class RuleSetLoadException : Exception
    {
        public RuleSetLoadException(string message) : base(message)
        {
            throw new RuleSetLoadException("BoardSize is missing in the JSON-file ");
        }
    }
}
