using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Checkers.CheckersGame.Validation
{
    public interface IRuleSetFactory
    {
        IRuleSet CreateFromJsonFile(string path); // läser in regler och skapar objekt
    }
}
