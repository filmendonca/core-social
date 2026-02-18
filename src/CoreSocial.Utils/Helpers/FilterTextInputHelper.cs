using Ardalis.GuardClauses;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;

namespace Utils.Helpers
{
    public static class FilterTextInputHelper
    {
        public static string ValidateText(string text)
        {
            Guard.Against.NullOrWhiteSpace(text, nameof(text));
            text = text.Trim();
            var sanitized = Regex.Replace(text, @"[^a-zA-Z0-9\s-_.,:;!?()']", "");
            return sanitized;
        }
    }
}
