using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Checkers.CheckersGame.Validation.ExceptionHandlers
{
    internal sealed class RuleSetLoadException : Exception // felhantering
    {
        public RuleSetLoadException(string message) : base(message)
        {
            throw new RuleSetLoadException("BoardSize is missing in the JSON-file ");
        }
    }
}
