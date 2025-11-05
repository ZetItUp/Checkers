using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Checkers.CheckersGame.Validation
{
    /// <summary>
    /// Interface för RuleSetFactory
    /// </summary>
    public interface IRuleSetFactory
    {
        IRuleSet CreateFromJsonFile(string path);
    }
}
