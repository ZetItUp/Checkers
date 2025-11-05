using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Checkers.CheckersGame.Validation
{
    /// <summary>
    /// Interface för RuleSet
    /// </summary>
    public interface IRuleSet
    {
        int BoardSize { get; }
        bool ForcedCaptures { get; }
        bool AllowMultipleJumps { get; }
    }
}
