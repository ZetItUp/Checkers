namespace Checkers.CheckersGame.Validation;

public class RuleSet
{
    public string Name { get; }
    public int BoardSize { get; }
    public bool ForcedCaptures { get; }
    public bool AllowBackwardCaptures { get; }
    public bool AllowMultipleJumps { get; }

    public RuleSet(string name, int boardSize, bool forcedCaptures, bool allowBackwardCaptures, bool allowMultipleJumps)
    {
        Name = name;
        BoardSize = boardSize;
        ForcedCaptures = forcedCaptures;
        AllowBackwardCaptures = allowBackwardCaptures;
        AllowMultipleJumps = allowMultipleJumps;
    }

    public static RuleSet CreateStandard()
    {
        return new RuleSet(
            name: "Standard American", 
            boardSize: 8, 
            forcedCaptures: true, 
            allowBackwardCaptures: false, 
            allowMultipleJumps: true
        );
    }

    
    public override string ToString()
    {
        return $"{Name} - {BoardSize}x{BoardSize}, " +
               $"Forced Captures: {ForcedCaptures}, " +
               $"Backward Captures: {AllowBackwardCaptures}, " +
               $"Multiple Jumps: {AllowMultipleJumps}";
    }
}    
