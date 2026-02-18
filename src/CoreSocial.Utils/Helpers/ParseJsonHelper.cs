using Ardalis.GuardClauses;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.Json;
using System.Threading.Tasks;

namespace Utils.Helpers
{
    public static class ParseJsonHelper
    {
        public static IReadOnlyDictionary<string, string?> Parse(string str)
        {
            Guard.Against.NullOrWhiteSpace(str, nameof(str));
            using var json = JsonDocument.Parse(str);
            JsonElement root = json.RootElement;
            var dict = new Dictionary<string, string?>();

            foreach (var prop in root.EnumerateObject())
            {
                //Get values from json depending on type
                switch (prop.Value.ValueKind)
                {
                    case JsonValueKind.String:
                        dict[prop.Name] = prop.Value.GetString();
                        break;
                    case JsonValueKind.Number:
                        dict[prop.Name] = prop.Value.GetRawText();
                        break;
                    case JsonValueKind.Null:
                        dict[prop.Name] = null;
                        break;
                    case JsonValueKind.True:
                        dict[prop.Name] = prop.Value.GetBoolean().ToString();
                        break;
                    case JsonValueKind.False:
                        dict[prop.Name] = prop.Value.GetBoolean().ToString();
                        break;
                    default:
                        dict[prop.Name] = prop.Value.GetRawText();
                        break;
                }
            }

            return dict;
        }
    }
}
