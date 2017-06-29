using System;
using System.Collections.Generic;
using System.Linq;
using System.IO;
using System.Text;
using System.Text.RegularExpressions;
using System.Reflection;

namespace NetCoreLab.InterpreterModels
{
    class TemplateParameter: IResolvable
    {
        public string parameterPath;
        public string modifier;
        public Tuple<int?, int?> trimRange;

        public TemplateParameter(string raw)
        {
            var parts = raw.Split('?');
            parameterPath = parts[0];
            if(parts.Length > 1)
            {
                modifier = parts[1];
            }

            SetupTrimCommand();
        }

        void SetupTrimCommand()
        {
            var matches = Regex.Match(parameterPath, @"(?<=\[)(.*?)(?=\])");
            if (matches.Success)
            {
                var rawTrim = matches.Value;
                parameterPath = parameterPath.Replace("[" + rawTrim + "]", "");
                var trimValues = rawTrim.Split(new[] { ':' }, StringSplitOptions.RemoveEmptyEntries);
                
                if(trimValues.Length == 1)
                {
                    if(rawTrim[0] == ':')
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

        string ApplyTrimCommand(string raw)
        {
            if(trimRange != null)
            {
                if(trimRange.Item1.HasValue && trimRange.Item2.HasValue)
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

        public static TemplateParameter FromRaw(string raw)
        {
            var matches = Regex.Match(raw, @"(?<=\{)(.*?)(?=\})");
            if (matches.Captures.Count > 0)
            {
                var unwrapped = matches.Captures[0].ToString();
                return new TemplateParameter(unwrapped);
            }

            return null;
        }

        public string ResolveTemplate(object target)
        {
            var raw = AnonymExplorer.GetValueFromAnonym(target, parameterPath);
            raw = ApplyTrimCommand(raw);
            return ApplyModifier(raw, modifier);
        }

        string ApplyModifier(string input, string modifier)
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
    }
}
