using System;
using Checkers.CheckersGame.Validation.Config; // för RuleSetDto
namespace Checkers.CheckersGame.Validation;

public class RuleSet : IRuleSet
{
    public int BoardSize { get; }
    public bool ForcedCaptures { get; }
    public bool AllowMultipleJumps { get; }

    public RuleSet(int boardSize, bool forcedCaptures, bool allowMultipleJumps)
    {
        BoardSize = boardSize;
        ForcedCaptures = forcedCaptures;
        AllowMultipleJumps = allowMultipleJumps;
    }
    public override string ToString()
    {
        return $"{BoardSize}x{BoardSize}, " +
               $"Forced Captures: {ForcedCaptures}, " +
               $"Multiple Jumps: {AllowMultipleJumps}";
    }

}    

