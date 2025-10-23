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
        [JsonPropertyName("name")]
        public string? Name { get; set; }

        [JsonPropertyName("boardSize")]
        public int? BoardSize { get; set; }

        [JsonPropertyName("forcedCaptures")] // tvingat slag
        public bool? ForcedCaptures { get; set; }

        [JsonPropertyName("allowBackwardCaptures")]
        public bool? AllowBackwardCaptures { get; set; }

        [JsonPropertyName("allowBackwardCaptures")]
        public bool? AllowForwardCaptures { get; set; }

        [JsonPropertyName("allowMultipleJumps")]
        public bool? AllowMultipleJumps { get; set; }

        [JsonPropertyName("version")]
        public int? Version { get; set; }

    }
}
