using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Text.Json.Serialization;

namespace Checkers.CheckersGame.Validation.Config
{
    internal sealed class RuleSetDto // sealed = ingen kan ärva från klassen
    {
        [JsonPropertyName("boardSize")]
        public int? BoardSize { get; set; }

        [JsonPropertyName("forcedCaptures")] // tvingat slag
        public bool? ForcedCaptures { get; set; }

        [JsonPropertyName("allowMultipleJumps")]
        public bool? AllowMultipleJumps { get; set; }

    }
}
