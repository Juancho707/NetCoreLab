using System;
using System.Collections.Generic;
using System.Text.RegularExpressions;

namespace NetCoreLab.InterpreterModels
{
    /// <summary>
    /// Placeholder object.
    /// </summary>
    internal class Placeholder : IResolvable
    {
        private string parameterPath;
        private string modifier;
        private Tuple<int?, int?> trimRange;

        /// <summary>
        /// The concrete form of the placeholder representation.
        /// </summary>
        public string ConcreteForm { get; set; }

        private Placeholder(string raw)
        {
            ConcreteForm = "${" + RegExHelper.ClearStringFormatting(raw) + "}";
            var parts = raw.Split('?');
            parameterPath = parts[0];
            if (parts.Length > 1)
            {
                modifier = parts[1];
            }

            SetupTrimCommand();
        }

        private void SetupTrimCommand()
        {
            var matches = RegExHelper.GetContainedString(parameterPath, @"\[", "]");
            if (matches.Count > 0 && !string.IsNullOrEmpty(matches[0].Value))
            {
                var rawTrim = matches[0].Value;
                parameterPath = parameterPath.Replace("[" + rawTrim + "]", "");
                var trimValues = rawTrim.Split(new[] { ':' }, StringSplitOptions.RemoveEmptyEntries);

                if (trimValues.Length == 1)
                {
                    if (rawTrim[0] == ':')
                    {
                        trimRange = new Tuple<int?, int?>(null, int.Parse(trimValues[0]));
                    }
                    else
                    {
                        trimRange = new Tuple<int?, int?>(int.Parse(trimValues[0]), null);
                    }
                }
                else
                {
                    trimRange = new Tuple<int?, int?>(int.Parse(trimValues[0]), int.Parse(trimValues[1]));
                }
            }
        }

        private string ApplyTrimCommand(string raw)
        {
            if (trimRange != null)
            {
                if (trimRange.Item1.HasValue && trimRange.Item2.HasValue)
                {
                    return raw.Remove(trimRange.Item2.Value).Remove(0, trimRange.Item1.Value - 1);
                }
                else
                {
                    if (trimRange.Item1.HasValue)
                    {
                        return raw.Remove(0, trimRange.Item1.Value - 1);
                    }

                    return raw.Remove(trimRange.Item2.Value);
                }
            }

            return raw;
        }

        private string ApplyModifier(string input, string modifier)
        {
            switch (modifier)
            {
                case "cap_first":
                    var capFirst = input.ToLower().ToCharArray();
                    capFirst[0] = char.ToUpper(capFirst[0]);
                    return new string(capFirst);
                    break;
                case "upper_case":
                    return input.ToUpper();
                    break;
            }

            return input;
        }

        /// <summary>
        /// Resolve placeholder value and format.
        /// </summary>
        /// <param name="target">Target data context.</param>
        /// <returns>Formatted string.</returns>
        public string ResolveTemplate(object target)
        {
            var raw = JObjectHelper.GetValueFromAnonym(target, parameterPath);
            raw = ApplyTrimCommand(raw);
            return ApplyModifier(raw, modifier);
        }

        /// <summary>
        /// Creates a placeholder from a raw string.
        /// </summary>
        /// <param name="raw">Raw string.</param>
        /// <returns></returns>
        public static Placeholder FromRaw(string raw)
        {
            var matches = RegExHelper.GetContainedString(raw, "{", "}");
            if (matches.Count > 0)
            {
                var unwrapped = matches[0].Value;
                return new Placeholder(unwrapped);
            }

            return null;
        }

        /// <summary>
        /// Creates many placeholders from a raw string.
        /// </summary>
        /// <param name="raw">Raw string.</param>
        /// <returns></returns>
        public static Placeholder[] ManyFromRaw(string raw)
        {
            var matches = RegExHelper.GetContainedString(raw, "{", "}");
            if (matches.Count > 0)
            {
                var result = new List<Placeholder>();
                foreach (Match c in matches)
                {
                    var unwrapped = c.Value;
                    result.Add(new Placeholder(unwrapped));
                }

                return result.ToArray();
            }

            return null;
        }
    }
}
