using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Checkers.CheckersGame.Validation
{
    internal interface IRuleSet // dessa ska MoveValidator kunna hämta
    {
        string Name { get; }
        int BoardSize { get; }
        bool ForcedCaptures { get; }
        bool AllowBackwardCaptures { get; }
        bool AllowMultipleJumps { get; }
    }
    internal interface IRuleSetFactory
    {
        IRuleSet CreateFromJsonFile(string path); // läser in regler och skapar objekt

        IRuleSet CreateFromJson(string json); // skapar IRuleset objekt från sträng
    }
    
    internal sealed class RuleSetLoadException : Exception // felhantering
    {
        public RuleSetLoadException(string message) : base(message)
        {
            throw new RuleSetLoadException("BoardSize is missing in the JSON-file ");
        }
    }
}
