using Checkers.CheckersGame.Validation.Config; // f�r RuleSetDto
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

    public static RuleSet CreateStandard() 
    {                                      
        return new RuleSet(
            boardSize: 8, 
            forcedCaptures: true,
            allowMultipleJumps: true
        );
    }

    
    public override string ToString()
    {
        return $"{BoardSize}x{BoardSize}, " +
               $"Forced Captures: {ForcedCaptures}, " +
               $"Multiple Jumps: {AllowMultipleJumps}";
    }
}    

