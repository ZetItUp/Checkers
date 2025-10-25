using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Text.Json;
using System.Threading.Tasks;
using Checkers.CheckersGame.Validation.Config;

namespace Checkers.CheckersGame.Validation
{
    internal class RuleSetFactory : IRuleSetFactory // fabrik som skapar IRuleSet från JSON
    {
        public IRuleSet CreateFromJsonFile(string path) // läs in json-fil och skapa IRuleSet objekt
        {
            if (string.IsNullOrWhiteSpace(path))
                throw new ArgumentException("path cannot be empty.", nameof(path));

            if (!File.Exists(path))
                throw new FileNotFoundException($"Could not find file: {path}");

            var json = File.ReadAllText(path); // läser in filen som text
            return CreateFromJson(json); // skickar till metoden som hanterar Json-strängen
        }
        public IRuleSet CreateFromJson(string json)
        {
            if (string.IsNullOrEmpty(json))
                throw new ArgumentException("JSON får inte vara tom", nameof(json));

            // ej känslig för små/stora bokstäver
            var options = new JsonSerializerOptions { PropertyNameCaseInsensitive = true };

            // försök att konvertera JSON text till RuleSetDto
            var dto = JsonSerializer.Deserialize<RuleSetDto>(json, options);

            if (dto == null)
                throw new InvalidOperationException("Could not read JSON to RuleSetDto");

            return new RuleSet(         // skapa och returnera en rulseset från json
                dto.Name!.Trim(), 
                dto.BoardSize!.Value,
                dto.ForcedCaptures!.Value,
                dto.AllowBackwardCaptures!.Value,
                dto.AllowMultipleJumps!.Value
                );

                
        }
    }
}
