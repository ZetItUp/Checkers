using System;

namespace Checkers.CheckersGame.Validation
{
    /// <summary>
    /// RuleSet klass som håller reda på spelregler (Det som går att ändra i JSON-filen)
    /// </summary>
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

        /// <summary>
        /// Override:ar ToString för att ge en beskrivning av RuleSet
        /// </summary>
        /// <returns>RuleSet string</returns>
        public override string ToString()
        {
            return $"{BoardSize}x{BoardSize}, " +
                   $"Forced Captures: {ForcedCaptures}, " +
                   $"Multiple Jumps: {AllowMultipleJumps}";
        }

    }
}

