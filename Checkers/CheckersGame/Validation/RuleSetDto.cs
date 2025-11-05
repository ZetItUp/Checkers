using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Text.Json.Serialization;

namespace Checkers.CheckersGame.Validation.Config
{
    /// <summary>
    /// Klass för att representera RuleSet-dataöverföringsobjekt (DTO) från JSON
    /// </summary>
    internal sealed class RuleSetDto
    {
        // Sätt JsonPropertyName attribut för att matcha JSON-egenskaper
        [JsonPropertyName("boardSize")]
        public int? BoardSize { get; set; }

        [JsonPropertyName("forcedCaptures")]
        public bool? ForcedCaptures { get; set; }

        [JsonPropertyName("allowMultipleJumps")]
        public bool? AllowMultipleJumps { get; set; }

    }
}
