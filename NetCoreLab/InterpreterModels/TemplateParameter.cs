using System;
using System.Collections.Generic;
using System.Linq;
using System.IO;
using System.Text;
using System.Text.RegularExpressions;
using System.Reflection;

namespace NetCoreLab.InterpreterModels
{
    class TemplateParameter
    {
        public string parameterPath;
        public string modifier;

        public TemplateParameter(string raw)
        {
            var parts = raw.Split('?');
            parameterPath = parts[0];
            if(parts.Length > 1)
            {
                modifier = parts[1];
            }
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

        public string Evaluate(object target)
        {
            var raw = AnonymExplorer.GetValueFromAnonym(target, parameterPath);
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
