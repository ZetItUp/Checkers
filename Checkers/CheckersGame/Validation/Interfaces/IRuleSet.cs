using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Checkers.CheckersGame.Validation
{
    public interface IRuleSet // dessa ska MoveValidator kunna hämta
    {
        int BoardSize { get; }
        bool ForcedCaptures { get; }
        bool AllowMultipleJumps { get; }
    }
}
